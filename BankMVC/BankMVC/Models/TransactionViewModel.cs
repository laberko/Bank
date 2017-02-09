using System;
using System.ComponentModel.DataAnnotations;

namespace BankMVC.Models
{
    public class TransactionViewModel
    {
        [Display(Name = "Приход")]
        public bool IsIncome { get; set; }
        [Display(Name = "Дата")]
        public DateTime? TimeStamp { get; set; }
        [Display(Name = "Банкнот $100")]
        public int Banknote100 { get; set; }
        [Display(Name = "Банкнот $500")]
        public int Banknote500 { get; set; }
        [Display(Name = "Банкнот $1000")]
        public int Banknote1000 { get; set; }
        [Display(Name = "Банкнот $5000")]
        public int Banknote5000 { get; set; }
        [Display(Name = "Общая сумма")]
        public int TransactionValue => Banknote100 * 100 + Banknote500 * 500 + Banknote1000 * 1000 + Banknote5000 * 5000;
    }
}