using System;
using System.Linq;
using Autofac;
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
        public static LoginResult ProcessLogin(UserData userData) //return second until can login ? if 0 process login?
        {
            //Add somewhere check there is for user user with this nick? If not return false without any process

            if (!ProcessLoginAttempt(userData, out var timeoutDurationInSeconds))
                return new LoginResult
                {
                    IsSuccess = false,
                    TimeoutDurationInSeconds = timeoutDurationInSeconds
                };

            Login(userData);

            return new LoginResult
            {
                IsSuccess = true,
                TimeoutDurationInSeconds = 0
            };

            //Process login - show menu
        }

        private static bool ProcessLoginAttempt(UserData userData, out int timeoutDurationInSeconds)
        {
            int setTimeoutDurationInSeconds = 0;
            var currentTime = DateTime.Now;
            TimeSpan timeSinceLastLoginAttempt; // TODO
            var currentUserId = Context.Users.First(x => x.Login == userData.Login).Id;

            var lastLoginAttempt = Context.LoginAttempts.Where(x => x.IdUser == currentUserId && !x.IsStale).OrderByDescending(y => y.LoginAttemptDate).FirstOrDefault();
            if (lastLoginAttempt != null)
                timeSinceLastLoginAttempt = currentTime - lastLoginAttempt.LoginAttemptDate; // Złe odejmowanie czasu chyba
            else
                timeSinceLastLoginAttempt = TimeSpan.Zero;

            switch (CheckLoginPossibility(currentUserId))
            {
                case BlockLevel.None: //All good - could check data is valid
                    break;

                case BlockLevel.FirstLevel:
                    {
                        {
                            var deltaTime = (int)BlockLevel.FirstLevel - timeSinceLastLoginAttempt.Seconds;
                            timeoutDurationInSeconds = deltaTime > 0 ? deltaTime : 0;
                            {
                                setTimeoutDurationInSeconds = deltaTime;
                                //return false;
                            }
                        }
                        break;
                    }
                case BlockLevel.SecondLevel:
                    {
                        {
                            var deltaTime = (int)BlockLevel.SecondLevel - timeSinceLastLoginAttempt.Seconds;
                            timeoutDurationInSeconds = deltaTime > 0 ? deltaTime : 0;
                            {
                                setTimeoutDurationInSeconds = deltaTime;
                                //return false;
                            }
                        }
                        break;
                    }
                case BlockLevel.ThirdLevel:
                    {
                        {
                            var deltaTime = (int)BlockLevel.ThirdLevel - timeSinceLastLoginAttempt.Seconds;
                            timeoutDurationInSeconds = deltaTime > 0 ? deltaTime : 0;
                            {
                                setTimeoutDurationInSeconds = deltaTime;
                                //return false;
                            }
                        }
                        break;
                    }
            }

            if (!CheckLoginDataIsValid(userData)) // Check login data is correct
            {
                //If not save login try with fail - quit from app

                Context.LoginAttempts.Add(new LoginAttemptsDb
                {
                    LoginAttemptDate = DateTime.Now,
                    IsStale = false,
                    IdUser = currentUserId
                });

                Context.SaveChanges();

                //Fail - this time no ban
                {
                    timeoutDurationInSeconds = setTimeoutDurationInSeconds;
                    return false;
                }
            }

            //If ok - set all previous data of login trails with this file to state "stale"

            var previousLoginAttempts = Context.LoginAttempts.Where(x => x.IdUser == currentUserId).ToList();
            previousLoginAttempts.ForEach(x => x.IsStale = true);

            Context.SaveChanges();

            //Success - no ban
            {
                timeoutDurationInSeconds = 0;
                return true;
            }
        }

        private static BlockLevel CheckLoginPossibility(int userId)
        {
            var previousLoginAttempts = Context.LoginAttempts.Where(x => x.IdUser == userId && !x.IsStale).ToList();

            switch (previousLoginAttempts.Count)
            {
                case 0:
                    return BlockLevel.None;
                case 1:
                    return BlockLevel.FirstLevel;
                case 2:
                    return BlockLevel.SecondLevel;
                case 3:
                    return BlockLevel.ThirdLevel;
            }

            return previousLoginAttempts.Count >= 4 ? BlockLevel.ThirdLevel : BlockLevel.None;

            //Get all not stale data of tries login for this user

            //left block time counts from last failed login
            //0/1 fails = all good
            //2 fails = 5 sec block
            //3 fails = 10 sec block
            //4 fails = 2 min block

            //return enum? - BlockLevel
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

        public new static void Login(UserData userData)
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
    }
}