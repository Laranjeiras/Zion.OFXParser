using System;

namespace Zion.OFXParser.Modelos
{
    public class HeaderExtract
    {
        public string Language { get; set; }

        public DateTime? ServerDate { get; set; }

        public string BankName { get; set; }

        public HeaderExtract()
        {

        }

        public HeaderExtract(string language, DateTime serverDate, string bankName)
        {
            this.Language = language;
            this.ServerDate = serverDate;
            this.BankName = bankName;
        }
    }
}
