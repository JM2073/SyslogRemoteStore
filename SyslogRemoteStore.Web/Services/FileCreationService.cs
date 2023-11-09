using SyslogRemoteStore.Web.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SyslogRemoteStore.Web.Services
{
    public class FileCreationService 
    {
        public void CreateFile(List<Log> logs, string fn) //string filename Changes depending on name
        {
            string filename = string.Format(@"{0}.txt",fn);
            string path = String.Format(@"{0}{1}", AppDomain.CurrentDomain.BaseDirectory, filename);

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (FileStream fs = File.Create(path))
                {

                    foreach (var log in logs)
                    {

                        string logline = string.Format(@"{0} {1} {2} {3} {4} {5} {6}", log.SourceIp, log.SourceItem, log.Facilty, log.Severity, log.TimeStamp, log.Tag, log.Message);

                        Byte[] LogEntry = new UTF8Encoding(true).GetBytes(logline);
                        fs.Write(LogEntry, 0, LogEntry.Length);
                        byte[] newline = UTF8Encoding.ASCII.GetBytes(Environment.NewLine);
                        fs.Write(newline, 0, newline.Length);

                    }

                }
            }
            catch (Exception Exept)
            {
                Console.WriteLine(Exept.ToString());
            }

        }
    }
    
}