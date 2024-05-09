using Ekzakt.FileManager.Core.Contracts;
using Ekzakt.FileManager.Core.Models.EventArgs;
using Ekzakt.FileManager.Core.Models.Requests;
using Ekzakt.FileManager.Core.Models.Responses;
using Ekzakt.Templates.Console.Utilities;
using Ekzakt.Utilities.Extensions;
using System.Net;

namespace Ekzakt.FileManager.Console;

public class TaskRunner(ConsoleHelpers c, IEkzaktFileManager fileManager)
{
    private const string BASE_LOCATION = "data";
     
    public async Task SaveFileAsync(bool useChunks = false, bool useStream = false)
    {
        var progressHandler = new Progress<ProgressEventArgs>(progress =>
        {
            c.Write($"Uploading file {progress.FileName}: {(int)progress.PercentageDone} % done.");
        });

        var basePath = @"C:\Users\EricJansen\source\repos\Ekzakt\Ekzakt.Showcases\Ekzakt.FileManager.Console\Files";

        while (true)
        {

            var fileList = new List<string>();

            foreach (var file in Directory.GetFiles(basePath, "*.*", SearchOption.TopDirectoryOnly).ToList())
            {
                var info = new FileInfo(file);

                fileList.Add($"{info.Name} - {info.Length.FormatFileSize(0)}");
            }

            var fileKeyMap = BuildFileKeyMap(fileList);
            var key = c.WriteMenu(fileList);
            var sourceFile = string.Empty;

            while(string.IsNullOrEmpty(sourceFile))
            {
                if (fileKeyMap.TryGetValue(key.Key, out string? source))
                {
                    sourceFile = source.Split('-')[0];
                }

                if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }

            var fileName = string.Empty;

            while (string.IsNullOrEmpty(fileName))
            {
                c.Write($"Enter the destination filename for source file {sourceFile}:");
                fileName = c.ReadLine();
            }


            var containerName = BASE_LOCATION;
            var path = Path.Combine(basePath, sourceFile);

            if (!useChunks)
            {
                await SaveFileAsync(path, fileName, containerName, progressHandler);
            }
            else
            {
                if (!useStream)
                {
                    await SaveFileChunkedAsync(path, fileName, containerName, progressHandler);
                }
                else
                {
                    await SaveFileStreamAsync(path, fileName, containerName, progressHandler);
                }
            }
            
            if (!c.ConfirmYesNo("Do you want to try again?"))
            {
                break;
            }
        }
    }


    public async Task ListFilesAsync()
    {
        while (true)
        {
            c.Clear();

            var pathsString = string.Empty;

            while (string.IsNullOrEmpty(pathsString))
            {
                c.Write("Which path(s) do you want to download? Separate multiple paths by a semicolon (;).");
                c.Write("Type a dash (-) to list all files.");

                pathsString = c.ReadLine();
            }

            var pathsList = new List<string>();

            if (!string.IsNullOrEmpty(pathsString) && !pathsString.Equals("-", StringComparison.OrdinalIgnoreCase))
            {
                pathsList.AddRange(pathsString.Split(";"));
            }

            var request = new ListFilesRequest
            {
                //BaseLocation = BASE_LOCATION,
                Paths = pathsList
            };

            var result = await fileManager.ListFilesAsync(request);

            if (result.IsSuccess())
            {
                c.WriteSuccess(result);
            }
            else
            {
                c.WriteError(result);
            }

            if (!c.ConfirmYesNo("Do you want to try again?"))
            {
                break;
            }
        }
    }


    public async Task DownloadFileAsync()
    {
        while (true)
        {
            var fileName = string.Empty;

            while (string.IsNullOrEmpty(fileName))
            {
                c.Write("Which file do you want to download?");
                fileName = c.ReadLine();
            }


            var request = new DownloadSasTokenRequest
            {
                BaseLocation = BASE_LOCATION,
                FileName = fileName
            };

            var result = await fileManager.DownloadSasTokenAsync(request);

            if (result.IsSuccess())
            {
                c.WriteSuccess(result);
            }
            else
            {
                c.WriteError(result);
            }


            if (!c.ConfirmYesNo("Do you want to try again?"))
            {
                break;
            }
        }
    }


