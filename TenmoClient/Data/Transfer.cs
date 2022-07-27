using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class Transfer
    {
        public int TransferId { get; set; }

        public int ToUserId { get; set; }

        public string FromUsername { get; set; }

        public string ToUsername { get; set; }

        public string TransferTypeDesc { get; set; }

        public double Amount { get; set; }

        public string TransferStatus { get; set; }
    }
}

