using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfService
{
    public class Employee
    {
        string empId;
        string empFullname;
        string smsCode;
        string oldPassword;
        string newPassword;
        string action;

        public string EmpId
        {
            get
            {
                return empId;
            }

            set
            {
                empId = value;
            }
        }

        public string EmpFullname
        {
            get
            {
                return empFullname;
            }

            set
            {
                empFullname = value;
            }
        }

        public string SmsCode
        {
            get
            {
                return smsCode;
            }

            set
            {
                smsCode = value;
            }
        }

        public string OldPassword
        {
            get
            {
                return oldPassword;
            }

            set
            {
                oldPassword = value;
            }
        }

        public string NewPassword
        {
            get
            {
                return newPassword;
            }

            set
            {
                newPassword = value;
            }
        }

        public string Action
        {
            get
            {
                return action;
            }

            set
            {
                action = value;
            }
        }
    }
}