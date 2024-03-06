namespace FinancesWebApi.Interfaces.Services;

public interface IMaskConverter
{
    public string[] SplitPhoneNumber(string strToSplit);
    public string ConvertToString(List<string> masks);
    public bool NumberIsValid(string masks, string number);
    public bool CompareMaskNumber(string mask, string number);
}