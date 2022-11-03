using BusinessLogicLayer;
using BusinessLogicLayer.Entity;
using BusinessLogicLayer.Exceptions;
using ConsoleMenuBase;
using System.Collections;
using System.Runtime.Serialization;

namespace PresentationLayer;

public class MyConsoleMenu : ConsoleMenu
{
    static Dictionary<string, Action> mainMenu = new Dictionary<string, Action>();
    static Dictionary<string, Action> changeMenu = new Dictionary<string, Action>();
    static Interaction inter = new();

    public MyConsoleMenu()
    {
        mainMenu.Add("/info", () => Info());
        mainMenu.Add("/create", () => Create(false));
        mainMenu.Add("/create def", () => Create(true));
        mainMenu.Add("/change", () => StartChanging());
        mainMenu.Add("/pass", () => PassTest());
        mainMenu.Add("/stats", () => StartStats());
        mainMenu.Add("/show", () => ShowTest());
        mainMenu.Add("/get all", () => ShowQuestions());
        mainMenu.Add("/clear", () => ClearFile());
        mainMenu.Add("/cls", () => { Console.Clear(); Info(); });
        mainMenu.Add("/end", () => { Console.WriteLine("Bye, have a good time!"); Console.Read(); });

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
                int number;
                if(int.TryParse(input, out number))
                {
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
                        break;

                    }
                }
                if(mainMenu.ContainsKey(input))
                    mainMenu[input]();
                else
                {
                    Console.WriteLine("Unknow Command");
                }
            }
            catch(SerializationException)
            {
                Console.WriteLine("File is empty or not filled with test!");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        } while(input != "/end");
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
        //Console.ReadKey();
    }
    private void ChangeInfo()
    {
        string? changeInfo = "***** Change *****\n" +
            $"file: {FilePath}\n" +
            "1.question;\n" +
            "2.answers;\n" +
            "3.select file path;\n" +
            "4.end";
        Console.WriteLine(changeInfo);
        //Console.ReadKey();
    }
    private void QuestionInfo()
    {
        Console.Clear();
        string? questionInfo =
            "***** Changing Question *****\n" +
            $"file: {FilePath}\n" +
            "1.add;\n" +
            "2.delete;\n" +
            "3.change;\n" +
            "4.return.";
        Console.WriteLine(questionInfo);
        //Console.ReadKey();
    }
    private void AnswerInfo(int questionIndex)
    {
        Console.Clear();
        var question = Test[questionIndex];
        string? answerInfo =
        "***** Changing Answers *****\n" +
        $"{{\nfile: {FilePath}\n" +
        $"question: {question}\n" +
        $"answers: \n{question.Answers}}}\n" +
        "\n1.add;\n" +
        "2.delete;\n" +
        "3.change;\n" +
        "4.set right answer;\n" +
        "5.select question;\n" +
        "6.end.";
        Console.WriteLine(answerInfo);
        //Console.ReadKey();
    }
    private void PassingInfo(User user)
    {
        string? passingInfo = "***** Passing the Test *****\n" +
                     $"file: {FilePath}\n" +
                     $"user: {user};\n" +
                     "\n/next to go to next answer;\n" +
                     "/prev to go to previous answer;\n" +
                     "/end;\n";
        Console.WriteLine(passingInfo);
        //Console.ReadKey();
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
        //Console.ReadKey();
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
        if(!filled)
        {
            if(!AskForFilePath(ref inter))
                return;
        }

        Console.WriteLine(Test.ToString(format));
    }
    private void ShowQuestions(bool filled = false)
    {
        if(!filled)
        {
            if(!AskForFilePath(ref inter))
                return;
        }

        Show(Test.Questions);
    }
    private void ShowQuestion(Test test, int questionIndex)
    {
        var question = test[questionIndex];
        Console.WriteLine(question + "\n" + question.Answers.ToString("T"));
    }
    private void ShowStatistic(bool filled = false, Statistic? stats = null)
    {
        if(!filled)
        {
            if(!AskForFilePath(ref inter))
                return;
        }

        stats ??= Test.Statistic;

        Console.WriteLine(stats.ToString());
    }
    #endregion

    #region Create
    public void Create(bool def)
    {
        if(!AskForFilePath(ref inter))
            return;
        if(def)
        {
            inter.AddTest(inter.DefTest);
            ShowTest(true);
        }
        else
        {
            AddQuestion();
        }
    }
    #endregion

    #region Change
    private void StartChanging()
    {
    askForFilePath:
        Console.Clear();
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
                int number;
                if(int.TryParse(input, out number))
                {
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
                }
                if(changeMenu.ContainsKey(input))
                    changeMenu[input]();
                else
                {
                    Console.WriteLine("Unknow Command");
                }
            }
            catch(SerializationException)
            {
                Console.WriteLine("File is empty or not filled with test!");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        } while(true);
    } // working 1/11 1:57 p.m
    private void StartChangingQuestion()
    {
        do
        {
            string input = "";
            Console.Clear();
            QuestionInfo();

            int number;
            do
            {
                Console.Write("Enter what you want to do: ");
                input = Console.ReadLine();
                if(input == "/return" || input == "4")
                    return;
            } while(!int.TryParse(input, out number));

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
                Console.WriteLine("Unknow Command");
                Console.ReadKey();
                continue;
            }
            Console.ReadKey();
        } while(true);
    } // working 1/11 1:57 p.m
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

            if(input == "/return")
                return;


            var test = Test;

            if(!int.TryParse(input, out questionIndex))
                continue;

            --questionIndex; // because in list counting starts from 0 

            if(!inter.IsIndexValid(questionIndex, test.Questions))
            {
                Console.WriteLine(inter.wrongIndex.Message);
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
            do
            {
                Console.Write("Enter what you want to do: ");
                input = Console.ReadLine();
                if(input == "/select" || input == "5")
                    goto askQuestionIndex;
                if(input == "/end" || input == "6")
                    return;
            } while(!int.TryParse(input, out number));

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
                Console.WriteLine("Unknow Command");
                Console.ReadKey();
                continue;
            }

        } while(true);
    } // working 1/11 1:57 p.m
    #endregion

    #region Changing Question
    private void AddQuestion()
    {
        ShowQuestions(true);
        //ask for question and while question is not valid
        //or question != /return keep asking
        Question question;
    loop:
        Console.Write("Enter your question" +
            "\n(your question should star with upper case letter and end with ?(question mark))\n: ");
        string questionString = Console.ReadLine();

        if(questionString == "/return")
            return;

        try
        {
            question = inter.CreateQuestion(questionString);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            goto loop;
        }

        inter.AddQuestion(question);

        int questionIndex = Test.Questions.IndexOf(question);

        AddAnswer(questionIndex);
        ShowTest(true);
    } // working 1/11 1:57 p.m
    private void DeleteQuestion()
    {
    loop:
        int questionIndex;
        string input;
        do
        {
            ShowQuestions(true);
            Console.Write("Enter index of question which you want to delete: ");
            input = Console.ReadLine();
            if(input == "/return")
                return;
        } while(!int.TryParse(input, out questionIndex));

        --questionIndex; // because in list counting starts from 0
        try
        {
            inter.DeleteQuestion(questionIndex);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            goto loop;
        }

        if(inter.Count > 0)
            goto loop;

        ShowQuestions(true);
    } // working 1/11 1:57 p.m
    private void ChangeQuestion()
    {
    // enter index of question while it is not valid
    loop:
        string input;
        int questionIndex;

        do
        {
            ShowQuestions(true);
            Console.Write("Enter index of question which you want to change: ");
            input = Console.ReadLine();
            if(input == "/return")
                return;

        } while(!int.TryParse(input, out questionIndex));
        --questionIndex; // because in list counting starts from 0

        if(inter.IsIndexValid(questionIndex, Test.Questions))
        {
        // enter new question while it is not valid
        loop2:
            Console.Write("Enter new question: ");
            string question = Console.ReadLine();

            try
            {
                inter.ChangeQuestion(questionIndex, inter.CreateQuestion(question));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                goto loop2;
            }
        }
        else
        {
            Console.WriteLine(inter.wrongIndex.Message);
            goto loop;
        }
        ShowQuestions(true);
    }  // working 1/11 1:57 p.m
    #endregion

    #region Changing Answers
    private void AddAnswer(int questionIndex)
    {
    //enter answer while answers.Count < Answers.maxCapacity
    loop1:
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
                ;
            }
            else
            {
                inter.AddAnswer(questionIndex, input);
                goto loop1;
            }
        }

        //enter index of right answer while it is not valid
        SetRightAnswer(questionIndex);
    } // working 1/11 1:57 p.m
    private void DeleteAnswer(int questionIndex)
    {
    //enter index of answer while it is not valid
    loop1:
        var answers = Test[questionIndex].Answers;
        int answerIndex = 0;
        if(!AskAnswerIndex(questionIndex, ref answerIndex, answers))
            goto set;

        if(!inter.IsIndexValid(answerIndex, answers))
        {
            Console.WriteLine(inter.wrongIndex.Message);
            goto loop1;
        }

        int count = answers.Count;
        inter.DeleteAnswer(questionIndex, answerIndex);

        if(--count > 0)
            goto loop1;
        set:
        //enter index of right answer while it is not valid
        SetRightAnswer(questionIndex);
    } // working 1/11 1:57 p.m
    private void ChangeAnswer(int questionIndex)
    {
    //enter index of answer while it is not valid
    loop1:
        var answers = Test[questionIndex].Answers;
        int answerIndex = 0;

        if(!AskAnswerIndex(questionIndex, ref answerIndex, answers))
            goto set;

        if(!inter.IsIndexValid(answerIndex, answers))
        {
            Console.WriteLine(inter.wrongIndex.Message);
            goto loop1;
        }

        Console.Write("Enter new answer: ");
        inter.ChangeAnswer(questionIndex, answerIndex, Console.ReadLine());

    set:
        SetRightAnswer(questionIndex);
    } // working 1/11
    private void SetRightAnswer(int questionIndex)
    {
    loop:
        var answers = Test[questionIndex].Answers;
        if(answers.Count <= 0)
            return;
        Console.Write($"Please choose right answer (1-{answers.Count})\n" +
                $"{answers.ToString("A")}\n" +
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
    } // working 1/11
    private bool AskAnswerIndex(int questionIndex, ref int answerIndex, IList answers)
    {
        //var answers = Test[questionIndex].Answers;
        Console.Write("Enter index of answer" +
            $"\n{answers}" +
            ": ");
        string input;
        do
        {
            input = Console.ReadLine();
            if(input == "/return" || input == "/end")
                return false;

        } while(!int.TryParse(input, out answerIndex));

        answerIndex--;
        return true;
    } // working 1/11 1:57 p.m

    #endregion

    #region Pass
    private User AskUserData()
    {
        User user;
        Console.Clear();
        do
        {

            Console.Write("Enter your first name: ");
            string? firstName = Console.ReadLine();

            if(firstName == "/return")
                return null;

            Console.Write("Enter your last name: ");
            string? lastName = Console.ReadLine();
            if(lastName == "/return")
                return null;

            try
            {
                user = inter.CreateUser(firstName, lastName);
                break;
            }
            catch(UserException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("First and Last name must start with upper case letter and contain only letters!");
            }
        } while(true);
        return user;
    }
    private void PassTest()
    {
    //ask for file path while file does not contain test
    loop:
        Console.Clear();

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
    } // TODO : refactor
    #endregion

    #region Stats
    private void StartStats()
    {
    askForFilePath:
        Console.Clear();
        if(!AskForFilePath(ref inter))
            return;

        string? input = "";
        do
        {
            Console.Clear();
            StatisticInfo();
            try
            {
            loop:
                Console.Write("Enter what you want to do: ");
                input = Console.ReadLine();
                int number;
                if(int.TryParse(input, out number))
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
                        break;
                        case 7:
                        mainMenu["/cls"]();
                        break;
                        default:
                        Console.WriteLine("Unknow Command");
                        goto loop;
                    }
                }
                else
                {
                    Console.WriteLine("Unknow Command");
                }
            }
            catch(SerializationException)
            {
                Console.WriteLine("File is empty or not filled with test!");
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
        Console.Write("Enter file path:");
        string? filePath = Console.ReadLine();
        if(filePath == "/return")
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
        if(!filled)
            if(!AskForFilePath(ref inter))
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
