using Ekzakt.Templates.Console.Utilities;

namespace Ekzakt.AiCodeAggregator
{
    public class TaskRunner(ConsoleHelpers c)
    {
        public async Task AggregateProjectFiles()
        {
            await Task.Delay(10);

            c.Clear();

            while (true)
            {
                c.Write("What folder do you want to search?");
                var basePath = c.ReadLine();

                if (!Path.Exists(basePath))
                {
                    c.WriteError("The path you provided does not exist.");
                    if (!c.ConfirmYesNo("Do you want to try again?"))
                    {
                        break;
                    }
                    break;
                }

                List<string> csFiles = Directory.GetFiles(basePath, "*.cs", SearchOption.AllDirectories).ToList();

                var count = csFiles.RemoveAll(x => x.EndsWith(".g.cs"));
                count += csFiles.RemoveAll(x => x.EndsWith(".AssemblyInfo.cs"));
                count += csFiles.RemoveAll(x => x.EndsWith(".AssemblyAttributes.cs"));


                Console.WriteLine("List of .cs files:");
                foreach (var file in csFiles)
                {
                    c.Write(file.Trim());
                }

                if (!c.ConfirmYesNo( $"Continue aggregating {csFiles?.Count ?? 0} files?"))
                {
                    break;
                }

                foreach (string csFile in csFiles ?? [])
                {
                    var fileText = File.ReadAllText(csFile);
                    var dest = Path.Combine(basePath, "Aggregator.txt");

                    File.AppendAllText(dest, $"*** Begin file: {csFile}" + Environment.NewLine + Environment.NewLine);
                    File.AppendAllText(dest, fileText);
                    File.AppendAllText(dest, Environment.NewLine + $"*** End file: {csFile}" + Environment.NewLine + Environment.NewLine);
                }
            }
        }
    }

}
