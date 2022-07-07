﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class Transfer
    {
        public int Transfer_Id { get; set; }

        public int From_User_Id { get; set; }

        public int Transfer_Type_Id { get; set; }

        public int Transfer_Status_Id { get; set; }

        public int Account_From_Id { get; set; }

        public int Account_To_Id { get; set; }

        public double Amount { get; set; }
    }
}
