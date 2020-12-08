using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Newtonsoft.Json;
using PasswordWallet.Crypto.Interfaces;
using PasswordWallet.Data.DbModels;
using PasswordWallet.Helpers;
using PasswordWallet.Models.Classes;
using PasswordWallet.Models.Classes.Results;
using PasswordWallet.Models.Enums;

namespace PasswordWallet.BusinessLogic
{
    public class AccountManagement : Configuration
    {
        public static LoginResult ProcessLoginAttemptByIp(UserData userData)
        {
            var currentTime = DateTime.Now;

            var currentUserId = Context.Users.First(x => x.Login == userData.Login).Id;
            var currentIpAddress = GetCurrentIpAddress();
            var previousIpAttempts = Context.IpAttempts.Where(x => x.IpAddress == currentIpAddress && !x.IsStale && !x.WasSuccess).ToList();
            var countPreviousIpFails = previousIpAttempts.Count;

            if (CheckLoginDataIsValid(userData))
            {
                if (countPreviousIpFails == 0 || countPreviousIpFails == 1)
                {
                    var ipAttemptsDb = new IpAttemptsDb
                    {
                        LoginAttemptDate = DateTime.Now,
                        IpAddress = currentIpAddress,
                        IsStale = true,
                        WasSuccess = true,
                        IdUser = currentUserId
                    };

                    Context.IpAttempts.Add(ipAttemptsDb);

                    Context.SaveChanges();

                    var loginAttemptResult = ProcessLoginAttemptByUserData(userData); // logowanie poprawne - obsługa konta

                    return loginAttemptResult;
                }
                else
                {
                    //Dane były poprawne, ale było 2 lub więcej błędnych prób logowania z tego ip

                    int banTimeout = GetBanTimeoutDurationForIp(countPreviousIpFails); // TODO obsługa perma bana, -1 czas bana

                    var lastIpAttempt = Context.IpAttempts
                        .Where(x => x.IpAddress == currentIpAddress && !x.IsStale && !x.WasSuccess)
                        .OrderByDescending(y => y.LoginAttemptDate).FirstOrDefault();

                    TimeSpan timeSinceLastIpAttempt = currentTime - lastIpAttempt.LoginAttemptDate;

                    if (banTimeout == -1) //perma ban
                    {
                        return new LoginResult
                        {
                            TimeoutDurationInSeconds = banTimeout,
                            IsSuccess = false
                        };
                    }
                    else if (timeSinceLastIpAttempt.TotalSeconds >= banTimeout)
                    {
                        var currentPreviousIpAttempts = Context.IpAttempts.Where(x => x.IpAddress == currentIpAddress && !x.IsStale && !x.WasSuccess).ToList();
                        currentPreviousIpAttempts?.ForEach(x => x.IsStale = true);

                        //zapis poprawnej próby logowania z tego IP - co z kejsem gdzie tutaj zapisze poprawne logowanie, ale na koncie jest ban i tam się to nie zapisze?
                        var ipAttemptsDb = new IpAttemptsDb
                        {
                            LoginAttemptDate = DateTime.Now,
                            IpAddress = currentIpAddress,
                            IsStale = true,
                            WasSuccess = true,
                            IdUser = currentUserId
                        };

                        Context.IpAttempts.Add(ipAttemptsDb);
                        Context.SaveChanges();

                        var loginAttemptResult = ProcessLoginAttemptByUserData(userData); // logowanie "potencjalnie" poprawne - obsługa konta
                        return loginAttemptResult;
                    }
                    else
                    {
                        //zostało timeSinceLastLoginAttempt.TotalSeconds bana
                        return new LoginResult
                        {
                            TimeoutDurationInSeconds = banTimeout - (int)timeSinceLastIpAttempt.TotalSeconds,
                            IsSuccess = false
                        };

                        //brak obsługi konta
                    }
                }
            }
            else
            {
                ProcessLoginAttemptByUserData(userData);

                var ipAttemptsDb = new IpAttemptsDb
                {
                    LoginAttemptDate = DateTime.Now,
                    IpAddress = currentIpAddress,
                    IsStale = false,
                    WasSuccess = false,
                    IdUser = currentUserId
                };

                Context.IpAttempts.Add(ipAttemptsDb);

                Context.SaveChanges();

                int newTimeout = GetBanTimeoutDurationToShowForIp(countPreviousIpFails);

                return new LoginResult
                {
                    TimeoutDurationInSeconds = newTimeout,
                    IsSuccess = false
                };
                //skip obsługi konta ?
            }
        }

