using BusinessLogicLayer.Entity;
using BusinessLogicLayer.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace BusinessLogicLayer.Tests;

[TestClass()]
public class InteractionTests
{
    #region Fields
    static Interaction inter = new();

    static Answers answers = new() { "A", "B", "C" };
    static Answers answers1 = new() { "1", "0", "-10" };
    static Answers answers2 = new() { "a", "b", "c" };

    static Question question = new Question("How are you?") { Answers = answers };
    static Question question1 = new Question("What is number of letter A in alphabet?") { Answers = answers1 };
    static Question question2 = new Question("What is lover case letter of D?") { Answers = answers2 };

    static Test test = new() { Questions = { question, question1, question2 } };
    #endregion

    [TestMethod()]
    public void InteractionTest()
    {

    }

    [TestMethod()]
    public void InteractionTest1()
    {

    }

    #region IsQuestionValid
    [TestMethod()]
    public void IsQuestionValid_Success()
    {
        //arrange
        bool expected = true;

        //act
        Interaction interaction = new();
        var question = interaction.CreateQuestion("How old are 123 you?");
        bool actual = interaction.IsQuestionValid(question);

        //assert
        Assert.AreEqual(expected, actual, question);
    }

    [TestMethod()]
    [DataRow("How are you!")]
    [DataRow("How are you.")]
    [DataRow("How are you")]
    [DataRow("1ow are you!")]
    [DataRow("how are you!")]
    public void IsQuestionValid_Fail(string questionS)
    {
        //arrange
        bool expected = false;

        //act
        bool actual = inter.IsQuestionValid(questionS);

        //assert
        Assert.AreEqual(expected, actual);
    }
    #endregion

    #region IsIndexValid
    [TestMethod()]
    public void IsIndexValid_2_Success()
    {
        //arrange
        int index = 2;

        //act
        bool actual = inter.IsIndexValid(index, test[0].Answers);

        //assert
        Assert.IsTrue(actual);
    }

    [TestMethod()]
    public void IsIndexValid_Minus1_Fail()
    {
        //arrange
        int index = -1;

        //act
        bool actual = inter.IsIndexValid(index, test.Questions);

        //assert
        Assert.IsFalse(actual);
    }

    [TestMethod()]
    public void IsIndexValid_3_Fail()
    {
        //arrange
        int index = 6;

        //act
        Debug.WriteLine(test.Count);
        bool actual = inter.IsIndexValid(index, test.Questions);

        //assert
        Assert.IsFalse(actual);
    }
    #endregion


    #region AddTest
    [TestMethod()]
    public void Add_1Test_Success()
    {
        //act
        inter = new("file.xml");
        inter.ClearFile();
        inter.AddTest(test);
    }

    [TestMethod()]
    public void Add_2Tests_Success()
    {
        //arrange
        Test expected = new Test();
        expected.AddRange(test.Questions);
        expected.AddRange(test.Questions);

        //act
        inter = new("file.xml");
        inter.ClearFile();
        inter.AddTest(test);
        inter.AddTest(test);
    }
    #endregion

    #region GetTest
    [TestMethod()]
    public void GetTest_Success()
    {
        //arrange
        var expected = test;

        //act
        inter = new("file.xml");
        inter.ClearFile();
        inter.AddTest(test);
        var actual = inter.GetTest();


        Assert.AreEqual(expected.ToString() , actual.ToString() );
    }

    [TestMethod()]
    [ExpectedException(typeof(FileNotFoundException))]
    public void GetTest_NotExistingFile_Fail()
    {
        //arrange
        var expected = test;

        //act
        inter = new("file1123.xml");
        var actual = inter.GetTest();


        //Assert.AreEqual(expected, actual, actual.ToString());
    }

    [TestMethod()]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GetTest_FileWithOtherData_Fail()
    {
        //arrange
        var expected = test;

        //act
        File.Create("file1.xml").Close();
        inter = new("file1.xml");
        inter.GetTest();

        //Assert.AreEqual(expected, actual, actual.ToString());
    }
    #endregion

    [TestMethod()]
    public void AddQuestion_Success()
    {
        //arrange
        Test expected = new() { Questions = { new Question("How are you?") } };

        //act

        inter = new("file.xml");
        inter.ClearFile();
        inter.AddQuestion(new Question("How are you?"));
        var actual = inter.GetTest();

        //arrange
        Assert.AreEqual(expected.ToString(), actual.ToString());
    }

    [TestMethod()]
    public void DeleteQuestionTest()
    {
        //arrange
        Test expected = new();
        expected.AddRange(test.Questions);
        expected.RemoveAt(1);

        //act
        inter = new("file.xml");
        inter.ClearFile();
        inter.AddTest(test);
        inter.DeleteQuestion(1);
        var actual = inter.GetTest();

        //assert
        Assert.AreEqual(expected.ToString(), actual.ToString());
    }

    #region ChangeQuestion
    [TestMethod()]
    public void ChangeQuestion_Success()
    {
        //arrange
        Test expected = new();
        expected.AddRange(test.Questions);
        expected[0] = question1;
        expected[0].Answers = question.Answers;

        //act
        inter = new("file.xml");
        inter.ClearFile();
        inter.AddTest(test);
        inter.ChangeQuestion(0, question1);
        var actual = inter.GetTest();

        //assert
        Assert.AreEqual(expected.ToString(), actual.ToString());
    }

