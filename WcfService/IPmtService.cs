using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPmtService" in both code and config file together.
    [ServiceContract]
    public interface IPmtService
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "health")]
        string Health();

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "")]
        string DoAction(Employee emp);
    }
}
