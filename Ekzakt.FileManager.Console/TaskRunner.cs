using Ekzakt.FileManager.Core.Contracts;
using Ekzakt.FileManager.Core.Models;
using Ekzakt.FileManager.Core.Models.Requests;
using Ekzakt.FileManager.Core.Models.Responses;
using Ekzakt.Templates.Console.Utilities;
using System.Net;

namespace Ekzakt.FileManager.Console;

public class TaskRunner(ConsoleHelpers c, IFileManager fileManager)
{
    public async Task SaveFileAsync(bool useChunks = false, bool useStream = false)
    {
        var progressHandler = new Progress<ProgressEventArgs>(progress =>
        {
            c.Write($"Uploading file {progress.FileName}: {(int)progress.PercentageDone} % done.");
        });


        while (true)
        {
            var menu = new List<string>()
            {
                "file01.jpg",
                "file02.jpg",
                "file03.jpg",
                "video01.mp4",
                "video02.mov"
            };
            
            var key = c.WriteMenu(menu);

            var sourceFile = key.Key switch
            {
                ConsoleKey.A => "file01.jpg",
                ConsoleKey.B => "file02.jpg",
                ConsoleKey.C => "file03.jpg",
                ConsoleKey.D => "video01.mp4",
                ConsoleKey.E => "video02.mov",
                _ => "file01.jpg"
            };

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }

            var fileName = string.Empty;

            while (string.IsNullOrEmpty(fileName))
            {
                c.Write($"Enter the destination filename for source file {sourceFile}:");
                fileName = c.ReadLine();
            }

            var basePath = @"C:\Users\EricJansen\source\repos\Ekzakt\Ekzakt.Showcases\Ekzakt.FileManager.Console\Files";
            var containerName = "demo-blazor8";
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


    public async Task ListFiles()
    {
        while (true)
        {
            var request = new ListFilesRequest
            {
                BlobContainerName = "demo-blazor8"
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


    public async Task DownloadFile()
    {
        while (true)
        {
            var fileName = string.Empty;

            while (string.IsNullOrEmpty(fileName))
            {
                c.Write("Which file do you want to download?");
                fileName = c.ReadLine();
            }


            var request = new DownloadFileRequest
            {
                BlobContainerName = "demo-blazor8",
                FileName = fileName
            };

            var result = await fileManager.DownloadFileAsync(request);

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


    public async Task DeleteFile()
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
                BlobContainerName = "demo-blazor8",
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




    #region Helpers

    internal async Task SaveFileAsync(string path, string fileName, string containerName, Progress<ProgressEventArgs> progressHandler)
    {
        using FileStream fileStream = File.OpenRead(path);

        var request = new SaveFileRequest
        {
            BlobContainerName = containerName,
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


    internal async Task SaveFileChunkedAsync(string path, string fileName, string containerName, Progress<ProgressEventArgs> progressHandler)
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
                BlobContainerName = containerName,
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
    internal async Task SaveFileStreamAsync(string path, string fileName, string containerName, Progress<ProgressEventArgs> progressHandler)
    {
        await using (var fileStream = File.OpenRead(path))

        using (var fileContent = new StreamContent(fileStream))
        using (var formData = new MultipartFormDataContent())

        formData.Add(fileContent, "files", $"{Guid.NewGuid()}.jpg");
    }

    #endregion Helpers

}
