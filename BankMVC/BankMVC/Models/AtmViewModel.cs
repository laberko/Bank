using System.ComponentModel.DataAnnotations;

namespace BankMVC.Models
{
    public class AtmViewModel
    {
        [Display(Name = "Банкнот $100:")]
        public int Banknote100 { get; set; }
        [Display(Name = "Банкнот $500:")]
        public int Banknote500 { get; set; }
        [Display(Name = "Банкнот $1000:")]
        public int Banknote1000 { get; set; }
        [Display(Name = "Банкнот $5000:")]
        public int Banknote5000 { get; set; }
        [Display(Name = "Остаток на счете:")]
        public int AccountValue { get; set; }
        [Display(Name = "Остаток в банкомате:")]
        public int AtmValue => Banknote100 * 100 + Banknote500 * 500 + Banknote1000 * 1000 + Banknote5000 * 5000;
    }
}