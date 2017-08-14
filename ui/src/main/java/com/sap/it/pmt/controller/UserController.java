package com.sap.it.pmt.controller;


import java.security.Principal;
import java.security.cert.X509Certificate;

import javax.servlet.http.HttpServletRequest;

import org.apache.log4j.Logger;
import org.springframework.context.annotation.Scope;
import org.springframework.http.HttpMethod;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.sap.it.pmt.dto.SessionInfo;
import com.sap.it.pmt.entity.Employee;
import com.sap.it.pmt.util.RestClientHelper;
import com.sap.it.pmt.util.SessionHolder;

@Controller
@RequestMapping("user")
@Scope("request")
public class UserController {
	private static final Logger LOGGER = Logger.getLogger(UserController.class);
	private final RestClientHelper restClient = new RestClientHelper();
	
	@RequestMapping(value="/health", method = RequestMethod.GET)
	@ResponseBody
	public String checkHealth(){
		return "Server is on...";
	}
	
	@RequestMapping(value="/pmthealth", method = RequestMethod.GET)
	@ResponseBody
	public String checkPmtHealth(){
		return restClient.execute("health", HttpMethod.GET, null, String.class);
	}

    @RequestMapping(value = "/", method = RequestMethod.POST)
    @ResponseBody
    public String doAction(@RequestBody Employee emp) {
    	return restClient.execute("", HttpMethod.POST, emp, String.class);
    }

    @RequestMapping(value = "/active", method = RequestMethod.POST)
    @ResponseBody
    public SessionInfo active(HttpServletRequest req, @RequestBody String empId) {
    	SessionInfo rlt = new SessionInfo();
		String usrId = "";
		String usrFullName = "";
		String usrRole = "";
    	Employee emp = restClient.execute("getEmpInfo/" + empId, HttpMethod.GET, null, Employee.class);
    	if (null == emp) {
    	    rlt.setError("Can't get your information!");
    	} else {
    	    usrId = emp.getEmpId();
    		usrFullName = emp.getEmpFullname();
    		rlt.setUserFullName(usrFullName != null && usrFullName.length() > 0 ? 
    				usrId + " (" + usrFullName + ")" : usrId);
    		rlt.setCurrentUser(usrId);
    		rlt.setMobile(emp.getMobile());
    
    		req.getSession().setAttribute(SessionHolder.USER_ID, usrId);
            req.getSession().setAttribute(SessionHolder.USER_FULLNAME, usrFullName);
    	}
        SessionHolder.setContext(usrId, usrFullName, usrRole);
        
        return rlt;
    }
    
    private String getEmpIdFromCert(HttpServletRequest req) {
        String rlt = "";
    	// Get the client SSL certificates associated with the request
		X509Certificate[] certs = (X509Certificate[]) req.getAttribute("javax.servlet.request.X509Certificate");
		// Check that a certificate was obtained
		if (null == certs || certs.length < 1) {
			rlt = "SSL not client authenticated";
			return rlt;
		}
		// The base of the certificate chain contains the client's info
		X509Certificate principalCert = certs[0];

		// Get the Distinguished Name from the certificate
		// CN = I063098	O = SAP-AG	C = DE
		Principal principal = principalCert.getSubjectDN();

		// Extract the common name (CN)
		int start = principal.getName().indexOf("CN");
		String tmpName = "";
		if (start > -1) {
			tmpName = principal.getName().substring(start + 3);
			int end = tmpName.indexOf(",");
			if (end > 0) {
				rlt = tmpName.substring(0, end);
			} else {
				rlt = tmpName;
			}
		}
        return rlt;
    }

}
