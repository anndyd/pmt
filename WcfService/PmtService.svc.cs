using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PmtService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PmtService.svc or PmtService.svc.cs at the Solution Explorer and start debugging.
    public class PmtService : IPmtService
    {
        public void DoWork()
        {
        }

        void IPmtService.DoWork()
        {
            throw new NotImplementedException();
        }

        string IPmtService.Health()
        {
            return "The response from: " + Environment.MachineName;
        }

        string IPmtService.DoAction(Employee emp)
        {
            return emp.action + " [" + emp.empId + "] @ the " + Environment.MachineName;
        }
    }
}
