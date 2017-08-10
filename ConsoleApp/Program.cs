using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter user: ");
            String username = Console.ReadLine();
            //***** System.DirectoryServices Examples ************/
            try
            {
                // create LDAP connection object  
                DirectoryEntry myLdapConnection = createDirectoryEntry();
                // create search object which operates on LDAP connection object  
                // and set search object to only find the user specified  
                DirectorySearcher search = new DirectorySearcher(myLdapConnection);
                //search.Filter = "(cn=" + username + ")";
                search.Filter = "(&(objectClass=user)(sAMAccountName=" + username + "))"; // result same as above.
                // create results objects from search object  
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    // user exists, cycle through LDAP fields (cn, telephonenumber etc.)  
                    ResultPropertyCollection fields = result.Properties;

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\work\it\tmp\testad1.txt"))
                    {
                        foreach (String ldapField in fields.PropertyNames)
                        {
                            // cycle through objects in each field e.g. group membership  
                            // (for many fields there will only be one object such as name)  
                            foreach (Object myCollection in fields[ldapField])
                            {
                                String line = String.Format("{0,-20} : {1}", ldapField, myCollection.ToString());
                                //Console.WriteLine(line);
                                file.WriteLine(line);
                            }
                        }
                    }

                }
                else
                {
                    // user does not exist  
                    Console.WriteLine("User not found!");
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception caught:\n\n" + e.ToString());
            }
        }

        static DirectoryEntry createDirectoryEntry()
        {
            // create and return new LDAP connection with desired settings  
            DirectoryEntry ldapConnection = new DirectoryEntry("global.corp.sap:636");
            ldapConnection.Path = "LDAP://OU=Identities,DC=global,DC=corp,DC=sap";
            ldapConnection.AuthenticationType = AuthenticationTypes.Secure;

            return ldapConnection;
        }

        // Retrieving Selected Information From a User’s Record
        static void GetSelectedInfo(string username)
        {
            try
            {
                DirectoryEntry myLdapConnection = createDirectoryEntry();
                DirectorySearcher search = new DirectorySearcher(myLdapConnection);
                search.Filter = "(cn=" + username + ")";

                // create an array of properties that we would like and  
                // add them to the search object  

                string[] requiredProperties = new string[] { "cn", "postofficebox", "mail" };

                foreach (String property in requiredProperties)
                    search.PropertiesToLoad.Add(property);

                SearchResult result = search.FindOne();

                if (result != null)
                {
                    foreach (String property in requiredProperties)
                        foreach (Object myCollection in result.Properties[property])
                            Console.WriteLine(String.Format("{0,-20} : {1}",
                                          property, myCollection.ToString()));
                }

                else Console.WriteLine("User not found!");
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception caught:\n\n" + e.ToString());
            }
        }

        // Retrieving Information for All Users
        static void GetInfo4AllUsers(string property)
        {
            try
            {
                DirectoryEntry myLdapConnection = createDirectoryEntry();

                DirectorySearcher search = new DirectorySearcher(myLdapConnection);
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add(property);

                SearchResultCollection allUsers = search.FindAll();

                foreach (SearchResult result in allUsers)
                {
                    if (result.Properties["cn"].Count > 0 && result.Properties[property].Count > 0)
                    {
                        Console.WriteLine(String.Format("{0,-20} : {1}",
                                      result.Properties["cn"][0].ToString(),
                                      result.Properties[property][0].ToString()));
                    }
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception caught:\n\n" + e.ToString());
            }
        }

        // Updating a User
        static void UpdateUser(string username)
        {
            try
            {
                DirectoryEntry myLdapConnection = createDirectoryEntry();

                DirectorySearcher search = new DirectorySearcher(myLdapConnection);
                search.Filter = "(cn=" + username + ")";
                search.PropertiesToLoad.Add("title");

                SearchResult result = search.FindOne();
                if (result != null)
                {
                    // create new object from search result  
                    DirectoryEntry entryToUpdate = result.GetDirectoryEntry();
                    // show existing title  
                    Console.WriteLine("Current title   : " +
                                      entryToUpdate.Properties["title"][0].ToString());
                    Console.Write("\n\nEnter new title : ");
                    // get new title and write to AD  
                    String newTitle = Console.ReadLine();
                    entryToUpdate.Properties["title"].Value = newTitle;
                    entryToUpdate.CommitChanges();
                    Console.WriteLine("\n\n...new title saved");
                }
                else Console.WriteLine("User not found!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught:\n\n" + e.ToString());
            }
        }

        // Adding a New User
        static void AddUser()
        {
            // connect to LDAP  
            DirectoryEntry myLdapConnection = createDirectoryEntry();
            // define vars for user  
            String domain = "leeds-art.ac.uk";
            String first = "Test";
            String last = "User";
            String description = ".NET Test";
            object[] password = { "12345678" };
            String[] groups = { "Staff" };
            String username = first.ToLower() + last.Substring(0, 1).ToLower();
            String homeDrive = "H:";
            String homeDir = @"\\gonzo.leeds-art.ac.uk\data3\USERS\" + username;

            // create user  
            try
            {
                if (createUser(myLdapConnection, domain, first, last, description,
                         password, groups, username, homeDrive, homeDir, true) == 0)
                {
                    Console.WriteLine("Account created!");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Problem creating account :(");
                    Console.ReadLine();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception caught:\n\n" + e.ToString());
                Console.ReadLine();
            }
        }

        static int createUser(DirectoryEntry myLdapConnection, String domain, String first,
                              String last, String description, object[] password,
                              String[] groups, String username, String homeDrive,
                              String homeDir, bool enabled)
        {
            // create new user object and write into AD  
            DirectoryEntry user = myLdapConnection.Children.Add(
                                  "CN=" + first + " " + last, "user");
            // User name (domain based)   
            user.Properties["userprincipalname"].Add(username + "@" + domain);
            // User name (older systems)  
            user.Properties["samaccountname"].Add(username);
            // Surname  
            user.Properties["sn"].Add(last);
            // Forename  
            user.Properties["givenname"].Add(first);
            // Display name  
            user.Properties["displayname"].Add(first + " " + last);
            // Description  
            user.Properties["description"].Add(description);
            // E-mail  
            user.Properties["mail"].Add(first + "." + last + "@" + domain);
            // Home dir (drive letter)  
            user.Properties["homedirectory"].Add(homeDir);
            // Home dir (path)  
            user.Properties["homedrive"].Add(homeDrive);
            user.CommitChanges();
            // set user's password  
            user.Invoke("SetPassword", password);
            // enable account if requested (see http://support.microsoft.com/kb/305144 for other codes)   
            if (enabled)
                user.Invoke("Put", new object[] { "userAccountControl", "512" });

            // add user to specified groups  
            foreach (String thisGroup in groups)
            {
                DirectoryEntry newGroup = myLdapConnection.Parent.Children.Find(
                                          "CN=" + thisGroup, "group");
                if (newGroup != null)
                    newGroup.Invoke("Add", new object[] { user.Path.ToString() });
            }
            user.CommitChanges();

            // make home folder on server  
            Directory.CreateDirectory(homeDir);
            // set permissions on folder, we loop this because if the program  
            // tries to set the permissions straight away an exception will be  
            // thrown as the brand new user does not seem to be available, it takes  
            // a second or so for it to appear and it can then be used in ACLs  
            // and set as the owner  
            bool folderCreated = false;

            while (!folderCreated)
            {
                try
                {
                    // get current ACL  
                    DirectoryInfo dInfo = new DirectoryInfo(homeDir);
                    DirectorySecurity dSecurity = dInfo.GetAccessControl();
                    // Add full control for the user and set owner to them  
                    IdentityReference newUser = new NTAccount(domain + @"\" + username);
                    dSecurity.SetOwner(newUser);
                    FileSystemAccessRule permissions =
                       new FileSystemAccessRule(newUser, FileSystemRights.FullControl,
                                                AccessControlType.Allow);
                    dSecurity.AddAccessRule(permissions);
                    // Set the new access settings.  
                    dInfo.SetAccessControl(dSecurity);
                    folderCreated = true;
                }
                catch (System.Security.Principal.IdentityNotMappedException)
                {
                    Console.Write(".");
                }
                catch (Exception ex)
                {
                    // other exception caught so not problem with user delay as   
                    // commented above  
                    Console.WriteLine("Exception caught:" + ex.ToString());
                    return 1;
                }
            }

            return 0;
        }

        // ********** System.DirectoryServices.AccountManagement Examples
        // Retrieving Information From a User’s Record
        static void GetUserInfo()
        {
            try
            {
                // enter AD settings  
                PrincipalContext AD = new PrincipalContext(ContextType.Domain, "leeds-art.ac.uk");

                // create search user and add criteria  
                Console.Write("Enter logon name: ");
                UserPrincipal u = new UserPrincipal(AD);
                u.SamAccountName = Console.ReadLine();

                // search for user  
                PrincipalSearcher search = new PrincipalSearcher(u);
                UserPrincipal result = (UserPrincipal)search.FindOne();
                search.Dispose();

                // show some details  
                Console.WriteLine("Display Name : " + result.DisplayName);
                Console.WriteLine("Phone Number : " + result.VoiceTelephoneNumber);
            }

            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        // Retrieving Info From All Users Again
        static void GetAllUserInfo()
        {
            try
            {
                PrincipalContext AD = new PrincipalContext(ContextType.Domain, "leeds-art.ac.uk");
                UserPrincipal u = new UserPrincipal(AD);
                PrincipalSearcher search = new PrincipalSearcher(u);

                foreach (UserPrincipal result in search.FindAll())
                    if (result.VoiceTelephoneNumber != null)
                        Console.WriteLine("{0,30} {1} ", result.DisplayName, result.VoiceTelephoneNumber);

                search.Dispose();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

        }

        // Using Both Libraries
        static void UseBoth()
        {
                      try  
            {  
                PrincipalContext  AD     = new PrincipalContext(ContextType.Domain, "leeds-art.ac.uk");  
                UserPrincipal     u      = new UserPrincipal(AD);  
                PrincipalSearcher search = new PrincipalSearcher(u);  
  
                foreach (UserPrincipal result in search.FindAll())  
                {  
                    if (result.VoiceTelephoneNumber != null)  
                    {  
                        DirectoryEntry lowerLdap = (DirectoryEntry)result.GetUnderlyingObject();  
  
                        Console.WriteLine("{0,30} {1} {2}",   
                            result.DisplayName,   
                            result.VoiceTelephoneNumber,  
                            lowerLdap.Properties["postofficebox"][0].ToString());  
                    }  
                }  
  
                search.Dispose();                
            }  
  
            catch (Exception e)  
            {  
                Console.WriteLine("Error: " + e.Message);  
            }  

        }

        // ssssssssssssssssssss access with LDAPS
        static Boolean isUserCredentialsValid(string username, string password)
        {
            const int ldapErrorInvalidCredentials = 0x31;
            string activeDirectoryServer = "your.ad.server:636";
            string activeDirectpryDomain = "your.ad.server";
            try
            {
                using (var ldapConnection = new LdapConnection(activeDirectoryServer))
                {
                    var networkCredential = new NetworkCredential(username, password, activeDirectpryDomain);
                    ldapConnection.SessionOptions.SecureSocketLayer = true;
                    ldapConnection.AuthType = AuthType.Negotiate;
                    ldapConnection.Bind(networkCredential);
                }
                // If the bind succeeds, the credentials are valid
                return true;
            }
            catch (LdapException ldapException)
            {
                // Invalid credentials throw an exception with a specific error code
                if (ldapException.ErrorCode.Equals(ldapErrorInvalidCredentials))
                {
                    return false;
                }
                throw;
            }
        }

        /////////////
        private LdapConnection CreateConnection()
        {
            string _domainAndUserName = null;
            string _pwd = null;
            try
            {
                var con = new LdapConnection(new LdapDirectoryIdentifier(ConfigurationSettings.AppSettings["ServerName"].ToString()));
                con.SessionOptions.SecureSocketLayer = true;
                con.SessionOptions.ProtocolVersion = 3;
                con.SessionOptions.VerifyServerCertificate = new VerifyServerCertificateCallback(ServerCallback);
                con.Credential = new NetworkCredential(_domainAndUserName, _pwd);
                con.AuthType = AuthType.Basic;
                con.Timeout = new TimeSpan(1, 0, 0);
                return con;
            }
            catch (LdapException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public bool ServerCallback(LdapConnection connection, X509Certificate certificate)
        {
            //...return true / false;
            return true;
        }
        public bool LDAPSAuthenticate(String username, String pwd)
        {
            LdapConnection con = CreateConnection();
            username = username.Trim();
            try
            {
                con.Bind();
            }
            catch (LdapException ex)
            {
                throw new LdapException(ex.Message);
            }
            catch (DirectoryOperationException ex)
            {
                throw new DirectoryOperationException(ex.Message);
            }

            try
            {
                string UsersDN = null;
                SearchRequest request = new SearchRequest(
                    UsersDN,
                    "(&(objectClass=person)(SAMAccountName=" + username + "))",
                    System.DirectoryServices.Protocols.SearchScope.Subtree
                    );
                SearchResponse response = (SearchResponse)con.SendRequest(request);

                if (response.Entries.Count == 0)
                {
                    return false;
                }
                else
                {
                    SearchResultEntry entry = response.Entries[0];
                    string dn = entry.DistinguishedName;
                    con.Credential = new NetworkCredential(dn, pwd);
                    con.Bind();
                    return true;
                }
            }
            catch (DirectoryOperationException ex)
            {
                throw new DirectoryOperationException(ex.Message);
            }
            catch (LdapException ex)
            {
                throw new LdapException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new LdapException(ex.Message);
            }
        }

    }
}

