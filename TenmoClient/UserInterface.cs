using System;
using System.Collections.Generic;
using System.Linq;
using TenmoClient.Data;

namespace TenmoClient
{
    public class UserInterface
    {
        private readonly UsersService usersService = new UsersService();
        private readonly ConsoleService consoleService = new ConsoleService();
        private readonly AuthService authService = new AuthService();

        private bool quitRequested = false;

        public void Start()
        {
            while (!quitRequested)
            {
                while (!authService.IsLoggedIn)
                {
                    ShowLogInMenu();
                }

                // If we got here, then the user is logged in. Go ahead and show the main menu
                ShowMainMenu();
            }
        }

        private void ShowLogInMenu()
        {
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.Write("Please choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int loginRegister))
            {
                Console.WriteLine("Invalid input. Please enter only a number.");
            }
            else if (loginRegister == 1)
            {
                HandleUserLogin();
            }
            else if (loginRegister == 2)
            {
                HandleUserRegister();
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }

        private void ShowMainMenu()
        {
            int menuSelection;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else
                {
                    switch (menuSelection)
                    {
                        case 1: // View Balance
                            double balance = usersService.GetBalanceById();
                            Console.WriteLine("Your current account balance is: $" + balance);
                            break;

                        case 2: // View Past Transfers
                            DisplayTransfers();
                            break;

                        case 3: // View Pending Requests
                            Console.WriteLine("NOT IMPLEMENTED!"); // TODO: Implement me
                            break;

                        case 4: // Send TE Bucks
                            DisplayUsers();
                            DoTransfer();
                            Console.WriteLine("Transfer Complete!");
                            break;

                        case 5: // Request TE Bucks
                            Console.WriteLine("NOT IMPLEMENTED!"); // TODO: Implement me
                            break;

                        case 6: // Log in as someone else

                            authService.ClearAuthenticator();

                            // NOTE: You will need to clear any stored JWTs in other API Clients
                            Console.WriteLine("NOT IMPLEMENTED!");

                            return; // Leaves the menu and should return as someone else

                        case 0: // Quit
                            Console.WriteLine("Goodbye!");
                            quitRequested = true;
                            return;

                        default:
                            Console.WriteLine("That doesn't seem like a valid choice.");
                            break;
                    }
                }
            } while (menuSelection != 0);
        }

        private void HandleUserRegister()
        {
            bool isRegistered = false;

            while (!isRegistered) //will keep looping until user is registered
            {
                LoginUser registerUser = consoleService.PromptForLogin();
                isRegistered = authService.Register(registerUser);
            }

            Console.WriteLine("");
            Console.WriteLine("Registration successful. You can now log in.");
        }

        private void HandleUserLogin()
        {
            while (!authService.IsLoggedIn) //will keep looping until user is logged in
            {
                LoginUser loginUser = consoleService.PromptForLogin();

                // Log the user in
                API_User authenticatedUser = authService.Login(loginUser);

                if (authenticatedUser != null)
                {
                    string jwt = authenticatedUser.Token;

                    // TODO: Do something with this JWT.
                    Console.WriteLine("Successfully logged in with JWT of " + jwt);
                    usersService.UpdateToken(jwt);
                }
            }
        }

        private void DisplayUsers()
        {
            Console.WriteLine("---------------------------------");
            Console.WriteLine("User ID              Name");
            Console.WriteLine("---------------------------------");
            List<API_User> recipients = new List<API_User>();
            recipients = usersService.DisplayRecipients();
            foreach(API_User recipient in recipients)
            {
                Console.WriteLine(recipient.UserId + "        " + recipient.Username);
            }
        }

        private void DoTransfer()
        {
            Transfer transfer = new Transfer();
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Please enter ID of user you are sending to (0 to cancel): ");
            transfer.ToUserId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Please enter amount to send (0 to cancel): ");
            transfer.Amount = Convert.ToDouble(Console.ReadLine());
            usersService.SendMoney(transfer);
        }

        private void DisplayTransfers()
        {
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Transfer ID     From/To       Amount    Transaction Type");
            Console.WriteLine("---------------------------------");
            List<Transfer> transfers = new List<Transfer>();
            transfers = usersService.DisplayTransfers();

            foreach (Transfer transfer in transfers)
            {
                Console.WriteLine(transfer.TransferId + "           " + transfer.FromUsername + "/" + transfer.ToUsername + "       " + transfer.Amount + "        " + transfer.TransferTypeDesc);
            }
            if (transfers == null)
            {
                Console.WriteLine("No transfers yet for this user.");
            }
            else if (transfers != null)
            {
                DisplayTransferByID(transfers);
            }

        }
        private void DisplayTransferByID(List<Transfer> transfers)
        {
            Transfer transfer = null;

            while (transfer == null)
            {
                Console.WriteLine("Please enter transfer ID to view details (0 to cancel): ");

                if (int.TryParse(Console.ReadLine(), out int transferID))
                {
                    transfer = usersService.DisplayTransferByID(transferID);
                    if (transfer == null)
                    {
                        Console.WriteLine("Invalid transfer ID (does not exist). Please try again");
                    }
                    else if (transfers.Any(t => t.TransferId == transfer.TransferId))
                    {
                        Console.WriteLine("---------------------------------");
                        Console.WriteLine("Transfer Details");
                        Console.WriteLine("---------------------------------");
                        Console.WriteLine("Id: " + transfer.TransferId);
                        Console.WriteLine("From: " + transfer.FromUsername);
                        Console.WriteLine("To: " + transfer.ToUsername);
                        Console.WriteLine("Type: " + transfer.TransferTypeDesc);
                        Console.WriteLine("Status: " + transfer.TransferStatus);
                        Console.WriteLine("Amount: " + transfer.Amount);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Only input a number.");
                }
            }
        }
    }
}
