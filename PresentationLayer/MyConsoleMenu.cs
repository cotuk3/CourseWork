using BusinessLogicLayer;
using BusinessLogicLayer.Entity.Stats;
using BusinessLogicLayer.Entity.Test;
using BusinessLogicLayer.Exceptions;
using ConsoleMenuBase;
using System.Collections;

namespace PresentationLayer;

public class MyConsoleMenu : ConsoleMenu
{
    #region Fields
    static readonly Dictionary<string, Action> mainMenu = new();
    static readonly Dictionary<string, Action> changeMenu = new();
    static Interaction inter = new();
    static readonly Exception unknownCommand = new("Unknown Command!");
    #endregion

    public MyConsoleMenu()
    {
        mainMenu.Add("/info", () => Info());                       // +
        mainMenu.Add("/create", () => Create(false));              // 
        mainMenu.Add("/create def", () => Create(true));           // 
        mainMenu.Add("/change", () => StartChanging());            // 
        mainMenu.Add("/pass", () => PassTest());                   // 
        mainMenu.Add("/stats", () => StartStats());                // 
        mainMenu.Add("/show", () => ShowTest());                   // +
        mainMenu.Add("/get all", () => ShowQuestions());           // +
        mainMenu.Add("/clear", () => ClearFile());                 // 
        mainMenu.Add("/cls", () => Console.Clear());               // +
        //mainMenu.Add("/end", () =>                                 // +
        //{ Console.WriteLine("Bye, have a good time!"); Console.Read(); });

        changeMenu.Add("/info", () => ChangeInfo());
        changeMenu.Add("/question", () => StartChangingQuestion());
        changeMenu.Add("/answers", () => StartChangingAnswers());
    }

