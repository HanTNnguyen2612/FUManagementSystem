using BusinessObjects;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        public List<SystemAccount> GetAccounts() => SystemAccountDAO.GetAccounts();

        public SystemAccount GetAccountById(short id) => SystemAccountDAO.GetAccountById(id);

        public SystemAccount GetAccountByEmail(string email) => SystemAccountDAO.GetAccountByEmail(email);

        public List<SystemAccount> SearchAccounts(string keyword) => SystemAccountDAO.SearchAccounts(keyword);

        public void SaveAccount(SystemAccount account) => SystemAccountDAO.SaveAccount(account);

        public void UpdateAccount(SystemAccount account) => SystemAccountDAO.UpdateAccount(account);

        public void DeleteAccount(SystemAccount account) => SystemAccountDAO.DeleteAccount(account);
    }
}