using System.Globalization;

namespace BusinessLogicLayer.Entity;
[Serializable]
public class Answers : List<string>, IFormattable
{
    public const int maxCapacity = 4;
    public const string testFormat = "T";
    public const string answerFormat = "A";
    public const string defaultFormat = "D";

    public int RightAnswer { get; set; } = -1;
    public int UserAnswer { get; set; } = -1;
    public override string ToString()
    {
        return ToString(testFormat, CultureInfo.CurrentCulture);
    }
    public string ToString(string? format)
    {
        return ToString(format, CultureInfo.CurrentCulture);
    }
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if(string.IsNullOrEmpty(format))
            format = defaultFormat;

        formatProvider ??= CultureInfo.CurrentCulture;

        string res = "";
        int index = 1;
        switch(format.ToUpperInvariant())
        {
            case answerFormat:
            foreach(string answer in this)
            {
                res += $"\t{index}" + $") {answer};";
                if(index - 1 == RightAnswer)
                    res += " +";
                index++;
            }
            return res;
            case testFormat:
            foreach(string answer in this)
            {
                res += $"\t{index}" + $") {answer};";
                if(index - 1 == UserAnswer)
                    res += " <--";

                index++;
            }
            return res;
            case defaultFormat:
            default:
            foreach(string answer in this)
            {
                res += $"\t{index}" + $") {answer};";
                index++;
            }
            return res;
        }
    }

}
