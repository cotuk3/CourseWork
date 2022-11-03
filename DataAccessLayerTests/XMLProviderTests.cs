using BusinessLogicLayer.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.Tests;

[TestClass()]
public class XMLProviderTests
{
    //[TestMethod()]
    //public void XMLProviderTest()
    //{
    //    Assert.Fail();
    //}

    //[TestMethod()]
    //public void XMLProviderTest1()
    //{
    //    Assert.Fail();
    //}

    [TestMethod()]
    public void SerializeTest()
    {

    }

    [TestMethod()]
    public void DeserializeTest()
    {
        //arrange
        Question expected = new("How are you?") { Answers = new() { "Good", "Normal" } };

        //act
        XMLProvider xp = new(typeof(Question));
        xp.Serialize(expected, "file.xml");
        var actual = xp.Deserialize("file.xml") as Question;

        //assert
        Assert.AreEqual(expected, actual, actual);
    }
}