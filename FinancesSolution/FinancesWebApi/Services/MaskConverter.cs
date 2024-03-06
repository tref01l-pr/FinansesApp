using FinancesWebApi.Interfaces.Services;

namespace FinancesWebApi.Services;

public class MaskConverter : IMaskConverter
{
    public string[] SplitPhoneNumber(string strToSplit) => strToSplit.Split('/');
    public string ConvertToString(List<string> masks)
    {
        if (masks.Count == 0)
            throw new Exception("Error in ConvertToString in MaskConverter");
        
        string result = masks[0];

        if (masks.Count <= 1) 
            return result;
        
        
        for (int i = 1; i < masks.Count; i++)
        {
            result += "/" + masks[i];
        }

        return result;
    }

    public bool NumberIsValid(string masks, string number) => SplitPhoneNumber(masks).Any(mask => CompareMaskNumber(mask, number));

    public bool CompareMaskNumber(string mask, string number)
    {
        var maskToCompare = (from value in mask let valueAsString = value.ToString() where value == '#' || int.TryParse(valueAsString, out _) select value).ToList();

        if (maskToCompare.Count != number.Length)
            return false;

        return !maskToCompare.Where((t, i) => t != '#' && t != number[i]).Any();
    }
}