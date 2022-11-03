namespace BusinessLogicLayer.Entity;
[Serializable]
public class User
{
    public User()
    {

    }
    public User(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public override string ToString()
    {
        return FirstName + " " + LastName;
    }
    public override bool Equals(object? obj)
    {
        User? user = obj as User;
        if(obj is not null)
        {
            return FirstName == user.FirstName && LastName == LastName;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return FirstName.GetHashCode() * LastName.GetHashCode();
    }
}
