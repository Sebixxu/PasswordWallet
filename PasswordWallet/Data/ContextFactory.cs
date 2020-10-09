using Microsoft.EntityFrameworkCore;

namespace PasswordWallet.Data
{
    public class ContextFactory
    {
        public static DataContext GetContext()
        {
            var optionBuilder = new DbContextOptionsBuilder<DataContext>();
            optionBuilder.UseSqlServer(@"Server=DESKTOP-ON1599H;Database=TestDb;Trusted_Connection=True;");
            var context = new DataContext(optionBuilder.Options);

            return context;
        }
    }
}