using System.ComponentModel.DataAnnotations;

namespace BankMVC.Models
{
    public class GetMoneyViewModel
    {
        [Display(Name = "Сумма")]
        public int Value { get; set; }
    }
}