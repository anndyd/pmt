package com.sap.it.pmt.controller;


import java.security.Principal;
import java.security.cert.X509Certificate;

import javax.servlet.http.HttpServletRequest;

import org.apache.log4j.Logger;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Scope;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.sap.it.pmt.dto.SessionInfo;
import com.sap.it.pmt.entity.User;
import com.sap.it.pmt.util.SessionHolder;

@Controller
@RequestMapping("user")
@Scope("request")
public class UserController {
	private static final Logger LOGGER = Logger.getLogger(UserController.class);
	
	@Autowired(required=true)
	private HttpServletRequest request;	
	
	@RequestMapping(value="/health", method = RequestMethod.GET)
	@ResponseBody
	public String checkHealth(){
		return "Server is on...";
	}

    @RequestMapping(value = "/active", method = RequestMethod.POST)
    @ResponseBody
    public SessionInfo active(HttpServletRequest req) {
    	SessionInfo rlt = new SessionInfo();
    	// Get the client SSL certificates associated with the request
		X509Certificate[] certs = (X509Certificate[]) req.getAttribute("javax.servlet.request.X509Certificate");
		// Check that a certificate was obtained
		if (certs.length < 1) {
			rlt.setError("SSL not client authenticated");
		}
		// The base of the certificate chain contains the client's info
		X509Certificate principalCert = certs[0];

		// Get the Distinguished Name from the certificate
		// CN = I063098	O = SAP-AG	C = DE
		Principal principal = principalCert.getSubjectDN();

		// Extract the common name (CN)
		int start = principal.getName().indexOf("CN");
		String usrId = null;
		String usrFullName = "";
		String usrRole = "";
		String tmpName = "";
		if (start > -1) {
			tmpName = principal.getName().substring(start + 3);
			int end = tmpName.indexOf(",");
			if (end > 0) {
				usrId = tmpName.substring(0, end);
			} else {
				usrId = tmpName;
			}
		}
		User usr = new User();
		if (usr != null /*&& usr.getStatus()*/) {
    		usrFullName = usr.getFullName();
    		usrRole = usr.getRole();
    		rlt.setUserFullName(usrFullName != null && usrFullName.length() > 0 ? 
    				usrId + " (" + usrFullName + ")" : usrId);
    		rlt.setCurrentUser(usrId);
    		rlt.setRole(usrRole);
    	}
        req.getSession().setAttribute(SessionHolder.USER_ID, usrId);
        req.getSession().setAttribute(SessionHolder.USER_FULLNAME, usrFullName);
        req.getSession().setAttribute(SessionHolder.USER_ROLE, usrRole);

        SessionHolder.setContext(usrId, usrFullName, usrRole);
        
        return rlt;
    }

}
