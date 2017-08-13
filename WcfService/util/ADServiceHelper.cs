using log4net;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WcfService.util
{
    public class ADServiceHelper
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static string AD_DOMAIN = "global.corp.sap";
        private static string AD_PORT = ":389";
        private static string AD_PATH = "LDAP://OU=Identities,DC=global,DC=corp,DC=sap";
        private static string PWR_USR = "";
        private static string PWR_PWD = "";

        public Employee GetEmpInfo(string empId)
        {
            Employee emp = new Employee();
            try
            {
                log.Info("Get employee info: " + empId);
                DirectoryEntry myLdapConnection = createDirectoryEntry();
                DirectorySearcher search = new DirectorySearcher(myLdapConnection);
                search.Filter = "(cn=" + empId + ")";

                // create an array of properties that we would like and  
                // add them to the search object  
                string[] requiredProperties = new string[] { "displayname", "mobile", "mail", "department" };
                foreach (String property in requiredProperties)
                {
                    search.PropertiesToLoad.Add(property);
                }
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    emp.empId = empId;
                    emp.empFullname = result.Properties["displayname"][0].ToString();
                    emp.mobile = result.Properties["mobile"][0].ToString();
                    emp.mail = result.Properties["mail"][0].ToString();
                    emp.department = result.Properties["department"][0].ToString();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception when get employee information from AD:", e);
            }
            return emp;
        }

        public string SetEmpPassword(Employee emp)
        {
            string rlt = "fail";
            try
            {
                DirectoryEntry myLdapConnection = createDirectoryEntry();
                DirectorySearcher search = new DirectorySearcher(myLdapConnection);
                search.Filter = "(cn=" + emp.empId + ")";
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    UpdateEmpInfo(emp, result.GetDirectoryEntry());
                }
                rlt = "success";
            }
            catch (Exception e)
            {
                log.Error("Exception when set employee password:", e);
                // The specified network password is not correct. (Exception from HRESULT: 0x80070056)
                if (e.InnerException.HResult.Equals(0x80070056))
                {
                    rlt = "wrong password";
                }
            }
            return rlt;
        }

        private void UpdateEmpInfo(Employee emp, DirectoryEntry userEntry)
        {
            if (userEntry != null)
            {
                switch (emp.action)
                {
                    case "change":
                        emp.newPassword = StringHelper.RandomString(8);
                        userEntry.Invoke("ChangePassword", new object[] { emp.oldPassword, emp.newPassword });
                        break;
                    case "unlock":
                        userEntry.Properties["LockOutTime"].Value = 0;
                        break;
                    case "lock":
                        userEntry.InvokeSet("IsAccountLocked", true);
                        break;
                    case "reset":
                        userEntry.Invoke("SetPassword", emp.newPassword);
                        break;
                }
                userEntry.CommitChanges();
            }
        }

        private DirectoryEntry createDirectoryEntry(string user, string password)
        {
            log.Info("Connect to AD with power user.");
            DirectoryEntry ldapConnection = createDirectoryEntry();
            ldapConnection.Username = user;
            ldapConnection.Password = password;
            return ldapConnection;
        }
        private DirectoryEntry createDirectoryEntry()
        {
            log.Info("Connect to AD.");
            // create and return new LDAP connection with desired settings  
            DirectoryEntry ldapConnection = new DirectoryEntry(AD_DOMAIN + AD_PORT);
            ldapConnection.Path = AD_PATH;
            ldapConnection.AuthenticationType = AuthenticationTypes.Secure;

            return ldapConnection;
        }
        private string GetEmployeeFromAD(string empId)
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