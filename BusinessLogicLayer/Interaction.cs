using BusinessLogicLayer.Entity;
using BusinessLogicLayer.Exceptions;
using DataAccess;
using System.Collections;
using System.Text.RegularExpressions;

namespace BusinessLogicLayer;
public class Interaction
{
    #region Fields 
    string? _filePath;
    string? _fileExtension;

    public readonly ArgumentException wrongFile = new ArgumentException("Unknow file extension!");
    public readonly IndexOutOfRangeException wrongIndex = new IndexOutOfRangeException("Index out of range!");

    static readonly Dictionary<string, Func<string, object>> deser = new()
    {
        { ".xml", (filePath) => new XMLProvider(typeof(Test), new Type[]
        {
            typeof(Question), typeof(Answers), typeof(Statistic), typeof(User), typeof(Mark)
        }).Deserialize(filePath) },

        { ".dat", (filePath) => new BinaryProvider(typeof(Test)).Deserialize(filePath) },
        { ".json", (filePath) => new JSONProvider(typeof(Test)).Deserialize(filePath) }
    };

    static Dictionary<string, Action<object, string>> ser = new Dictionary<string, Action<object, string>>()
    {
        {".xml", (graph, filePath) => new XMLProvider(typeof(Test), new Type[]
        {
            typeof(Question), typeof(Answers), typeof(Statistic),typeof(User), typeof(Mark)
        }).Serialize(graph, filePath) },
        {".dat", (graph, filePath) => new BinaryProvider(typeof(Test)).Serialize(graph, filePath) },
        {".json", (graph, filePath) => new JSONProvider(typeof(Test)).Serialize(graph, filePath) }
    };

    static readonly Regex validQuestion = new(@"^[A-Z a-z1-9,+-:*\|/A-Z]+\?$");
    #endregion

    #region Ctors
    public Interaction()
    {

    }
    public Interaction(string filePath)
    {
        FilePath = filePath;
    }
    #endregion

    #region Auxiliary Methods
    public string FilePath
    {
        get => _filePath;
        set
        {
            string extension = Path.GetExtension(value);
            if(extension == ".xml" || extension == ".dat" || extension == ".json")
            {
                _filePath = value;
                _fileExtension = Path.GetExtension(_filePath);
            }

            else
                throw wrongFile;
        }
    }
    public Test DefTest
    {
        get
        {
            Test test = new Test();
            test.Add(new Question("What is 2+2?") { Answers = { "1", "2", "4", "5" }, RightAnswer = 2 });
            test.Add(new Question("What is capital of Ukraine?")
            { Answers = { "Kyiv", "Odesa", "London", "New-York" }, RightAnswer = 0 });
            test.Add(new Question("What is the second letter of alphabet?")
            { Answers = { "A", "B", "C", "D" }, RightAnswer = 1 });
            return test;
        }
    }
    public bool IsQuestionValid(string question)
    {
        return validQuestion.IsMatch(question);
    }
    public bool IsIndexValid(int index, IList list)
    {
        return index >= 0 && index < list.Count;
    }
    public void ClearFile() => DataProvider.ClearFile(_filePath);
    public int Count
    {
        get
        {
            try
            {
                return GetTest().Count;
            }
            catch
            {
                return -1;
            }
        }
    }
    #endregion

    #region Test
    public void AddTest(Test test)
    {
        if(File.Exists(_filePath))
        {
            Test? res;
            try
            {
                res = deser[_fileExtension](_filePath) as Test;
            }
            catch
            {
                res = new Test();
            }
            if(res is null)
                res = new Test();
            foreach(Question question in res.Questions)
                test.Add(question);
        }
        ser[_fileExtension](test, _filePath);
    }
    public Test GetTest()
    {
        try
        {
            var test = deser[_fileExtension](_filePath) as Test;
            return test;
        }
        catch(FileNotFoundException)
        {
            throw new FileNotFoundException("File not found!");
        }
        catch(InvalidOperationException)
        {
            throw new InvalidOperationException("File contains another data!");
        }
    }
    #endregion

