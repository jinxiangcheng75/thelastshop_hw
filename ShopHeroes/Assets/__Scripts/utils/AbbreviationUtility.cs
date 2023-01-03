

using System;
using System.Collections.Generic;
using System.Linq;

public static class AbbreviationUtility
{
    private static SortedDictionary<long, string> ENrevations = new SortedDictionary<long, string>
    {
        {1000,"K"},
        {1000000, "M" },
        {1000000000, "B" },
        {1000000000000, "T" },
        {1000000000000000, "Q" }
    };

    private static SortedDictionary<long, string> CNrevations = new SortedDictionary<long, string>
    {
        {10000,"万"},
        {100000000, "亿" },
        {1000000000000, "兆" },
    };



    public static string AbbreviateNumber(double number, int decimalPlaces = 3)
    {
        if (number < 10000) //小于一万直接返还
        {
            return number.ToString("N0");
        }

        SortedDictionary<long, string> revations = LanguageManager.inst.curType == LanguageType.ENGLISH ? ENrevations : CNrevations;

        for (int i = revations.Count - 1; i >= 0; i--)
        {
            KeyValuePair<long, string> kvp = revations.ElementAt(i);
            if (Math.Abs(number) >= kvp.Key)
            {
                return RoundDown(number / kvp.Key, decimalPlaces).ToString() + LanguageManager.inst.GetValueByKey(kvp.Value);
            }
        }
        return RoundDown(number, 0).ToString();
    }

    public static double RoundDown(double number, int decimalPlaces)
    {
        return Math.Floor(number * Math.Pow(10, decimalPlaces)) / Math.Pow(10, decimalPlaces);
    }
}
