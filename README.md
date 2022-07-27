# Online Payment Service - Overview

My software development bootcamp (Tech Elevator) tasked my classmate Mohammed Qaralos and myself with developing an application for an online payment service meant to transfer "TE bucks" between friends. 

We utilized all that we learned about Client-Server Programming in C# to build a RESTful API server with the ASP.NET Core framework. All user information (username, password, account balance, and much more) is stored in a SQL database which is accessed and manipulated by the server. On the client side, we created a command-line application to act as a user interface with which the user can interact with the server.

## Technologies Used

- C#

- ASP.NET

- SQL Server Management Studio

- Postman (for server testing)

## Application Features

- Users can Register with a username and password. Newly registered users start with an initial balance of 1,000 TE Bucks.

- Users can Login using their registered username and password. Logging in and subsequent interactions are authenticated using the user's JWT.

- Authenticated users can display their current balance.

- Authenticated users may initiate a transfer of a specific amount of TE Bucks to another registered user. Once user selects the Send TE Bucks option, a list of all other registered users is displayed to choose from. User specifies a recipient and an amount, and the program updates both balances (sender and recipient) in SQL. Protections are in place to ensure user selects a valid recipient, can send only as many TE Bucks as are in their account, etc.

 - Authenticated users may see a list of all transfers they have sent or received. Once transfer list is displayed, they may choose to display additional details of any such transfer via its Transfer ID.

## SQL Database Schema

![Database schema](./database_schema.png)

### Users table

The `users` table stores the login information for users of the system.

| Field           | Description                                                                    |
| --------------- | ------------------------------------------------------------------------------ |
| `user_id`       | Unique identifier of the user                                                  |
| `username`      | String that identifies the name of the user; used as part of the login process |
| `password_hash` | Hashed version of the user's password                                          |
| `salt`          | String that helps hash the password                                            |

### Accounts table

The `accounts` table stores the accounts of users in the system.

| Field           | Description                                                        |
| --------------- | ------------------------------------------------------------------ |
| `account_id`    | Unique identifier of the account                                   |
| `user_id`       | Foreign key to the `users` table; identifies user who owns account |
| `balance`       | The amount of TE bucks currently in the account                    |

### Transfer types table

The `transfer_types` table stores the types of transfers that are possible.

| Field                | Description                             |
| -------------------- | --------------------------------------- |
| `transfer_type_id`   | Unique identifier of the transfer type  |
| `transfer_type_desc` | String description of the transfer type |

There are two types of transfers:

| `transfer_type_id` | `transfer_type_desc` | Purpose                                                                |
| ------------------ | -------------------- | ---------------------------------------------------------------------- |
| 1                  | Request              | Identifies transfer where a user requests money from another user      |
| 2                  | Send                 | Identifies transfer where a user sends money to another user           |

### Transfer statuses table

The `transfer_statuses` table stores the statuses of transfers that are possible.

| Field                  | Description                               |
| ---------------------- | ----------------------------------------- |
| `transfer_status_id`   | Unique identifier of the transfer status  |
| `transfer_status_desc` | String description of the transfer status |

There are three statuses of transfers (this table is largely unutilized in our application):

| `transfer_status_id` | `transfer_status_desc` |Purpose                                                                                 |
| -------------------- | -------------------- | ---------------------------------------------------------------------------------------  |
| 1                    | Pending                | Identifies transfer that hasn't occurred yet and requires approval from the other user |
| 2                    | Approved               | Identifies transfer that has been approved and occurred                                |
| 3                    | Rejected               | Identifies transfer that wasn't approved                                               |

### Transfers table

The `transfer` table stores the transfers of TE bucks.

| Field                | Description                                                                                     |
| -------------------- | ----------------------------------------------------------------------------------------------- |
| `transfer_id`        | Unique identifier of the transfer                                                               |
| `transfer_type_id`   | Foreign key to the `transfer_types` table; identifies type of transfer                          |
| `transfer_status_id` | Foreign key to the `transfer_statuses` table; identifies status of transfer                     |
| `account_from`       | Foreign key to the `accounts` table; identifies the account that the funds are being taken from |
| `account_to`         | Foreign key to the `accounts` table; identifies the account that the funds are going to         |
| `amount`             | Amount of the transfer                                                                          |

## Setup

Open the database creation script `OnlinePaymentService.sql` in SQL Server Management Studio and execute it. That's it!

