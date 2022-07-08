using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Data;

namespace TenmoClient
{
    public class UsersService
    {

        private const string API_BASE_URL = "https://localhost:44315/";
        private readonly RestClient client = new RestClient();

        public UsersService()
        {
            this.client = new RestClient("https://localhost:44315/");
        }

        public double GetBalanceById() // remove console writelines someday
        {
            RestRequest request = new RestRequest("users");

            IRestResponse<API_User> response = client.Get<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Could not connect to the database; Try again later!");
               
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine("Problem getting balance: " + response.StatusDescription);
                Console.WriteLine(response.Content);        
            }
            return response.Data.Balance;
        }


        public bool SendMoney(Transfer transfer)
        {
            bool SuccessfulTransfer = false;

            RestRequest request = new RestRequest("users");
            request.AddJsonBody(transfer);

            IRestResponse<Transfer> response = client.Put<Transfer>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Could not connect to the database; Try again later!");

            }

            else if (!response.IsSuccessful)
            {
                Console.WriteLine("Problem executing transfer: " + response.StatusDescription);
                Console.WriteLine(response.Content);
            }
            else
            {
                SuccessfulTransfer = true;
            }

            return SuccessfulTransfer;
        }

        public List<API_User> DisplayRecipients()
        {

            RestRequest request = new RestRequest("users/recipients");

            IRestResponse<List<API_User>> response = client.Get<List<API_User>>(request);

            return response.Data;
        }

        public List<Transfer> DisplayTransfers()
        {
            RestRequest request = new RestRequest("users/transfers");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

            //if (response.ResponseStatus != ResponseStatus.Completed)
            //{
            //    Console.WriteLine("An error occurred communicating with the server.");
            //}
            //else if (!response.IsSuccessful)
            //{
            //    if (!string.IsNullOrWhiteSpace(response.Data.))
            //    {
            //        Console.WriteLine("An error message was received: " + response.Data.Message);
            //    }
            //    else
            //    {
            //        Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
            //    }
            //    return false;
            //}
            //else

                return response.Data;
        }

        public Transfer DisplayTransferByID(int id)
        {
            RestRequest request = new RestRequest("users/transfer/" + id);

            IRestResponse <Transfer> response = client.Get<Transfer>(request);

            return response.Data;
        }

        private string token;
        public void UpdateToken(string jwt)
        {
            token = jwt;
        
            if (jwt == null)
            {
                client.Authenticator = null;
            }
            else
            {
                client.Authenticator = new JwtAuthenticator(jwt);
            }
        }
    }
}