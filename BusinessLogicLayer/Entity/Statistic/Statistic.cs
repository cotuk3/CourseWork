namespace BusinessLogicLayer.Entity.Stats;
[Serializable]
public class Statistic : List<string>
{
    public void Add(User user, Mark mark)
    {
        string res = user.ToString() + " : " + mark.ToString() + "\n";
        Add(res);
    }
    public void AddRange(IDictionary<User, Mark> dict)
    {
        foreach(var item in dict)
        {
            Add(item.Key, item.Value);
        }
    }

    public Statistic GetStatsByUser(User user)
    {
        string userString = user.ToString();

        Statistic userStats = (Statistic)(from s in this
                                          where s.StartsWith(userString)
                                          select s).ToList();

        //if we didnt find any entry throw exception
        return userStats.Count != 0 ? userStats : throw new Exception("Unknow user!");
    }
    public Statistic GetStatsByDate(DateTime time)
    {
        string dateString = $"{time:dd.MM.yyyy}";

        Statistic dateStats = (Statistic)(from s in this
                                          where s.Contains(dateString)
                                          select s).ToList();

        //if we didnt find any entry throw exception
        return dateStats.Count != 0 ? dateStats : throw new Exception("Unknow user!");
    }

    public override string ToString()
    {
        string res = "";
        foreach(var s in this)
        {
            res += s;
        }
        return res;
    }
}
