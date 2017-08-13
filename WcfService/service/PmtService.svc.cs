using log4net.Config;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfService.util;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PmtService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PmtService.svc or PmtService.svc.cs at the Solution Explorer and start debugging.
    public class PmtService : IPmtService
    {
        private ADServiceHelper ash = new ADServiceHelper();

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
            string rlt = ash.SetEmpPassword(emp);
            //return emp.action + " [" + emp.empId + "] @ the " + Environment.MachineName;
            return rlt;
        }

        public Employee GetEmpInfo(string empId)
        {
            return ash.GetEmpInfo(empId);
        }
    }
}
