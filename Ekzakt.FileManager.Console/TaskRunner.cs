using Ekzakt.FileManager.AzureBlob.Services;
using Ekzakt.FileManager.Core.Contracts;
using Ekzakt.Templates.Console.Utilities;

namespace Ekzakt.FileManager.Console
{
    public class TaskRunner(ConsoleHelpers? c, IFileManager fileManager)
    {
        public async Task SaveFile()
        {
            c.Clear();

            while (true)
            {
                var result = await fileManager.SaveAsync();

                c.WriteResult(c.WriteJson<IFileResult>(result));
                c.Write();

                if (!c.ConfirmYesNo("Would you like to try again?")) break;
            }
        }
    }

}
