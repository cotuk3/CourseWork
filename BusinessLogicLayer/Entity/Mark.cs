namespace BusinessLogicLayer.Entity;
[Serializable]
public class Mark
{
    public const string timeFormat = "dddd, MM.dd.yyyy, H:mm";
    public Mark(double value, DateTime time)
    {
        Value = value;
        Time = time;
    }
    public Mark()
    {

    }
    public double Value { get; set; }
    public DateTime Time { get; set; }
    public override string ToString()
    {
        return $"{Value:f2}/{100:f2} : {Time.ToString(timeFormat)};";
    }
}
