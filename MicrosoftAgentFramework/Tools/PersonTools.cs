
using MicrosoftAgentFramework.Utilities;

namespace MicrosoftAgentFramework.Tools;

public class PersonTools
{
    public PersonInfo[] GetPersons()
    {
       Output.Gray("Getting persons...");
        return GetData();
    }

    public PersonInfo? GetPerson(string name)
    {
        Output.Gray($"(GetPerson was called with '{name}')");
        PersonInfo[] data = GetData();
        return data.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
    }  

    private static PersonInfo[] GetData()
    {
        return
        [
            new PersonInfo("Rasmus", "Blue"),
                new PersonInfo("John", "Red"),
                new PersonInfo("Ben", "Green"),
                new PersonInfo("Jenny", "Red"),
                new PersonInfo("Mona", "Yellow"),
            ];
    }

    //Action Tool
    public static void ChangeConsoleColor(ConsoleColor color)
    {
        Output.Gray($"(ChangeConsoleColor was called with '{color}')");
        Console.ForegroundColor = color;
    }
}

