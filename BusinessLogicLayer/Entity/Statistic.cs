using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Entity;
[Serializable]
public class Statistic : List<string>
{
    public void Add(User user, Mark mark)
    {
        string res = user.ToString() + " : " + mark.ToString()+"\n";
        this.Add(res);
    }

    public void AddRange(IDictionary<User, Mark> dict)
    {
        foreach(var item in dict)
        {
            this.Add(item.Key, item.Value);
        }
    }

    public Statistic GetStatsByUser(User user)
    {
        Statistic userStats = new();
        string userString = user.ToString();

        foreach(string s in this)
        {
            if(s.StartsWith(userString))
            {
                userStats.Add(s);
            }
        }
        if(userStats.Count == 0)
            throw new Exception("Unknow user!");

        return userStats;
    }
    public Statistic GetStatsByDate(DateTime time)
    {
        Statistic dateStats = new();
        string dateString = $"{time:MM.dd.yyyy}";

        foreach(string s in this)
        {
            if(s.Contains(dateString))
            {
                dateStats.Add(s);
            }
        }
        return dateStats;
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
