using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessObjects
{
    public class SystemAccountDAO
    {
        public static List<SystemAccount> GetAccounts()
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.SystemAccounts.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting accounts: " + e.Message);
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
                throw new Exception("Error getting account by ID: " + e.Message);
            }
        }

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
                throw new Exception("Error getting account by email: " + e.Message);
            }
        }

        public static List<SystemAccount> SearchAccounts(string keyword)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.SystemAccounts
                        .Where(a => a.AccountName.Contains(keyword) || a.AccountEmail.Contains(keyword))
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error searching accounts: " + e.Message);
            }
        }

        public static void SaveAccount(SystemAccount account)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    db.SystemAccounts.Add(account);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error saving account: " + e.Message);
            }
        }

        public static void UpdateAccount(SystemAccount account)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    var existingAccount = db.SystemAccounts.FirstOrDefault(a => a.AccountId == account.AccountId);
                    if (existingAccount == null)
                        throw new Exception("Account not found.");

                    existingAccount.AccountName = account.AccountName;
                    existingAccount.AccountEmail = account.AccountEmail;
                    existingAccount.AccountPassword = account.AccountPassword;
                    existingAccount.AccountRole = account.AccountRole;

                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error updating account: " + e.Message);
            }
        }

        public static void DeleteAccount(SystemAccount account)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    if (db.NewsArticles.Any(n => n.CreatedById == account.AccountId || n.UpdatedById == account.AccountId))
                        throw new Exception("Cannot delete account as it is associated with news articles.");

                    var existingAccount = db.SystemAccounts.FirstOrDefault(a => a.AccountId == account.AccountId);
                    if (existingAccount == null)
                        throw new Exception("Account not found.");

                    db.SystemAccounts.Remove(existingAccount);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error deleting account: " + e.Message);
            }
        }
    }
}