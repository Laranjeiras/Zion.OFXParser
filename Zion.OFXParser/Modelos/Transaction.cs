using System;

namespace Zion.OFXParser.Modelos
{
    public class Transaction
    {
        public string TrnType { get; set; }
        public TransactionType TransactionType => TrnType switch
        {
            "DEBIT" => TransactionType.DEBIT,
            "CREDIT" => TransactionType.CREDIT,
            _ => TransactionType.OTHERS,
        };

        public DateTime Date { get; set; }

        public double Amount { get; set; }

        public string Id { get; set; }

        public string Description { get; set; }

        public long Checksum { get; set; }
    }
}