        public static LoginResult ProcessLoginAttemptByUserData(UserData userData)
        {
            var currentTime = DateTime.Now;

            var currentUserId = Context.Users.First(x => x.Login == userData.Login).Id;
            var previousLoginAttempts = Context.LoginAttempts
                .Where(x => x.IdUser == currentUserId && !x.IsStale && !x.WasSuccess).ToList();
            var countPreviousLoginFails = previousLoginAttempts.Count;

            if (CheckLoginDataIsValid(userData))
            {
                //Dane są poprawne

                if (countPreviousLoginFails == 0 || countPreviousLoginFails == 1)
                {
                    var currentPreviousLoginAttempts = Context.LoginAttempts
                        .Where(x => x.IdUser == currentUserId && !x.IsStale && !x.WasSuccess).ToList();
                    currentPreviousLoginAttempts?.ForEach(x => x.IsStale = true);

                    SaveLoginAttempt(currentTime, currentUserId, true, true);

                    return new LoginResult
                    {
                        TimeoutDurationInSeconds = 0,
                        IsSuccess = true
                    };
                }
                else
                {
                    int banTimeout = GetBanTimeoutDuration(countPreviousLoginFails);

                    var lastLoginAttempt = Context.LoginAttempts
                        .Where(x => x.IdUser == currentUserId && !x.IsStale && !x.WasSuccess)
                        .OrderByDescending(y => y.LoginAttemptDate).FirstOrDefault();

                    var timeSinceLastLoginAttempt = CalculateTimeSinceLastLoginAttempt(currentTime, lastLoginAttempt);

                    if (timeSinceLastLoginAttempt.TotalSeconds >= banTimeout)
                    {
                        var currentPreviousLoginAttempts = Context.LoginAttempts
                            .Where(x => x.IdUser == currentUserId && !x.IsStale && !x.WasSuccess).ToList();
                        currentPreviousLoginAttempts?.ForEach(x => x.IsStale = true);

                        SaveLoginAttempt(currentTime, currentUserId, true, true);

                        //Ban minął
                        return new LoginResult
                        {
                            TimeoutDurationInSeconds = 0,
                            IsSuccess = true
                        };
                    }
                    else
                    {
                        //zostało timeSinceLastLoginAttempt.TotalSeconds bana
                        return new LoginResult
                        {
                            TimeoutDurationInSeconds = banTimeout - (int)timeSinceLastLoginAttempt.TotalSeconds,
                            IsSuccess = false
                        };
                    }
                }
            }
            else
            {
                //Dane są błędne

                SaveLoginAttempt(currentTime, currentUserId, false, false);

                int newTimeout = GetBanTimeoutDurationToShow(countPreviousLoginFails);

                return new LoginResult
                {
                    TimeoutDurationInSeconds = newTimeout,
                    IsSuccess = false
                };
            }
        }

        public static TimeSpan CalculateTimeSinceLastLoginAttempt(DateTime currentTime, LoginAttemptsDb lastLoginAttempt)
        {
            TimeSpan timeSinceLastLoginAttempt = currentTime - lastLoginAttempt.LoginAttemptDate;

            return timeSinceLastLoginAttempt;
        }

        private static void SaveLoginAttempt(DateTime currentTime, int currentUserId, bool isStale, bool wasSuccess)
        {
            Context.LoginAttempts.Add(new LoginAttemptsDb
            {
                LoginAttemptDate = currentTime,
                IsStale = isStale,
                WasSuccess = wasSuccess,
                IdUser = currentUserId,
            });

            Context.SaveChanges();
        }

        private static string GetCurrentIpAddress()
        {
            IpInfo ipInfo;
            using (StreamReader r = new StreamReader("ipAddresses.json"))
            {
                string json = r.ReadToEnd();
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(json);
            }

            return ipInfo.IpAddresses[ipInfo.CurrentIpIndex];
        }

        //todo je do helpera ip i account
        public static int GetBanTimeoutDuration(int countPreviousFails)
        {
            int banTimeout;
            switch (countPreviousFails)
            {
                case 0:
                    banTimeout = 0;
                    break;
                case 1:
                    banTimeout = 0;
                    break;
                case 2:
                    banTimeout = 30;
                    break;
                case 3:
                    banTimeout = 120;
                    break;
                default:
                    banTimeout = 120;
                    break;
            }

            return banTimeout;
        }

        public static int GetBanTimeoutDurationForIp(int countPreviousFails)
        {
            int banTimeout;
            switch (countPreviousFails)
            {
                case 0:
                    banTimeout = 0;
                    break;
                case 1:
                    banTimeout = 0;
                    break;
                case 2:
                    banTimeout = 30;
                    break;
                case 3:
                    banTimeout = 60;
                    break;
                default:
                    banTimeout = -1;
                    break;
            }

            return banTimeout;
        }

