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
        private readonly RestClient client = new RestClient(); // API that communicates to Server's Controller

        public UsersService()
        {
            this.client = new RestClient("https://localhost:44315/");
        }

        public double  GetBalanceById()
        {
            RestRequest request = new RestRequest("users");

            IRestResponse<API_User> response = client.Get<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Could not connect to the dad-a-base; Try again later!");

               
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine("Problem getting joke: " + response.StatusDescription);
                Console.WriteLine(response.Content);

                
            }

            return response.Data.Balance;

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