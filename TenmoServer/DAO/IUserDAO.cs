using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IUserDAO
    {
        User GetUser(string username);
        User AddUser(string username, string password);
        List<User> GetUsers();
        User GetBalanceById(int id);
        bool TransferBucks(Transfer transfer);
        // List<Transfer> GetTransfers(int id);
    }
}
