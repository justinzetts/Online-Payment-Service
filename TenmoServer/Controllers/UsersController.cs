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
    // [Authorize] ???
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
            transfer.Account_From_Id = LoggedInUserId;

            bool transferSuccessful = userDAO.TransferBucks(transfer);

            if (transferSuccessful == false)
            {
                return NotFound("Not successful"); // add a more robust message here later
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
