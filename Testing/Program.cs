using BusinessObjects;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            // Cấu hình chuỗi kết nối (thay bằng chuỗi kết nối thực tế của bạn)
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("E:\\SE_Ky7\\DependencyInjection_Learing\\TranNguyenHan_SE18B03_A01\\TranNguyenHanMVC\\appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            string connectionString = configuration.GetConnectionString("ConnectionStrings:MyStockDB");

            // Cấu hình DbContext
            var optionsBuilder = new DbContextOptionsBuilder<FunewsManagementContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using (var context = new FunewsManagementContext(optionsBuilder.Options))
            {
                Console.WriteLine("Testing SystemAccountDAO Methods...\n");

                // 1. Test GetAccounts
                Console.WriteLine("Get all accounts:");
                var accounts = SystemAccountDAO.GetAccounts();
                foreach (var account in accounts)
                {
                    Console.WriteLine($"ID: {account.AccountId}, Name: {account.AccountName}, Email: {account.AccountEmail}, Role: {account.AccountRole}, Password: {account.AccountPassword}");
                }
                Console.WriteLine();

                // 2. Test GetAccountById
                Console.WriteLine("Get account by ID (ID = 3):");
                var accountById = SystemAccountDAO.GetAccountById(3);
                if (accountById != null)
                {
                    Console.WriteLine($"ID: {accountById.AccountId}, Name: {accountById.AccountName}, Email: {accountById.AccountEmail}, Role: {accountById.AccountRole}, Password: {accountById.AccountPassword}");
                }
                else
                {
                    Console.WriteLine("Account not found.");
                }
                Console.WriteLine();

                // 3. Test GetAccountByEmail
                Console.WriteLine("Get account by Email (IsabellaDavid@FUNewsManagement.org):");
                var accountByEmail = SystemAccountDAO.GetAccountByEmail("IsabellaDavid@FUNewsManagement.org");
                if (accountByEmail != null)
                {
                    Console.WriteLine($"ID: {accountByEmail.AccountId}, Name: {accountByEmail.AccountName}, Email: {accountByEmail.AccountEmail}, Role: {accountByEmail.AccountRole}, Password: {accountByEmail.AccountPassword}");
                }
                else
                {
                    Console.WriteLine("Account not found.");
                }
                Console.WriteLine();

                // 5. Test SaveAccount (Thêm tài khoản mới)
                Console.WriteLine("Adding a new account:");
                var newAccount = new SystemAccount
                {
                    AccountName = "John Doe",
                    AccountEmail = "JohnDoe@FUNewsManagement.org",
                    AccountPassword = "@1",
                    AccountRole = 2
                };
                try
                {
                    SystemAccountDAO.SaveAccount(newAccount);
                    Console.WriteLine("New account added successfully. ID: " + newAccount.AccountId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error adding account: " + ex.Message);
                }
                Console.WriteLine();

                // 6. Test UpdateAccount (Cập nhật tài khoản ID = 3)
                Console.WriteLine("Updating account ID = 3:");
                var accountToUpdate = SystemAccountDAO.GetAccountById(3);
                if (accountToUpdate != null)
                {
                    accountToUpdate.AccountName = "Isabella Updated";
                    accountToUpdate.AccountPassword = "@newpassword";
                    try
                    {
                        SystemAccountDAO.UpdateAccount(accountToUpdate);
                        Console.WriteLine("Account updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error updating account: " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Account not found for update.");
                }
                Console.WriteLine();

                
                Console.WriteLine();

                // Kiểm tra lại danh sách sau khi thêm/xóa
                Console.WriteLine("Get all accounts after operations:");
                var updatedAccounts = SystemAccountDAO.GetAccounts();
                foreach (var account in updatedAccounts)
                {
                    Console.WriteLine($"ID: {account.AccountId}, Name: {account.AccountName}, Email: {account.AccountEmail}, Role: {account.AccountRole}, Password: {account.AccountPassword}");
                }
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}