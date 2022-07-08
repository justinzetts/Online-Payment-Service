using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.DAO
{
    public class UserSqlDAO : IUserDAO
    {
        private readonly string connectionString;
        const decimal startingBalance = 1000;

        public UserSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public User GetUser(string username)
        {
            User returnUser = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT user_id, username, password_hash, salt FROM users WHERE username = @username", conn);
                cmd.Parameters.AddWithValue("@username", username);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows && reader.Read())
                {
                    returnUser = GetUserFromReader(reader);
                }
            }
            return returnUser;
        }

        public List<User> GetUsers(int senderId)
        {
            List<User> recipients = new List<User>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT u.user_id, u.username FROM users u WHERE u.user_id <> @senderID", conn);
                cmd.Parameters.AddWithValue("@senderID", senderId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        User u = GetRecipientsFromReader(reader);
                        recipients.Add(u);
                    }
                }
            }
            return recipients;
        }

        public User AddUser(string username, string password)
        {
            IPasswordHasher passwordHasher = new PasswordHasher();
            PasswordHash hash = passwordHasher.ComputeHash(password);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO users (username, password_hash, salt) VALUES (@username, @password_hash, @salt)", conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password_hash", hash.Password);
                cmd.Parameters.AddWithValue("@salt", hash.Salt);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("SELECT @@IDENTITY", conn);
                int userId = Convert.ToInt32(cmd.ExecuteScalar());

                cmd = new SqlCommand("INSERT INTO accounts (user_id, balance) VALUES (@userid, @startBalance)", conn);
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@startBalance", startingBalance);
                cmd.ExecuteNonQuery();
            }

            return GetUser(username);
        }


        public User GetBalanceById(int id)
        {
            const string sql = "SELECT u.username, u.user_id, (SELECT a.balance FROM accounts a WHERE u.user_id = a.user_id) AS 'Balance' "
            + "FROM Users u WHERE u.user_id = @id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return GetUserAndBalanceFromReader(reader);
                }
            }
            return null;
        }

        public bool CheckTransferValidity(Transfer transfer)
        {
            bool DoTransfer = true;

            const string sqlCheckValidity = "select a.balance from accounts a where a.user_id = @fromuserID";

            using (SqlConnection connCheck = new SqlConnection(connectionString))
            {
                connCheck.Open();

                SqlCommand commandCheck = new SqlCommand(sqlCheckValidity, connCheck);
                commandCheck.Parameters.AddWithValue("@fromuserID", transfer.FromUserId);

                double balance = Convert.ToDouble(commandCheck.ExecuteScalar());

                if (balance < transfer.Amount)
                {
                    DoTransfer = false;
                }
            }
            return DoTransfer;
        }

        public bool TransferBucks(Transfer transfer)
        {
            // TO DO - do checking to make sure balance is available, to make sure to account is valid, etc

            const string sqlIfValid = "INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                            "VALUES('1001', '2001', (SELECT a.account_id FROM accounts a WHERE a.user_id = @fromuserID), " +
                            "(SELECT a.account_id FROM accounts a WHERE a.user_id = @touserID), @amount); " +
                            "UPDATE accounts SET balance = balance - @amount WHERE user_id = @fromuserID; " +
                            "UPDATE accounts SET balance = balance + @amount WHERE user_id = @touserID; ";

            using (SqlConnection connIfValid = new SqlConnection(connectionString))
            {
                connIfValid.Open();

                SqlCommand commandIfValid = new SqlCommand(sqlIfValid, connIfValid);
                commandIfValid.Parameters.AddWithValue("@fromuserID", transfer.FromUserId);
                commandIfValid.Parameters.AddWithValue("@touserID", transfer.ToUserId);
                commandIfValid.Parameters.AddWithValue("@amount", transfer.Amount);

                int count = commandIfValid.ExecuteNonQuery();
            }
            return true;
        }

        public List<Transfer> GetTransfers(int id)
        {
            List<Transfer> transfers = new List<Transfer>();

            const string sql = "select t.transfer_id, " +
                                "(SELECT u.username FROM users u WHERE u.user_id = (SELECT a.user_id FROM accounts a WHERE account_id = t.account_from)) AS 'from_username', " +
                                "(SELECT u.username FROM users u WHERE u.user_id = (SELECT a.user_id FROM accounts a WHERE account_id = t.account_to)) AS 'to_username', " +
                                "t.amount, " +
                                "(SELECT tt.transfer_type_desc FROM transfer_types tt WHERE tt.transfer_type_id = t.transfer_type_id)  AS 'transfer_type_desc' " +
                                "from transfers t where t.account_from = (select a.account_id from accounts a where a.user_id = @id); ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Transfer t = GetTransfersFromReader(reader);
                        transfers.Add(t);
                    }
                }
            }
            return transfers;
        }

        public Transfer GetTransferById(int id)
        {
            const string sql = "select t.transfer_id, (SELECT u.username FROM users u WHERE u.user_id = " +
                               "(SELECT a.user_id FROM accounts a WHERE account_id = t.account_from)) AS 'from_username', " +
                               "(SELECT a.user_id FROM accounts a WHERE a.account_id = t.account_from) AS 'from_user_id', " +
                               "(SELECT a.user_id FROM accounts a WHERE a.account_id = t.account_to) AS 'to_user_id'," +
                               "(SELECT u.username FROM users u WHERE u.user_id = (SELECT a.user_id FROM accounts a WHERE account_id = t.account_to)) " +
                               "AS 'to_username', t.amount, (SELECT tt.transfer_type_desc FROM transfer_types tt WHERE tt.transfer_type_id = t.transfer_type_id) " +
                               "AS 'transfer_type_desc', (SELECT ts.transfer_status_desc FROM transfer_statuses ts WHERE ts.transfer_status_id = t.transfer_status_id) " +
                               "AS 'transfer_status_desc' from transfers t where t.transfer_id = @id; ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return GetTransferByIdFromReader(reader);
                }
            }
            return null;
        }

        private Transfer GetTransfersFromReader(SqlDataReader reader)
        {
            return new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),

                TransferTypeDesc = Convert.ToString(reader["transfer_type_desc"]),

                FromUsername = Convert.ToString(reader["from_username"]),

                ToUsername = Convert.ToString(reader["to_username"]),

                Amount = Convert.ToDouble(reader["amount"])
            };
        }

        private Transfer GetTransferByIdFromReader(SqlDataReader reader)
        {
            return new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),

                TransferTypeDesc = Convert.ToString(reader["transfer_type_desc"]),

                TransferStatus = Convert.ToString(reader["transfer_status_desc"]),

                FromUsername = Convert.ToString(reader["from_username"]),

                ToUsername = Convert.ToString(reader["to_username"]),

                ToUserId = Convert.ToInt32(reader["to_user_id"]), 

                FromUserId = Convert.ToInt32(reader["from_user_id"]), 

                Amount = Convert.ToDouble(reader["amount"])
            };
        }

        private User GetUserFromReader(SqlDataReader reader)
        {
            return new User()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Username = Convert.ToString(reader["username"]),
                PasswordHash = Convert.ToString(reader["password_hash"]),
                Salt = Convert.ToString(reader["salt"]),

            };
        }

        private User GetRecipientsFromReader(SqlDataReader reader)
        {
            return new User()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Username = Convert.ToString(reader["username"]),
            };
        }

        private User GetUserAndBalanceFromReader(SqlDataReader reader)
        {
            return new User()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Username = Convert.ToString(reader["username"]),
                Balance = Convert.ToDouble(reader["Balance"])
            };
        }
    }
}
