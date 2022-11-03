using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataAccess;
public class JSONProvider : DataProvider
{
    public JSONProvider(Type type)
        : base(type)
    {

    }

    public override void Serialize(object graph, string filePath)
    {
        using(FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            JsonSerializer.Serialize(fileStream, graph, _type);
        }
    }

    public override object Deserialize(string filePath)
    {
        object obj;
        using(var fileStream = File.OpenRead(filePath))
        {
            try
            {
                obj = JsonSerializer.Deserialize(fileStream, _type);
                return obj;
            }
            catch
            {
                return null;
            }
        }
    }
}