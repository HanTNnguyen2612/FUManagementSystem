using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        public SystemAccount GetAccountByEmail(string email) => SystemAccountDAO.GetAccountByEmail(email);

        public SystemAccount GetAccountById(short id) => SystemAccountDAO.GetAccountById(id);

        public void SaveAccount(SystemAccount account) => SystemAccountDAO.SaveAccount(account);

        public void UpdateAccount(SystemAccount account) => SystemAccountDAO.UpdateAccount(account);

        public void DeleteAccount(SystemAccount account) => SystemAccountDAO.DeleteAccount(account);
    }
}