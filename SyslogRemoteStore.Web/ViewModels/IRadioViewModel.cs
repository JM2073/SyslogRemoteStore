using System.Net.Sockets;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Stores;

namespace SyslogRemoteStore.Web.ViewModels;

public interface IRadioViewModel
{
    IT6S3 Radio { get; set; }
    string RadioId { get; set; }
    ConfigurationStore _configurationStore { get; }

}