    public async Task DeleteFileAsync()
    {
        while (true)
        {
            var fileName = string.Empty;

            while (string.IsNullOrEmpty(fileName))
            {
                c.Write("Which file do you want to delete?");
                fileName = c.ReadLine();
            }


            var request = new DeleteFileRequest
            {
                BaseLocation = BASE_LOCATION,
                FileName = fileName
            };


            var result = await fileManager.DeleteFileAsync(request);

            if (result.IsSuccess())
            {
                c.WriteSuccess(result);
            }
            else
            {
                c.WriteError(result);
            }


            if (!c.ConfirmYesNo("Do you want to try again?"))
            {
                break;
            }
        }
    }


    public async Task ReadFileAsStringAsync()
    {
        while (true)
        {
            var fileName = string.Empty;

            while (string.IsNullOrEmpty(fileName))
            {
                c.Write("Which file do you want to read?");
                fileName = c.ReadLine();
            }


            var request = new ReadFileAsStringRequest
            {
                BaseLocation = BASE_LOCATION,
                FileName = fileName
            };


            var result = await fileManager.ReadFileStringAsync(request);

            if (result.IsSuccess())
            {
                c.WriteSuccess(result);
            }
            else
            {
                c.WriteError(result);
            }


            if (!c.ConfirmYesNo("Do you want to try again?"))
            {
                break;
            }
        }
    }




    #region Helpers

    private async Task SaveFileAsync(string path, string fileName, string containerName, Progress<ProgressEventArgs> progressHandler)
    {
        using FileStream fileStream = File.OpenRead(path);

        var request = new SaveFileRequest
        {
            BaseLocation = containerName,
            FileName = fileName,
            FileStream = fileStream,
            InitialFileSize = fileStream.Length,
            ProgressHandler = progressHandler
        };

        var result = await fileManager.SaveFileAsync(request);

        if (result.IsSuccess())
        {
            c.WriteSuccess(result);
        }
        else
        {
            c.WriteError(result);
        }
    }


    private async Task SaveFileChunkedAsync(string path, string fileName, string containerName, Progress<ProgressEventArgs> progressHandler)
    {
        const long CHUNK_SIZE = (1024 * 1024) * 1;

        using FileStream fileStream = File.OpenRead(path);

        var uploadedBytes = (long)0;
        var percentDone = (long)0;
        var totalBytes = fileStream.Length;
        var chunkIndex = 0;
        var response = new FileResponse<string?>();

        while (uploadedBytes < totalBytes)
        {
            long chunkSize = Math.Min(CHUNK_SIZE, totalBytes - uploadedBytes);

            byte[] chunk = new byte[chunkSize];

            await fileStream.ReadAsync(chunk, 0, (int)chunkSize);

            var request = new SaveFileChunkedRequest
            {
                BaseLocation = containerName,
                FileName = fileName,
                InitialFileSize = totalBytes,
                ChunkData = Convert.ToBase64String(chunk),
                ChunkIndex = chunkIndex,
                ChunkTreshold = CHUNK_SIZE,
                ProgressHandler = progressHandler
            };

            response = await fileManager.SaveFileChunkedAsync(request);

            if (response!.Status == HttpStatusCode.Continue)
            {
                uploadedBytes += chunkSize;
                percentDone = uploadedBytes * 100 / totalBytes;

                c.Write($"Uploaded {percentDone}%  {uploadedBytes} of {totalBytes} | Fragment: {chunkIndex}");

                chunkIndex++;
            }
            else if (response.IsSuccess())
            {
                c.WriteSuccess(response);
                break;
            }
            else
            {
                c.WriteError(response);
                break;
            }
        }
    }


    // TODO: Fix this!
    private async Task SaveFileStreamAsync(string path, string fileName, string containerName, Progress<ProgressEventArgs> progressHandler)
    {
        await using (var fileStream = File.OpenRead(path))

        using (var fileContent = new StreamContent(fileStream))
        using (var formData = new MultipartFormDataContent())

        formData.Add(fileContent, "files", $"{Guid.NewGuid()}.jpg");
    }


    private Dictionary<ConsoleKey, string> BuildFileKeyMap(List<string> fileList)
    {
        //Dictionary<string, ConsoleKey> fileKeyMap = [];
        Dictionary<ConsoleKey, string> fileKeyMap = [];


        ConsoleKey currentKey = ConsoleKey.A;

        foreach (var file in fileList)
        {
            //fileKeyMap[file] = currentKey;
            fileKeyMap[currentKey] = file;

            currentKey++;

            if (currentKey > ConsoleKey.Z)
            { 
                currentKey = ConsoleKey.A;
            }
        }

        return fileKeyMap;
    }
    #endregion Helpers

}