        public static int GetBanTimeoutDurationToShow(int countPreviousFails)
        {
            int banTimeout;
            switch (countPreviousFails)
            {
                case 0:
                    banTimeout = 0;
                    break;
                case 1:
                    banTimeout = 30;
                    break;
                case 2:
                    banTimeout = 60;
                    break;
                case 3:
                    banTimeout = 120;
                    break;
                default:
                    banTimeout = 120;
                    break;
            }

            return banTimeout;
        }

        public static int GetBanTimeoutDurationToShowForIp(int countPreviousFails)
        {
            int banTimeout;
            switch (countPreviousFails)
            {
                case 0:
                    banTimeout = 0;
                    break;
                case 1:
                    banTimeout = 30;
                    break;
                case 2:
                    banTimeout = 60;
                    break;
                case 3:
                    banTimeout = -1;
                    break;
                default:
                    banTimeout = -1;
                    break;
            }

            return banTimeout;
        }

        public static IEnumerable<LoginAttemptData> GetLoginAttemptsView()
        {
            IList<LoginAttemptData> attemptsAttemptsData = new List<LoginAttemptData>();

            var user = Context.Users.First(x => x.Login == UserName);
            var loginAttempts = Context.LoginAttempts.Where(x => x.IdUser == user.Id).OrderByDescending(x => x.LoginAttemptDate).ToList();

            foreach (var loginAttempt in loginAttempts)
            {
                attemptsAttemptsData.Add(new LoginAttemptData
                {
                    LoginAttemptDate = loginAttempt.LoginAttemptDate,
                    WasSuccess = loginAttempt.WasSuccess
                });
            }

            return attemptsAttemptsData;
        }

        public static IList<string> GetPermanentlyBannedIpAddresses()
        {
            var query =
                from i in Context.IpAttempts
                where i.IsStale == false && i.WasSuccess == false
                group i by i.IpAddress
                into g
                select new
                {
                    ipAddress = g.Key,
                    count = g.Count()
                };

            var permanentlyBannedIpAddresses = query.Where(x => x.count >= 4).Select(y => y.ipAddress).ToList();

            return permanentlyBannedIpAddresses;
        }

        public static void UnbanIpAddress(string ipAddress)
        {
            var attemptsWithThisAddress = Context.IpAttempts.Where(x => x.IpAddress == ipAddress);

            foreach (var ipAttempts in attemptsWithThisAddress)
            {
                ipAttempts.IsStale = true;
                ipAttempts.WasSuccess = true;
            }

            Context.SaveChanges();
        }

        public static string Register(UserData userData, CryptoEnum cryptoEnum) //TODO Kolizja nazw - zajętość
        {
            Container.Resolve<ICryptoStrategy>().Encrypt(userData.Password, out var passwordHash, out var passwordSalt); //Hash password
            var hmaced = cryptoEnum == CryptoEnum.HMAC;

            Context.Users.Add(new UserDb //Prepare object and add to User table
            {
                IsHMAC = hmaced,
                Login = userData.Login,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });

            Context.SaveChanges(); //Save to Db

            return "Registration was successful.";
        }

        public static void Login(UserData userData)
        {
            UserName = userData.Login;
            Password = userData.Password.StringToSecureString();
        }

        private static bool CheckLoginDataIsValid(UserData userData)
        {
            var user = Context.Users.FirstOrDefault(x => x.Login == userData.Login);
            if (user == null)
                return false;

            return Container.Resolve<ICryptoStrategy>().VerifyPasswordHash(userData.Password, user.PasswordHash, user.PasswordSalt);
        }

        public static void ChangePassword(string oldPassword, string newPassword)
        {
            var user = Context.Users.First(x => x.Login == UserName);
            var isPasswordValid = Container.Resolve<ICryptoStrategy>().VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt);

            if (!isPasswordValid)
                return;

            Container.Resolve<ICryptoStrategy>().Encrypt(newPassword, out var passwordHash, out var passwordSalt); //Get hash for new password

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            PasswordManagement.RecryptPasswordForUser(newPassword.StringToSecureString()); //Recrypt website passwords

            Password = newPassword.StringToSecureString(); //Cache new password

            Context.SaveChanges();
        }

        public static CryptoEnum UserCryptoType(string userLogin)
        {
            var user = Context.Users.First(x => x.Login == userLogin); //TODO Obsługa braku usera o podanym loginie

            return user.IsHMAC ? CryptoEnum.HMAC : CryptoEnum.SHA512;
        }

        public static IEnumerable<UserInfo> GetAllOtherUserInfo()
        {
            IList<UserInfo> userInfos = new List<UserInfo>();
            var userDbs = Context.Users.ToList();

            foreach (var userDb in userDbs)
            {
                userInfos.Add(
                    new UserInfo
                    {
                        Id = userDb.Id,
                        Username = userDb.Login
                    });
            }

            userInfos.Remove(userInfos.Single(x => x.Username == UserName)); //Remove current user

            return userInfos;
        }
    }
}