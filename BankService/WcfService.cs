using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BankService
{
    public class WcfService : IWcfService
    {
        private static readonly object Locker = new object();
        public WcfService()
        {
            //var appSetting = ConfigurationManager.AppSettings["dataDir"];
            //var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            //var path = Path.Combine(baseDir, appSetting);
            //var fullPath = Path.GetFullPath(path);
            //AppDomain.CurrentDomain.SetData("DataDirectory", fullPath);
            //AppDomain.CurrentDomain.SetData("DataDirectory", @"D:\Cloud\GitHub\Bank\BankApp\bin\DataBase");
            try
            {
                using (var db = new BankModel())
                {
                    if (!db.Atm.Any())
                    {
                        db.Atm.Add(new Atm
                        {
                            Banknote100 = 20,
                            Banknote500 = 20,
                            Banknote1000 = 20,
                            Banknote5000 = 20,
                            AccountValue = 100000
                        });
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        //get last transactions
        public List<Transaction> GetTransactions(int count)
        {
            var list = new List<Transaction>();
            try
            {
                using (var db = new BankModel())
                {
                    if (count == 0)
                        list = db.Transactions.OrderByDescending(t => t.Id).ToList();
                    else
                    {
                        foreach (var t in db.Transactions.OrderByDescending(t => t.Id).Take(count))
                        {
                            list.Add(t);
                        }
                    }
                }
                Log("List of transactions sent to client. Count = " + list.Count);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            return list;
        }

        //get current atm and accoun state
        public Atm GetAtmState()
        {
            Atm atm = null;
            try
            {
                using (var db = new BankModel())
                {
                    atm = db.Atm.FirstOrDefault();
                }
                Log("ATM state sent to client.");
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            return atm;
        }

        //put money to account
        public async void PutMoneyAsync(Transaction transaction)
        {
            try
            {
                if (transaction.TransactionValue <= 0)
                    return;
                using (var db = new BankModel())
                {
                    var atm = db.Atm.FirstOrDefault();
                    if (atm == null)
                        return;
                    transaction.IsIncome = true;
                    transaction.TimeStamp = DateTime.Now;
                    db.Transactions.Add(transaction);
                    atm.Banknote100 += transaction.Banknote100;
                    atm.Banknote500 += transaction.Banknote500;
                    atm.Banknote1000 += transaction.Banknote1000;
                    atm.Banknote5000 += transaction.Banknote5000;
                    atm.AccountValue += transaction.TransactionValue;
                    db.Entry(atm).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                Log(transaction.TransactionValue + " put to account.");
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        public async Task<Transaction> GetMoneyAsync(int money, bool confirmed)
        {
            if (money <= 0)
                return null;
            Transaction transaction = null;
            try
            {
                using (var db = new BankModel())
                {
                    var atm = db.Atm.FirstOrDefault();
                    if (atm == null)
                        return null;
                    if (money > atm.AtmValue)
                        money = atm.AtmValue;
                    if (money > atm.AccountValue)
                        money = atm.AccountValue;
                    transaction = new Transaction();
                    while (money >= 5000 && atm.Banknote5000 > 0)
                    {
                        atm.Banknote5000--;
                        transaction.Banknote5000++;
                        money -= 5000;
                    }
                    while (money >= 1000 && atm.Banknote1000 > 0)
                    {
                        atm.Banknote1000--;
                        transaction.Banknote1000++;
                        money -= 1000;
                    }
                    while (money >= 500 && atm.Banknote500 > 0)
                    {
                        atm.Banknote500--;
                        transaction.Banknote500++;
                        money -= 500;
                    }
                    while (money >= 100 && atm.Banknote100 > 0)
                    {
                        atm.Banknote100--;
                        transaction.Banknote100++;
                        money -= 100;
                    }
                    if (confirmed)
                    {
                        transaction.TimeStamp = DateTime.Now;
                        db.Transactions.Add(transaction);
                        atm.AccountValue -= transaction.TransactionValue;
                        db.Entry(atm).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
                Log(transaction.TransactionValue + " taken from account.");
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            return transaction;
        }

        public static void Log(string text)
        {
            text = DateTime.Now.ToString("T") + " : " + text;
            Console.WriteLine(text);
            try
            {
                lock (Locker)
                {
                    using (var logFile = new StreamWriter("log.txt", true))
                    {
                        logFile.WriteLine(text);
                        logFile.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
