using Azure;
using Ekzakt.FileManager.AzureBlob.Services;
using Ekzakt.FileManager.Core.Contracts;
using Ekzakt.Templates.Console.Utilities;
using System.ComponentModel;

namespace Ekzakt.FileManager.Console
{
    public class TaskRunner(ConsoleHelpers? c, IFileManager fileManager) : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task SaveFile()
        {
            c.Clear();

            while (true)
            {

                fileManager.ProgressEventHandler += DisplayProgress;

                //var basePath = AppDomain.CurrentDomain.BaseDirectory;
                var basePath = @"C:\Users\EricJansen\source\repos\Ekzakt\Ekzakt.Showcases\Ekzakt.FileManager.Console\Files";
                var folder = "demo-blazor8";
                var file = "video01.mp4";

                var path  = Path.Combine(basePath, file);

                using Stream fs = File.OpenRead(path);

                var result = await fileManager.SaveAsync(folder, fs, "file01.jpg");

                c.WriteResult(c.WriteJson(result));
                c.Write();

                if (!c.ConfirmYesNo("Would you like to try again?")) break;
            }
        }

        private void DisplayProgress(object? sender, EventArgs e)
        {
            System.Console.CursorVisible = false;
            c.Clear();
            c.Write(Guid.NewGuid().ToString());
            System.Console.CursorVisible = true;
        }

        //private void DisplayProgress(object? sender, ProgressChangedEventArgs e)
        //{
        //    c.Clear();
        //    c.Write(e..ToString());
        //}

        private void DisplayProgress(object? sender, ProgressEventArgs e)
        {
            c.Clear();
            c.Write(e.BytesSent.ToString());
        }

        private void DisplayProgress(long bytesUpload)
        {
           
        }
    }
}
