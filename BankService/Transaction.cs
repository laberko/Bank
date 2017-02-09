using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace BankService
{
    [DataContract(Name = "Transaction", Namespace = "http://laberko.net")]
    public class Transaction
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime? TimeStamp { get; set; }
        [DataMember]
        public bool IsIncome { get; set; }
        [DataMember]
        public int Banknote100 { get; set; }
        [DataMember]
        public int Banknote500 { get; set; }
        [DataMember]
        public int Banknote1000 { get; set; }
        [DataMember]
        public int Banknote5000 { get; set; }
        [NotMapped]
        public int TransactionValue => Banknote100 * 100 + Banknote500 * 500 + Banknote1000 * 1000 + Banknote5000 * 5000;
    }
}
