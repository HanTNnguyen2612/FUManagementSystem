using BusinessObjects;

namespace Services
{
    public interface ISystemAccountService
    {
        SystemAccount GetAccountByEmail(string email);
        SystemAccount GetAccountById(short id);
        void SaveAccount(SystemAccount account);
        void UpdateAccount(SystemAccount account);
        void DeleteAccount(SystemAccount account);
    }
}