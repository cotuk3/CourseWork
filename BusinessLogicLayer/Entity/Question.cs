namespace BusinessLogicLayer.Entity;

[Serializable]
public class Question
{
    string value = "";
    Answers _answers = new Answers();

    #region Ctors
    public Question(string question, Answers answers)
    {
        value = question;
        _answers = answers;
    }
    public Question(string question)
    {
        value = question;
    }
    public Question()
    {

    }
    #endregion

    #region Properties
    public Answers Answers
    {
        get => _answers;
        set => _answers = value;
    }
    public string Value
    {
        get => value;
        set => this.value = value;
    }
    public string this[int index]
    {
        get => Answers[index];
        set => Answers[index] = value;
    }
    public int RightAnswer
    {
        get => _answers.RightAnswer;
        set => _answers.RightAnswer = value;
    }
    public int UserAnswer
    {
        get => _answers.UserAnswer;
        set => _answers.UserAnswer = value;
    }

    #endregion

    public void Add(string answer)
    {
        _answers.Add(answer);
    }

    public override string ToString()
    {
        string res = value;
        return res;
    }
    public override bool Equals(object? obj) => Value.Equals((obj as Question).Value);

    public override int GetHashCode() => Value.GetHashCode();



    public static implicit operator Question(string question)
    {
        return new Question(question);
    }
    public static implicit operator string(Question question)
    {
        return question.Value;
    }
}
