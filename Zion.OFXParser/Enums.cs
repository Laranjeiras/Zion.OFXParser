namespace Zion.OFXParser
{
    public enum AccountType
    {
        OTHERS = 0,
        CHECKING = 1,
        SAVING = 2
    }

    public enum TransactionType 
    { 
        OTHERS = -1,
        DEBIT = 0,
        CREDIT = 1
    }

    public enum PartDateTime
    {
        DAY,
        MONTH,
        YEAR,
        HOUR,
        MINUTE,
        SECOND
    }
}
