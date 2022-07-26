﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }

        public int FromUserId { get; set; }

        public int ToUserId { get; set; }

        public string FromUsername { get; set; }

        public string ToUsername { get; set; }

        public string TransferTypeDesc { get; set; }

        public double Amount { get; set; }

        public string TransferStatus { get; set; }
    }
}
