using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IUserDAO
    {
        User GetUser(string username);
        User AddUser(string username, string password);
        List<User> GetUsers(int senderId);
        User GetBalanceById(int id);
        bool CheckTransferValidity(Transfer transfer);
        bool TransferBucks(Transfer transfer);
        List<Transfer> GetTransfers(int id);
        Transfer GetTransferById(int id);
    }
}
