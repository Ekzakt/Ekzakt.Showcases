namespace EmailTemplateProvider.Console;

public class RunTask
{
    private ConsoleHelpers c = new();

    public RunTask() { }

    public async Task DoSomething()
    {
        Clear();

        while (true)
        {
            if (!c.ConfirmYes("Would you like to do something?")) break;
        }
    }




    #region Helpers

    private void WriteEmpty(string? message = "")
    {
        System.Console.WriteLine(message);
    }


    private void WriteLineMiddele(string? message = "")
    {
        System.Console.WriteLine();
        System.Console.WriteLine(message);
        System.Console.WriteLine();
    }


    private void WriteLineAfter(string? message = "")
    {
        System.Console.WriteLine(message);
        System.Console.WriteLine();
    }


    private void WriteError(Exception ex)
    {
        System.Console.ForegroundColor = ConsoleColor.Red;
        WriteLineAfter(ex.ToString());
        System.Console.ForegroundColor = ConsoleColor.White;
    }


    private void WriteResult(string? message)
    {
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.DarkGreen;
        System.Console.WriteLine(message);
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.White;
    }


    private void Clear()
    {
        System.Console.Clear();
    }


    #endregion Helpers
}

