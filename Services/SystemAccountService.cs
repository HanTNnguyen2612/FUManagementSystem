using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;

namespace Services
{
    public class SystemAccountService : ISystemAccountService
    {
        private readonly ISystemAccountRepository _systemAccountRepository;

        public SystemAccountService(ISystemAccountRepository systemAccountRepository)
        {
            _systemAccountRepository = systemAccountRepository;
        }

        public SystemAccount GetAccountById(short id)
        {
            return _systemAccountRepository.GetAccountById(id);
        }

        public SystemAccount GetAccountByEmail(string email)
        {
            return _systemAccountRepository.GetAccountByEmail(email);
        }

        public void SaveAccount(SystemAccount account)
        {
            if (!ValidateSystemAccount(account))
                throw new ArgumentException("Invalid account data.");

            _systemAccountRepository.SaveAccount(account);
        }

        public void UpdateAccount(SystemAccount account)
        {
            if (!ValidateSystemAccount(account))
                throw new ArgumentException("Invalid account data.");

            _systemAccountRepository.UpdateAccount(account);
        }

        public void DeleteAccount(SystemAccount account)
        {
            if (account == null)
                throw new ArgumentException("Account not found.");

            _systemAccountRepository.DeleteAccount(account);
        }

        // Đây là method phụ để kiểm tra tính hợp lệ, không thuộc interface
        public bool ValidateSystemAccount(SystemAccount account)
        {
            if (account == null)
                return false;

            if (string.IsNullOrWhiteSpace(account.AccountName) || account.AccountName.Length > 100)
                return false;

            if (string.IsNullOrWhiteSpace(account.AccountEmail) || account.AccountEmail.Length > 70)
                return false;

            if (string.IsNullOrWhiteSpace(account.AccountPassword) || account.AccountPassword.Length > 70)
                return false;

            if (!account.AccountRole.HasValue || account.AccountRole < 0)
                return false;

            return true;
        }
    }
}
