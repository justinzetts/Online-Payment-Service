using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int Transfer_Id { get; set; }

        public int From_User_Id { get; set; }

        public int To_User_Id { get; set; }

        public string From_Username { get; set; }

        public string To_Username { get; set; }

        public int Transfer_Type_Id { get; set; }

        public int Transfer_Status_Id { get; set; }

        public string Transfer_Type_Desc { get; set; }

        public int Account_From_Id { get; set; }

        public int Account_To_Id { get; set; }

        public double Amount { get; set; }
    }
}
