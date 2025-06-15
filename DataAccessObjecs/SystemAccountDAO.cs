using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DataAccessObjects
{
    public class SystemAccountDAO
    {
        public static SystemAccount GetAccountByEmail(string email)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.SystemAccounts.FirstOrDefault(a => a.AccountEmail == email);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting account: " + e.Message);
            }
        }

        public static SystemAccount GetAccountById(short id)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.SystemAccounts.FirstOrDefault(a => a.AccountId == id);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting account: " + e.Message);
            }
        }

        public static void SaveAccount(SystemAccount account)
        {
            try
            {
                using (var context = new FunewsManagementContext())
                {
                    context.SystemAccounts.Add(account);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void UpdateAccount(SystemAccount account)
        {
            try
            {
                using (var context = new FunewsManagementContext())
                {
                    var existingAccount = context.SystemAccounts.FirstOrDefault(a => a.AccountId == account.AccountId);
                    if (existingAccount != null)
                    {
                        existingAccount.AccountName = account.AccountName;
                        existingAccount.AccountEmail = account.AccountEmail;
                        existingAccount.AccountRole = account.AccountRole;
                        existingAccount.AccountPassword = account.AccountPassword;
                        context.Entry(existingAccount).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void DeleteAccount(SystemAccount account)
        {
            try
            {
                using (var context = new FunewsManagementContext())
                {
                    var existingAccount = context.SystemAccounts.FirstOrDefault(a => a.AccountId == account.AccountId);
                    if (existingAccount != null)
                    {
                        context.SystemAccounts.Remove(existingAccount);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}