namespace SyslogRemoteStore.Web.Models
{
    public interface IFileZip
    {
        void ZipFiles(string zipFilePath, List<string> filesToZip);
    }
}