    #region Question
    public void AddQuestion(Question question)
    {
        Test? test;
        if(File.Exists(_filePath))
        {
            try
            {
                test = deser[_fileExtension](_filePath) as Test;
            }
            catch
            {
                test = new Test();
            }
        }
        else
            test = new Test();

        test.Add(question);
        ser[_fileExtension](test, _filePath);
    }
    public void DeleteQuestion(int index)
    {

        if(IsIndexValid(index, GetTest().Questions))
        {
            var test = GetTest();
            if(test is not null)
            {
                test.RemoveAt(index);
                ClearFile();
                AddTest(test);
            }
            else
                throw wrongFile;
        }
        else
            throw wrongIndex;

    }
    public void ChangeQuestion(int index, Question newQuestion)
    {
        try
        {
            if(IsIndexValid(index, GetTest().Questions) && IsQuestionValid(newQuestion))
            {
                var test = GetTest();
                test[index].Value = newQuestion;
                ClearFile();
                AddTest(test);
            }
            else if(!IsIndexValid(index, GetTest().Questions))
                throw wrongIndex;
            else if(!IsQuestionValid(newQuestion))
                throw new QuestionException();
        }
        catch
        {
            throw;
        }
    }
    public Question CreateQuestion(string question)
    {
        if(IsQuestionValid(question))
            return new Question(question);

        throw new QuestionException();
    }
    #endregion

    #region Answers
    public void AddAnswer(int questionIndex, string answer)
    {
        if(IsIndexValid(questionIndex, GetTest().Questions))
        {
            var test = GetTest();
            Question question = test[questionIndex];
            try
            {
                AddAnswer(ref question, answer);
            }
            catch
            {
                throw;
            }
            test[questionIndex] = question;
            ClearFile();
            AddTest(test);
        }
        else
            throw wrongIndex;
    }
    public void DeleteAnswer(int questionIndex, int answerIndex)
    {
        if(IsIndexValid(questionIndex, GetTest().Questions))
        {
            var test = GetTest();
            Question question = test[questionIndex];
            try
            {
                DeleteAnswer(ref question, answerIndex);
            }
            catch
            {
                throw;
            }
            test[questionIndex] = question;
            ClearFile();
            AddTest(test);
        }
        else
            throw wrongIndex;
    }
    public void SetRightAnswer(int questionIndex, int answerIndex)
    {
        if(IsIndexValid(questionIndex, GetTest().Questions))
        {
            var test = GetTest();
            Question question = test[questionIndex];
            try
            {
                SetRightAnswer(ref question, answerIndex);
            }
            catch
            {
                throw;
            }
            test[questionIndex] = question;
            ClearFile();
            AddTest(test);
        }
        else
            throw wrongIndex;
    }
    public void ChangeAnswer(int questionIndex, int answerIndex, string answer)
    {
        if(IsIndexValid(questionIndex, GetTest().Questions))
        {
            var test = GetTest();
            Question question = test[questionIndex];
            try
            {
                ChangeAnswer(ref question, answerIndex, answer);
            }
            catch
            {
                throw;
            }
            test[questionIndex] = question;
            ClearFile();
            AddTest(test);
        }
        else
            throw wrongIndex;
    }

    void AddAnswer(ref Question question, string answer)
    {
        if(question.Answers.Count < Answers.maxCapacity)
        {
            question.Add(answer);
        }
        else
            throw new AnswerException();
    }
    void DeleteAnswer(ref Question question, int index)
    {
        if(IsIndexValid(index, question.Answers))
        {
            question.Answers.RemoveAt(index);
        }
        else
            throw wrongIndex;

    }
    void SetRightAnswer(ref Question question, int index)
    {
        if(IsIndexValid(index, question.Answers))
        {
            question.Answers.RightAnswer = index;
        }
        else
            throw wrongIndex;
    }
    void ChangeAnswer(ref Question question, int index, string answer)
    {
        if(IsIndexValid(index, question.Answers))
        {
            question.Answers[index] = answer;
        }
    }
    #endregion

    public User CreateUser(string firstName, string lastName)
    {
        Regex validName = new(@"^[A-Z]{1}[a-z]+$");
        if(!validName.IsMatch(firstName) && !validName.IsMatch(lastName))
            throw new UserException(nameof(firstName), nameof(lastName));

        if(!validName.IsMatch(firstName))
            throw new UserException(nameof(firstName));

        if(!validName.IsMatch(lastName))
            throw new UserException(nameof(lastName));

        return new User(firstName, lastName);
    }

    public double GetPersentOfRightAnswers(Test test, DateTime time, User user)
    {
        int count = 0;
        foreach(Question question in test.Questions)
        {
            if(question.RightAnswer == question.UserAnswer)
            {
                count++;
            }
            question.UserAnswer = -1;
        }
        double value = (double)((double)count / test.Count) * 100.0;
        Mark mark = new(value, time);
        test.AddStatistic(user, mark);
        ClearFile();
        AddTest(test);

        return value;
    }
}
