using System;
using System.Data.Entity;

namespace BankService
{
    public class BankModel : DbContext
    {
        public BankModel() : base("name=BankDb")
        {
            //AppDomain.CurrentDomain.SetData("DataDirectory", @"D:\Cloud\GitHub\Bank\BankApp\bin\DataBase");
        }

        public DbSet<Atm> Atm { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

    }
}