    [TestMethod()]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void ChangeQuestion_WrongIndex_Fail()
    {
        //act
        inter = new("file.xml");
        inter.ClearFile();
        inter.AddTest(test);
        inter.ChangeQuestion(-1, question1);
        //var actual = inter.GetTest();
    }

    [TestMethod()]
    [ExpectedException(typeof(QuestionException))]
    public void ChangeQuestion_WrongQuestion_Fail()
    {
        //act
        inter = new("file.xml");
        inter.ClearFile();
        inter.AddTest(test);
        inter.ChangeQuestion(1, "Hi my name");
        //var actual = inter.GetTest();
    }
    #endregion

    #region AddAnswer
    [TestMethod()]
    public void AddAnswer_Success()
    {
        //arrange 
        Question expectedQ = new("How are you?");
        expectedQ.Answers.Add("D");
        Test expected = new() { Questions = { expectedQ } };


        //act
        inter = new("file.xml");
        inter.ClearFile();
        var actualQ = inter.CreateQuestion(question);
        inter.AddQuestion("How are you?");
        inter.AddAnswer(0, "D");
        var actual = inter.GetTest();

        //assert
        Assert.AreEqual(expected.ToString(), actual.ToString());
    }

    [TestMethod()]
    [ExpectedException(typeof(AnswerException))]
    public void AddAnswer_WrongAnswer_Fail()
    {
        //arrange 
        Question expectedQ = new("How are you?") { Answers = answers };
        expectedQ.Answers.Add("D");
        Test expected = new() {Questions = { expectedQ } };


        //act
        inter = new("file.xml");
        inter.ClearFile();
        var actualQ = inter.CreateQuestion(question);
        inter.AddQuestion(question);
        inter.AddAnswer(0, "D");
        inter.AddAnswer(0, "E");
        var actual = inter.GetTest();


        //assert
        Assert.AreEqual(expected, actual, $"{actual} != {expected}");
    }

    [TestMethod()]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void AddAnswer_WrongQuestionIndex_Fail()
    {

        //act
        inter = new("file.xml");
        inter.ClearFile();
        var actualQ = inter.CreateQuestion(question);
        inter.AddQuestion(question);
        inter.AddAnswer(3, "D");
        inter.AddAnswer(3, "E");
    }
    #endregion

    [TestMethod()]
    public void DeleteAnswerTest()
    {
        //arrange 
        Question expectedQ = new("How are you?") { Answers = answers };
        expectedQ.Answers.RemoveAt(0);
        Test expected = new() { Questions = { expectedQ } };

        //act
        inter = new("file.xml");
        inter.ClearFile();
        var actualQ = inter.CreateQuestion(question);
        inter.AddQuestion("How are you?");
        inter.AddAnswer(0, "A");
        inter.AddAnswer(0, "B");
        inter.AddAnswer(0, "C");
        inter.DeleteAnswer(0, 0);
        var actual = inter.GetTest();

        //assert
        Assert.AreEqual(expected.ToString() , actual.ToString());
    }

    #region CreateQuestion
    [TestMethod()]
    public void CreateQuestion_Success()
    {
        //arrange
        Question expected = new Question("How are you?");

        //act
        var actual = inter.CreateQuestion("How are you?");

        //assert
        Assert.AreEqual(expected, actual, actual.ToString());
    }

    [TestMethod()]
    [ExpectedException(typeof(QuestionException))]
    [DataRow("How are you!")]
    [DataRow("How are you.")]
    [DataRow("How are you")]
    [DataRow("1ow are you!")]
    [DataRow("how are you!")]
    public void CreateQuestion_Fail(string question)
    {
        //act
        var actual = inter.CreateQuestion(question);
    }
    #endregion

    #region SetRightAswer
    [TestMethod()]
    public void SetRightAnswer_Success()
    {
        //arrange
        Test expectedT = new();
        expectedT.AddRange(test.Questions);
        expectedT[0].RightAnswer = 2;
        var expected = expectedT.Questions[0].Answers.RightAnswer;

        //act
        inter = new("file1.xml");
        inter.ClearFile();
        inter.AddTest(test);
        inter.SetRightAnswer(0, 2);
        var actual = inter.GetTest().Questions[0].Answers.RightAnswer;

        //assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void SetRightAnswer_WrongAnswerIndex_Fail()
    {
        inter = new("file.xml");
        inter.ClearFile();
        inter.AddTest(test);
        inter.SetRightAnswer(0, 4);
    }

    [TestMethod()]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void SetRightAnswer_WrongQuestionIndex_Fail()
    {
        inter = new("file.xml");
        inter.ClearFile();
        inter.AddTest(test);
        inter.SetRightAnswer(-1, 1);
    }
    #endregion

    [TestMethod()]
    public void ChangeAnswerTest()
    {
        //arrange 
        Question expectedQ = new("How are you?");
        expectedQ.Answers.Add("A");
        Test expected = new() { Questions = { expectedQ } };


        //act
        inter = new("file.xml");
        inter.ClearFile();
        var actualQ = inter.CreateQuestion(question);
        inter.AddQuestion("How are you?");
        inter.AddAnswer(0, "D");
        inter.ChangeAnswer(0, 0, "A");
        var actual = inter.GetTest();

        //assert
        Assert.AreEqual(expected.ToString(), actual.ToString());
    }

    [TestMethod()]
    public void GetPersentOfRightAnswersTest()
    {
        //Assert.Fail();
    }
}