    #region Start
    public override void Start()
    {
        string? input = "";
        do
        {
            Console.Clear();
            mainMenu["/info"]();
            try
            {
                Console.Write("Enter the command: ");
                input = Console.ReadLine();
                if(int.TryParse(input, out int number))
                    switch(number)
                    {
                        case 1:
                        input = "/info";
                        break;
                        case 2:
                        input = "/create";
                        break;
                        case 3:
                        input = "/change";
                        break;
                        case 4:
                        input = "/pass";
                        break;
                        case 5:
                        input = "/stats";
                        break;
                        case 6:
                        input = "/show";
                        break;
                        case 7:
                        input = "/get all";
                        break;
                        case 8:
                        input = "/clear";
                        break;
                        case 9:
                        input = "/cls";
                        break;
                        case 10:
                        input = "/end";
                        return;
                    }

                if(mainMenu.ContainsKey(input))
                    mainMenu[input]();
                else
                    throw unknownCommand;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        } while(true);
    }
    #endregion

    #region Info
    public override void Info()
    {
        string? info = "***** Course Work *****\n" +
            "1.info;\n" +
            "2.create;\n" +
            "3.change;\n" +
            "4.pass;\n" +
            "5.stats;\n" +
            "6.show test;\n" +
            "7.get all questions;\n" +
            "8.clear;\n" +
            "9.cls;\n" +
            "10.end.";
        Console.WriteLine(info);
    }
    private void ChangeInfo()
    {
        string? changeInfo = "***** Change *****\n" +
            $"file: {FilePath}\n" +
            "\n1.question;\n" +
            "2.answers;\n" +
            "3.select file path;\n" +
            "4.end";
        Console.WriteLine(changeInfo);
    }
    private void QuestionInfo()
    {
        Console.Clear();
        string? questionInfo =
            "***** Changing Question *****\n" +
            $"file: {FilePath}\n" +
            "\n1.add;\n" +
            "2.delete;\n" +
            "3.change;\n" +
            "4.return.";
        Console.WriteLine(questionInfo);
    }
    private void AnswerInfo(int questionIndex)
    {
        Console.Clear();
        var question = Test[questionIndex];
        string? answerInfo =
            "***** Changing Answers *****\n" +
            $"{{\nfile: {FilePath}\n" +
            $"question: {question}\n" +
            $"answers: \n{question.Answers:A}\n}}" +
            "\n1.add;\n" +
            "2.delete;\n" +
            "3.change;\n" +
            "4.set right answer;\n" +
            "5.select question;\n" +
            "6.end.";
        Console.WriteLine(answerInfo);
    }
    private void PassingInfo(User user)
    {
        string? passingInfo =
            "***** Passing the Test *****\n" +
            $"file: {FilePath}\n" +
            $"user: {user};\n" +
            "\n/next to go to next answer;\n" +
            "/prev to go to previous answer;\n" +
            "/end;\n";
        Console.WriteLine(passingInfo);
    }
    private void StatisticInfo()
    {
        string? res = "***** Statistic *****\n" +
            "1.show;\n" +
            "2.clear;\n" +
            "3.show by user;\n" +
            "4.show by date;\n" +
            "5.select file path;\n" +
            "6.return.";
        Console.WriteLine(res);
    }
    #endregion

    #region Show
    //filled == true if we have already asked about file path
    public override void Show(IEnumerable collection)
    {
        int index = 1;
        foreach(var item in collection)
        {
            Console.WriteLine($"{index++}.{item}");
        }
    }
    public void ShowTest(bool filled = false, string? format = "D")
    {
        if(!filled && !AskForFilePath(ref inter))
            return;

        Console.WriteLine(Test.ToString(format));
    }
    private void ShowQuestions(bool filled = false)
    {
        if(!filled && !AskForFilePath(ref inter))
            return;

        Show(Test.Questions);
    }
    private void ShowQuestion(Test test, int questionIndex)
    {
        var question = test[questionIndex];
        string res = question + "\n" + question.Answers.ToString("T");
        Console.WriteLine(res);
    }

    /// <summary>
    /// show def statistic from test, or specific stats by user/date
    /// </summary>
    /// <param name="filled"></param>
    /// <param name="stats">is not null when show stats by user/date</param>
    private void ShowStatistic(bool filled = false, Statistic? stats = null)
    {
        if(!filled && !AskForFilePath(ref inter))
            return;

        stats ??= Test.Statistic;

        Console.WriteLine(stats.ToString());
    }
    #endregion

    #region Create
    /// <summary>
    /// ask file path,
    /// then if def adds to file Interaction.DefList
    /// else calls AddQuestion
    /// </summary>
    /// <param name="def">defines whether to add def list or not</param>
    public void Create(bool def)
    {
        if(!AskForFilePath(ref inter))
            return;

        if(def)
        {
            inter.AddTest(Interaction.DefTest);
            ShowTest(true);
        }
        else
        {
            AddQuestion();
        }
    }
    #endregion

    #region Change
    /// <summary>
    /// asks file path, 
    /// then asks what to change,
    /// then calls specific methods for changing(question/answers)
    /// </summary>
    private void StartChanging()
    {
    askForFilePath:
        if(!AskForFilePath(ref inter))
            return;

        string? input = "";
        do
        {
            Console.Clear();
            changeMenu["/info"]();
            try
            {
                Console.Write("Enter what you want to change: ");
                input = Console.ReadLine();

                if(int.TryParse(input, out int number))
                    switch(number)
                    {
                        case 0:
                        input = "/info";
                        break;
                        case 1:
                        input = "/question";
                        break;
                        case 2:
                        input = "/answers";
                        break;
                        case 3:
                        goto askForFilePath;
                        case 4:
                        return;
                        case 5:
                        input = "/cls";
                        break;
                    }


                if(changeMenu.ContainsKey(input))
                    changeMenu[input]();
                else
                    throw unknownCommand;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        } while(true);
    }

    /// <summary>
    /// asks what to do,
    /// then calls specific methods(add, delete, change question)
    /// </summary>
    private void StartChangingQuestion()
    {
        do
        {
            string input = "";
            Console.Clear();
            QuestionInfo();

            Console.Write("Enter what you want to do: ");
            input = Console.ReadLine();

            if(input == "/return" || input == "4")
                return;

            if(!int.TryParse(input, out int number))
            {
                Console.WriteLine(unknownCommand.Message);
                Console.ReadKey();
                continue;
            }

            switch(number)
            {
                case 1:
                AddQuestion();
                break;
                case 2:
                DeleteQuestion();
                break;
                case 3:
                ChangeQuestion();
                break;
                default:
                Console.WriteLine(unknownCommand.Message);
                Console.ReadKey();
                continue;
            }
            Console.ReadKey();
        } while(true);
    }

    /// <summary>
    /// asks index of question,
    /// then asks what to do,
    /// then calls specific method (add, delete, change, setRight answer)
    /// </summary>
    private void StartChangingAnswers()
    {
    //ask question index while it is not valid
    askQuestionIndex:
        int questionIndex;
        do
        {
            Console.Clear();
            ShowQuestions(true);

            Console.Write("Enter index of question: ");
            string? input = Console.ReadLine();

            if(input == "/return" || input == "/end")
                return;

            var test = Test;

            if(!int.TryParse(input, out questionIndex))
            {
                Console.WriteLine(new QuestionException(input).Message);
                Console.ReadKey();
                continue;
            }

            --questionIndex; // because in list counting starts from 0 

            if(!Interaction.IsIndexValid(questionIndex, test.Questions))
            {
                Console.WriteLine(new QuestionException(questionIndex + 1).Message);
                Console.ReadKey();
                continue;
            }

            break;
        } while(true);

        do
        {
            AnswerInfo(questionIndex);

            int number;
            string input;

            Console.Write("Enter what you want to do: ");
            input = Console.ReadLine();
            if(input == "/select" || input == "5")
                goto askQuestionIndex;
            if(input == "/end" || input == "6")
                return;

            if(!int.TryParse(input, out number))
            {
                Console.WriteLine(unknownCommand.Message);
                Console.ReadKey();
                continue;
            }

            switch(number)
            {
                case 1:
                AddAnswer(questionIndex);
                break;
                case 2:
                DeleteAnswer(questionIndex);
                break;
                case 3:
                ChangeAnswer(questionIndex);
                break;
                case 4:
                SetRightAnswer(questionIndex);
                break;
                default:
                Console.WriteLine(unknownCommand.Message);
                Console.ReadKey();
                continue;
            }

        } while(true);
    }
    #endregion

    #region Changing Question
    /// <summary>
    /// asks for question while user do not enter /return
    /// </summary>
    private void AddQuestion()
    {
        //if we add first question in test do not show all questions
        if(inter.Count != -1)
            ShowQuestions(true);

        Question question;

        do
        {
            Console.Clear();
            Console.Write("Enter your question or /return to stop" +
                "\n(your question should star with upper case letter and end with ?(question mark))\n: ");
            string questionString = Console.ReadLine();

            if(questionString == "/return" || questionString == "/end")
                return;

            try
            {
                question = Interaction.CreateQuestion(questionString);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                continue;
            }

            inter.AddQuestion(question);

            int questionIndex = Test.Questions.IndexOf(question);

            AddAnswer(questionIndex);
            ShowTest(true);
        } while(true);
    }

    /// <summary>
    /// deletes questions while test has more than 0 or while user do not enter /return
    /// </summary>
    private void DeleteQuestion()
    {
        do
        {
            int questionIndex;
            string input;

            ShowQuestions(true);
            Console.Write("Enter index of question which you want to delete" +
                "\nor /return to stop: ");
            input = Console.ReadLine();

            if(input == "/return" || input == "/end")
                return;

            if(!int.TryParse(input, out questionIndex))
            {
                Console.WriteLine(new QuestionException(input).Message);
                Console.ReadKey();
                continue;
            }

            --questionIndex; // because in list counting starts from 0
            try
            {
                inter.DeleteQuestion(questionIndex);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                continue;
            }

            if(inter.Count > 0)
                continue;

            ShowQuestions(true);
        } while(true);
    }

    /// <summary>
    /// changes selected by user qustion
    /// </summary>
    private void ChangeQuestion()
    {
    // enter index of question while it is not valid
    askQuestionIndex:

        string input;
        int questionIndex;

        ShowQuestions(true);
        Console.Write("Enter index of question which you want to change: ");
        input = Console.ReadLine();
        if(input == "/return" || input == "/end")
            return;

        if(!int.TryParse(input, out questionIndex))
        {
            Console.WriteLine(new QuestionException(input).Message);
            Console.ReadKey();
            goto askQuestionIndex;
        }

        --questionIndex; // because in list counting starts from 0

        if(Interaction.IsIndexValid(questionIndex, Test.Questions))
        {
        // enter new question while it is not valid
        loop2:
            Console.Write("Enter new question: ");
            string question = Console.ReadLine();

            try
            {
                inter.ChangeQuestion(questionIndex, Interaction.CreateQuestion(question));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                goto loop2;
            }
        }
        else
        {
            Console.WriteLine(new QuestionException(questionIndex).Message);
            goto askQuestionIndex;
        }
        ShowQuestions(true);
    }
    #endregion

    #region Changing Answers
    /// <summary>
    /// enter answer while answers.Count less then Answer.maxCapacity or user does not enter return,
    /// then asks right answer
    /// </summary>
    /// <param name="questionIndex">Index of question to which answers are added</param>
    private void AddAnswer(int questionIndex)
    {

        do
        {
            var test = Test;
            var question = test[questionIndex];

            if(question.Answers.Count < Answers.maxCapacity)
            {
                Console.WriteLine(question + $"\n{question.Answers}");
                Console.Write("Enter your answer" +
                    "\nto stop enter /end" +
                    "\n: ");
                string input = Console.ReadLine();
                if(input == "/end")
                {
                    break;
                }
                else
                {
                    inter.AddAnswer(questionIndex, input);
                    continue;
                }
            }
            //enter index of right answer while it is not valid
            SetRightAnswer(questionIndex);
            break;
        } while(true);
    }

    /// <summary>
    /// deletes answers while question has more than 0 or while user does not enter /return,
    /// then asks for right answer
    /// </summary>
    /// <param name="questionIndex"></param>
    private void DeleteAnswer(int questionIndex)
    {
    //enter index of answer while it is not valid
    deleteAnswer:
        var answers = Test[questionIndex].Answers;
        int answerIndex = 0;

        //if user enter /return stop deleting answers and ask for right answer
        if(!AskAnswerIndex(ref answerIndex, answers))
            goto setRightAnswer;

        int count = answers.Count;
        try
        {
            inter.DeleteAnswer(questionIndex, answerIndex);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadKey();
            goto deleteAnswer;
        }

        if(--count > 0)
            goto deleteAnswer;

        setRightAnswer:
        //enter index of right answer while it is not valid
        SetRightAnswer(questionIndex);
    }

    /// <summary>
    /// changes answer selected by user,
    /// then asks for right answer
    /// </summary>
    /// <param name="questionIndex"></param>
    private void ChangeAnswer(int questionIndex)
    {
    //enter index of answer while it is not valid
    loop1:
        var answers = Test[questionIndex].Answers;
        int answerIndex = 0;

        if(!AskAnswerIndex(ref answerIndex, answers))
            goto set;

        if(!Interaction.IsIndexValid(answerIndex, answers))
        {
            Console.WriteLine(Interaction.wrongAnswer.Message);
            goto loop1;
        }

        Console.Write("Enter new answer: ");
        inter.ChangeAnswer(questionIndex, answerIndex, Console.ReadLine());

    set:
        SetRightAnswer(questionIndex);
    }

    /// <summary>
    /// while index is not valid ask for right answer(1 - answers.Count),
    /// if index valid sets right answer and returns
    /// </summary>
    /// <param name="questionIndex"></param>
    private void SetRightAnswer(int questionIndex)
    {
    loop:
        var question = Test[questionIndex];
        var answers = question.Answers;
        if(answers.Count <= 0)
            return;
        Console.Write($"Please choose right answer (1-{answers.Count})\n" +
                $"{question}\n" +
                $"{answers:A}\n" +
                $":");

        string? rightAnswer = Console.ReadLine();
        if(rightAnswer == "/return" || rightAnswer == "/end")
            return;
        if(!int.TryParse(rightAnswer, out int rightAnswerIndex))
            goto loop;
        --rightAnswerIndex;
        try
        {
            inter.SetRightAnswer(questionIndex, rightAnswerIndex);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            goto loop;
        }
    }

    /// <summary>
    /// while index is not valid ask for answer(1 - answers.Count)
    /// </summary>
    /// <param name="answerIndex">ref not out because if user enters /return or /end answerIndex is not set</param>
    /// <param name="answers"></param>
    /// <returns></returns>
    private static bool AskAnswerIndex(ref int answerIndex, IList answers)
    {
        string input;
        do
        {
            Console.Clear();
            Console.Write("Enter index of answer" +
                "\nto end enter /return" +
                $"\n{answers}" +
                "\n: ");

            input = Console.ReadLine();
            if(input == "/return" || input == "/end")
                return false;

        } while(!int.TryParse(input, out answerIndex));

        answerIndex--;
        return true;
    }
    #endregion

    #region Pass
    private static User? AskUserData()
    {
        User user;
        do
        {
            Console.Clear();
            Console.Write("Enter your first name: ");
            string? firstName = Console.ReadLine();

            if(firstName == "/return" || firstName == "/end")
                return null;

            Console.Write("Enter your last name: ");
            string? lastName = Console.ReadLine();
            if(lastName == "/return" || lastName == "/end")
                return null;

            try
            {
                user = Interaction.CreateUser(firstName, lastName);
                break;
            }
            catch(UserException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("First and Last name must start with upper case letter and contain only letters!");
                Console.ReadKey();
            }
        } while(true);
        return user;
    }
    private void PassTest()
    {
    //ask for file path while file does not contain test
    loop:
        if(!AskForFilePath(ref inter))
            return;
        Console.Clear();

        Test test;
        try
        {
            test = Test;
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.ReadKey();
            goto loop;
        }

        inter.Reset();

        string? rightAnswers = inter.CheckForRightAnswers();

        if(rightAnswers is not null)
        {
            Console.WriteLine(rightAnswers);
            Console.ReadKey();
            goto loop;
        }

        //ask user name and lastname
        User user = AskUserData();
        if(user is null)
            goto loop;

        Console.Clear();
        PassingInfo(user);
        int questionIndex = 0;
    loop2:
        while(true)
        {
            if(questionIndex == inter.Count)
            {
                Console.Clear();
                PassingInfo(user);
                questionIndex = 0;
            }

        //answer the questions till user enter /end 
        loop1:
            string input;

            ShowQuestion(test, questionIndex);

            Console.Write("Enter\n" + ":");
            input = Console.ReadLine();

            if(input == "/next")
            {
                if(questionIndex == inter.Count - 1)
                    questionIndex = 0;
                else
                    questionIndex++;
                goto loop2;
            }

            if(input == "/prev")
            {
                if(questionIndex == 0)
                    questionIndex = inter.Count - 1;
                else
                    questionIndex--;
                goto loop2;
            }

            if(input == "/end")
                break;

            //int answerIndex;
            if(!int.TryParse(input, out int answerIndex))
            {
                Console.WriteLine("Answer is not valid!");
                goto loop1;
            }

            if(answerIndex < 1 || answerIndex > 4)
            {
                Console.WriteLine("Answer is not valid!");
                goto loop1;
            }

            test[questionIndex].UserAnswer = --answerIndex;

            questionIndex++;
        }

        ClearFile(true);
        inter.AddTest(test);

        inter.GetPersentOfRightAnswers(test, DateTime.Now, user);
        test = Test;
        string res = test.GetLastStatistic();
        Console.WriteLine(res);
        ShowTest(true, "C");
        inter.Reset();
    } // TODO : refactor
    #endregion

    #region Stats
    private void StartStats()
    {
    askForFilePath:
        if(!AskForFilePath(ref inter))
            return;

        string? input = "";
        do
        {
            Console.Clear();
            StatisticInfo();
            try
            {

                Console.Write("Enter what you want to do: ");
                input = Console.ReadLine();

                if(int.TryParse(input, out int number))
                {
                    switch(number)
                    {
                        case 1:
                        ShowStatistic(true);
                        break;
                        case 2:
                        ClearStats();
                        break;
                        case 3:
                        GetStatsByUser();
                        break;
                        case 4:
                        GetStatsByDate();
                        break;
                        case 5:
                        goto askForFilePath;
                        case 6:
                        input = "/return";
                        return;
                        case 7:
                        mainMenu["/cls"]();
                        break;
                        default:
                        throw unknownCommand;
                    }
                }
                else
                {
                    throw unknownCommand;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        } while(input != "/return");

    }
    private void ClearStats()
    {
        var test = Test;
        test.ClearStatistic();

        ClearFile(true);
        inter.AddTest(test);
    }
    private void GetStatsByUser()
    {
        Statistic stats;
        while(true)
        {
            var user = AskUserData();
            if(user is null)
                return;
            try
            {
                stats = Test.GetStatisticByUser(user);
                break;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
        ShowStatistic(true, stats);
    }
    private void GetStatsByDate()
    {
        DateTime time;
        string? date;
        do
        {
            Console.Clear();
            Console.Write("Enter date\n" +
            "(date format (date.month.year)\n:");
            date = Console.ReadLine();

        } while(!DateTime.TryParse(date, out time));

        var stats = Test.GetStatisticByDate(time);
        ShowStatistic(true, stats);
    }
    #endregion

    #region Auxiliary Methods
    private bool AskForFilePath(ref Interaction inter)
    {
    loop:
        Console.Clear();
        Console.Write("Enter file path:");

        string? filePath = Console.ReadLine();
        if(filePath == "/return" || filePath == "/end")
            return false;
        try
        {
            inter.FilePath = filePath;
            FilePath = inter.FilePath;
        }
        catch
        {
            goto loop;
        }

        return true;
    }
    private void ClearFile(bool filled = false)
    {
        if(!filled && !AskForFilePath(ref inter))
                return;

        inter.ClearFile();
    }
    private string? FilePath { get; set; }
    private Test Test
    {
        get => inter.GetTest();
    }
    #endregion
}
