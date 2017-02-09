using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace BankService
{
    [ServiceContract]
    public interface IWcfService
    {
        [OperationContract]
        List<Transaction> GetTransactions(int count);

        [OperationContract]
        Atm GetAtmState();

        [OperationContract]
        void PutMoneyAsync(Transaction transaction);

        [OperationContract]
        Task<Transaction> GetMoneyAsync(int money, bool confirmed);
    }
}
