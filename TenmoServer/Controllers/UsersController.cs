using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TenmoServer.Controllers
{
    [Route("Users")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly IUserDAO userDAO;

        public UsersController(IUserDAO userDAO)
        {
            this.userDAO = userDAO;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetBalance()
        {
            int id = LoggedInUserId;
            User user = userDAO.GetBalanceById(id);

            if (user == null)
            {
                return NotFound("Could not find user " + id);
            }
            return Ok(user);
        }

        [HttpPut]
        [Authorize]
        public ActionResult SendMoney(Transfer transfer)
        {
            transfer.FromUserId = LoggedInUserId;

            bool DoTransfer = userDAO.CheckTransferValidity(transfer);

            if (DoTransfer == true)
            {
                userDAO.TransferBucks(transfer);
                return Ok(transfer);
            }
            else
            {
                return BadRequest("You attempted to send more TE Bucks than you have in your account. Please try again.");
            }
        }

        [HttpGet("recipients")]
        [Authorize]

        public ActionResult ListRecipients()
        {
            int senderId = LoggedInUserId;
            List<User> recipients = userDAO.GetUsers(senderId);

            return Ok(recipients);
        }

        [HttpGet("transfers")]
        [Authorize]

        public ActionResult ListTransfers()
        {
            int userId = LoggedInUserId;
            List<Transfer> transfers = userDAO.GetTransfers(userId);

            return Ok(transfers);
        }

        [HttpGet("transfer/{id}")]
        [Authorize]

        public ActionResult GetTransferById(int id)
        {
            Transfer transfer = userDAO.GetTransferById(id);
            if (transfer != null)
            {
                if (LoggedInUserId != transfer.ToUserId && LoggedInUserId != transfer.FromUserId)
                {
                    return Forbid();
                }
            }
            return Ok(transfer); 
        }

        private int LoggedInUserId
        {
            get
            {
                Claim idClaim = User.FindFirst("sub");
                if (idClaim == null)
                {
                    // User is not logged in
                    return -1;
                }
                else
                {
                    // User is logged in. Their subject (sub) claim is their ID
                    return int.Parse(idClaim.Value);
                }
            }
        }


        
    }
}
