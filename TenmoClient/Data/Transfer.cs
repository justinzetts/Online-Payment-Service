using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class Transfer
    {
        public int TransferId { get; set; }

        public int FromUserId { get; set; }

        public int ToUserId { get; set; }

        public string FromUsername { get; set; }

        public string ToUsername { get; set; }

        public int TransferTypeId { get; set; }

        public int TransferStatusId { get; set; }

        public string TransferTypeDesc { get; set; }

        public int AccountFromId { get; set; }

        public int AccountToId { get; set; }

        public double Amount { get; set; }

        public string TransferStatus { get; set; }
    }
}

