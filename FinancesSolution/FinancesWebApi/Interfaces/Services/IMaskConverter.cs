namespace FinancesWebApi.Interfaces.Services;

public interface IMaskConverter
{
    string[] SplitPhoneNumber(string strToSplit);
    string ConvertToString(List<string> masks);
    bool NumberIsValid(string masks, string number);
    bool CompareMaskNumber(string mask, string number);
}