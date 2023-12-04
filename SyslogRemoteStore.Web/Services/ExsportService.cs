using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Text;
using SyslogRemoteStore.Web.Models;

namespace SyslogRemoteStore.Web.Services;

public class ExsportService
{
    public MemoryStream ZipFiles(List<IT6S3> radios)
    {
        List<string> filesToZip = new();

        foreach (IGrouping<string, IT6S3> radioIpGroups in radios.GroupBy(x => x.GetFormatedIp()))
        {
            foreach (IT6S3 radio in radioIpGroups)
            {
                string filePath = CreateFile(radio.Logs, $"{radio.GetFormatedIp()}:{radio.Port}");
                filesToZip.Add(filePath);
            }
        }

        string FileName = "SyslogFiles.zip";
        string zipFilePath = string.Format(@"{0}{1}", AppDomain.CurrentDomain.BaseDirectory, FileName);


        using (FileStream createZip = new(zipFilePath, FileMode.Create))
        {
            using (ZipArchive archive = new(createZip, ZipArchiveMode.Create))
            {
                // Add each file to the zip archive
                foreach (string path in filesToZip)
                    if (File.Exists(path))
                    {
                        string entryName = Path.GetFileName(path);
                        archive.CreateEntryFromFile(path, entryName);
                    }
                    else
                    {
                        Console.WriteLine($"File not found: {path}");
                    }
            }
        }

        foreach (string file in filesToZip) File.Delete(file);

        byte[] binaryData = File.ReadAllBytes(zipFilePath);

        return new MemoryStream(binaryData);
    }

    private string CreateFile(ObservableCollection<Log> logs, string Ip) //port 
    {
        DateTime currentDateTime = DateTime.Now;
        string dateTime = currentDateTime.ToString("yyyyMMddHHmmss");

        string filename = string.Format(@"{0}_{1}.txt", Ip, dateTime);
        string path = string.Format(@"{0}{1}", AppDomain.CurrentDomain.BaseDirectory, filename);

        try
        {
            if (File.Exists(path)) File.Delete(path);

            using (FileStream fs = File.Create(path))
            {
                foreach (Log log in logs)
                {
                    string logline = string.Format(@"{0}", log.FullMessage);

                    byte[] LogEntry = new UTF8Encoding(true).GetBytes(logline);
                    fs.Write(LogEntry, 0, LogEntry.Length);
                    byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                    fs.Write(newline, 0, newline.Length);
                }
            }

        }
        catch (Exception Exept)
        {
            Console.WriteLine(Exept.ToString());
            path = string.Empty;
        }

        return path;
    }
}