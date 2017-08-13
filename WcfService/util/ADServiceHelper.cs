using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;

namespace WcfService.util
{
    public class ADServiceHelper
    {
        private static string AD_DOMAIN = "global.corp.sap";
        private static string AD_PORT = ":636";
        private static string AD_PATH = "LDAP://OU=Identities,DC=global,DC=corp,DC=sap";
        private static string PWR_USR = "";

        private Employee GetEmpInfo(string empId)
        {
            Employee emp = new Employee();
            try
            {
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
                Console.WriteLine("Exception caught:\n\n" + e.ToString());
            }
            return emp;
        }

        private DirectoryEntry createDirectoryEntry(string user, string password)
        {
            DirectoryEntry ldapConnection = createDirectoryEntry();
            ldapConnection.Username = user;
            ldapConnection.Password = password;
            return ldapConnection;
        }
            private DirectoryEntry createDirectoryEntry()
        {
            // create and return new LDAP connection with desired settings  
            DirectoryEntry ldapConnection = new DirectoryEntry(AD_DOMAIN + AD_PORT);
            ldapConnection.Path = AD_PATH;
            ldapConnection.AuthenticationType = AuthenticationTypes.Secure;

            return ldapConnection;
        }

    }
}