using SyslogRemoteStore.Web.Models;
using System;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Net.Mime;
using System.Text;

namespace SyslogRemoteStore.Web.Services
{
    public class FileZipService : IFileZip
    {






        public void ZipFiles(string zipPath, List<string> filesToZip)
        {
            // Create the zip file
            using (FileStream createZip = new FileStream(zipPath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(createZip, ZipArchiveMode.Create))
                {
                    // Add each file to the zip archive
                    foreach (string path in filesToZip)
                    {
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
            }
        }

    }
}
