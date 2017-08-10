using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
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

        public string GetEmployeeFromAD(string empId)
        {
            using (var ctx = new PrincipalContext
         (ContextType.Domain,
          "global.corp.sap",
          "CN=I068054,OU=I,OU=Identities,DC=global,DC=corp,DC=sap",
          "i068054@global.corp.sap",
          "Carol990")) //set the super user in this section. 

            {
                using (var tstUser = UserPrincipal.FindByIdentity
                       (ctx, IdentityType.SamAccountName, "i068054"))  //set the destination user 
                {
                    //tstUser.ChangePassword("Carol990", "G$tao123");
                    tstUser.SetPassword("Initial1"); //the super user need full permission on OU
                }
            }
            return "";
        }
    }
}
