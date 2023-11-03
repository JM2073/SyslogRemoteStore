using System.Net.Sockets;
using SyslogRemoteStore.Web.Models;

namespace SyslogRemoteStore.Web.ViewModels;

public interface IRadioViewModel
{
    IT6S3 Radio { get; set; }
    string RadioId { get; set; }
}