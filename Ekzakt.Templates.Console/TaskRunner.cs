namespace EmailTemplateProvider.Console;

public class TaskRunner
{
    private ConsoleHelpers c = new();

    public TaskRunner() { }

    public async Task DoSomething()
    {
        c.Clear();

        while (true)
        {
            c.Write($"Doing '{nameof(DoSomething)}'.");
            c.Write();

            if (!c.ConfirmYesNo("Would you like to try again?")) break;
        }
    }



    /// <summary>
    /// Writes a list of taks prefixed by follow-up letter from the 
    /// alpabet.  Q is preserved for quitting.
    /// </summary>
    /// <param name="taskList"></param>
    /// <returns></returns>
    public ConsoleKeyInfo WriteTaskList(List<string> taskList)
    {
        var alphabet = "ABCDEFGHIJKLMOPRSTUVWXYZ";

        var counter = 0;

        c.Clear();

        foreach (var task in taskList)
        {
            c.Write($"{alphabet.Substring(counter, 1)} = {task}");
            counter++;
        };

        c.Write($"{ConsoleKey.Q} = Quit.");

        var output = System.Console.ReadKey(true);
        return output;
    }
}

