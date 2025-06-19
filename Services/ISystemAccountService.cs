using BusinessObjects;
using System.Collections.Generic;

namespace Services
{
    public interface ISystemAccountService
    {
        List<SystemAccount> GetAccounts();
        SystemAccount GetAccountById(short id);
        SystemAccount GetAccountByEmail(string email);
        List<SystemAccount> SearchAccounts(string keyword);
        void SaveAccount(SystemAccount account);
        void UpdateAccount(SystemAccount account);
        void DeleteAccount(SystemAccount account);
    }
}