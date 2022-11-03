using System.Globalization;

namespace BusinessLogicLayer.Entity;

[Serializable]
public class Test : IFormattable
{
    #region Questions
    private List<Question> _questions = new();

    public void Add(Question question)
    {
        _questions.Add(question);
    }
    public void RemoveAt(int index)
    {
        _questions.RemoveAt(index);
    }
    public void AddRange(IEnumerable<Question> questions)
    {
        _questions.AddRange(questions);
    }

    public int Count
    {
        get => Questions.Count;
    }
    public Question this[int index]
    {
        get => _questions[index];
        set => _questions[index] = value;
    }
    public List<Question> Questions
    {
        get => _questions;
        set => _questions = value;
    }
    #endregion

    #region Statistic
    private Statistic _statistic = new();
    public Statistic Statistic
    {
        get => _statistic;
        set => _statistic = value;
    }
    public void AddStatistic(User user, Mark mark)
    {
        _statistic.Add(user, mark);
    }
    public string GetLastStatistic()
    {
        string res = _statistic[_statistic.Count - 1];

        return res;
    }
    public void ClearStatistic()
    {
        _statistic.Clear();
    }
    public Statistic GetStatisticByUser(User user)
    {
        return Statistic.GetStatsByUser(user);
    }
    public Statistic GetStatisticByDate(DateTime time)
    {
        return Statistic.GetStatsByDate(time);
    }

    #endregion

    public override string ToString()
    {
        return ToString("D", CultureInfo.CurrentCulture);
    }
    public override bool Equals(object? obj)
    {
        Test? test = obj as Test;
        if(obj is not null)
        {
            if(Questions.Count == test.Questions.Count)
            {
                for(int i = 0; i < Questions.Count; i++)
                {
                    if(Questions[i] != test.Questions[i])
                        return false;
                }
                return true;
            }
        }

        return false;

    }
    public override int GetHashCode()
    {
        return Questions.GetHashCode();
    }

    public string ToString(string? format)
    {
        return ToString(format, CultureInfo.CurrentCulture);
    }
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if(string.IsNullOrEmpty(format))
            format = Answers.defaultFormat;

        formatProvider ??= CultureInfo.CurrentCulture;

        string res = "";
        switch(format.ToUpperInvariant())
        {
            case Answers.answerFormat:
            foreach(Question question in _questions)
            {
                res += question.ToString();
                res += "\n" + question.Answers.ToString(Answers.answerFormat) + "\n";
            }
            return res;
            case Answers.testFormat:
            foreach(Question question in _questions)
            {
                res += question.ToString();
                res += "\n" + question.Answers.ToString(Answers.testFormat) + "\n";
            }
            return res;
            case Answers.defaultFormat:
            default:
            foreach(Question question in _questions)
            {
                res += question.ToString();
                res += "\n" + question.Answers.ToString(Answers.defaultFormat) + "\n";
            }
            return res;
        }
    }
}
