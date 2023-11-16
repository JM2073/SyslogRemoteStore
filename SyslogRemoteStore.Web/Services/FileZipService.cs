using Microsoft.AspNetCore.Routing.Constraints;
using SyslogRemoteStore.Web.Models;
using System.IO;
using System.IO.Compression;

namespace SyslogRemoteStore.Web.Services
{
    public class FileZipService : IFileZip
    {
        public void RenameFile(string path, string ip)
        {
            DateTime currentDateTime = DateTime.Now;
            string dateTime = currentDateTime.ToString("yyyyMMddHHmmssfff");
            string filename = string.Format(@"{0}_{1}.txt", ip, dateTime);
            string NewFilename = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory,filename);


            try
            {
                if (File.Exists(path))
                {
                    File.Move(path, NewFilename);
                }
                else
                {

                }
            }
            catch (Exception Exept)
            {
                Console.WriteLine(Exept.ToString());
            }


        }

        public void ZipFiles(string zipPath, List<string> filesToZip)
        {
            // Create the zip file
            using (FileStream createZip = new FileStream(zipPath, FileMode.Create))
            {
                using (ZipArchive zip = new ZipArchive(createZip, ZipArchiveMode.Create))
                {
                    // Add each file to the zip archive
                    foreach (string path in filesToZip)
                    {
                        if (File.Exists(path))
                        {
                            string entryName = Path.GetFileName(path);
                            zip.CreateEntryFromFile(path, entryName, CompressionLevel.Optimal);
                        }
                        else
                        {
                     
                        }
                        
                    }
                    zip.Dispose();
                }
            }
        }
    }
}
