using SyslogRemoteStore.Web.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SyslogRemoteStore.Web.Services
{
    public class FileCreationService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string SourceIp { get; set; }
        public string Severity { get; set; }


        public void CreateFile(List<Log> logs, string fileName) //string filename Changes depending on name
        {

            string filename = string.Format(@"{0}.txt", fileName);
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

                        string logline = string.Format(@"{0}", log.FullMessage);

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

        public void ExportZip()
        {







        }

    }
}
