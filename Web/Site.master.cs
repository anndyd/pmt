using System;
using System.Linq;
using System.Web;

public partial class Site : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            UserName = string.Empty;
            string userInfo = HttpContext.Current.Request.Headers["X-SSL-Client-S-DN"].ToLower();
            if (!string.IsNullOrEmpty(userInfo))
            {
                UserName = userInfo.Split("CN=".ToCharArray()).Last();
            }
            //PageUtil.GetLoginResultByUserId(userId);
        }
        catch (Exception)
        {
        }
    }

    public String UserName
    {
        get { return (String)ViewState["userName"]; }
        set { ViewState["userName"] = value; }
    }

}
