namespace SyslogRemoteStore.Web.Models;

public class WebAppState
{
    public bool NavState = false;

    public WebAppState(){}

	public void UpdateNavState()
	{
        NavState = NavState switch
        {
            false => true,
            _ => false,
        };
    }

}
