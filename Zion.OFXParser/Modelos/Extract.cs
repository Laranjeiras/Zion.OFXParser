using System;
using System.Collections.Generic;

namespace Zion.OFXParser.Modelos
{
    public class Extract
    {
        public HeaderExtract Header { get; set; }

        public BankAccount BankAccount { get; set; }

        public string Status { get; set; }

        public DateTime InitialDate { get; set; }

        public DateTime FinalDate { get; set; }

        public IList<Transaction> Transactions { get; private set; } = new List<Transaction>();

        public IList<string> ImportingErrors { get; private set; } = new List<string>();

        public Extract(HeaderExtract header, BankAccount bankAccount,
            string status, DateTime initialDate, DateTime finalDate) : this(header, bankAccount, status)
        {
            this.InitialDate = initialDate;
            this.FinalDate = finalDate;
        }

        public Extract(HeaderExtract header, BankAccount bankAccount, string status)
        {
            this.Header = header;
            this.BankAccount = bankAccount;
            this.Status = status;
        }

        public void AddTransaction(Transaction transaction)
        {
            this.Transactions.Add(transaction);
        }
    }
}
