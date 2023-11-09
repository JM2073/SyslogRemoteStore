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
        public string TimeStamp { get; set; }

        List<Log> logs = new List<Log>();

        string path = AppDomain.CurrentDomain.BaseDirectory + "FileName.txt"; 

        public void CreateFile()
        {
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

                        string place = string.Format(@"{0}_{1}\n", log.SourceIp, log.TimeStamp);

                        Byte[] LogEntry = new UTF8Encoding(true).GetBytes(place);
                        fs.Write(LogEntry, 0, LogEntry.Length);

                    }


                }


                using (StreamReader sr = File.OpenText(path))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        //Console.WriteLine(s);
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