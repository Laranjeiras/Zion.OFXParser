namespace Zion.OFXParser.Modelos
{
    public class BankAccount
    {
        public string AccType { get; set; }
        public AccountType Type => AccType switch 
        { 
            "CHECKING" => AccountType.CHECKING,
            "SAVING" => AccountType.SAVING,
            _ => AccountType.OTHERS,
        };

        public string AgencyCode { get; set; }

        public Bank Bank { get; set; }

        public string AccountCode { get; set; }
    }
}
