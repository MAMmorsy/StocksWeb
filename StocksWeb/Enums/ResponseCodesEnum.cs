namespace StocksWeb.Enums
{
    public enum ResponseCodesEnum
    {
        SuccessWithData = 1,
        SuccessWithoutData = 2,
        SaveAllRecords = 11,
        SaveSomeRecords = 12,
        SaveNoRecords = 13,
        InvalidParameters = 3,
        DbException = 4,
        ExistingData = 5
    }
}
