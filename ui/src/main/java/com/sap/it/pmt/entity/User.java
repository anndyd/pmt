package com.sap.it.pmt.entity;

import java.io.Serializable;

public class User implements Serializable {
    private static final long serialVersionUID = 185706989928100701L;

    private String userName;

    private String fullName;
    
    private boolean status;
    
    private String password;
    
    private String role;
    
	public String getUserName() {
		return userName;
	}

	public void setUserName(String userName) {
	    if (userName != null) {
	        userName = userName.toUpperCase();
        }
		this.userName = userName;
	}

	public String getPassword() {
		return password;
	}

	public String getFullName() {
		return fullName;
	}

	public void setFullName(String fullName) {
		this.fullName = fullName;
	}

	public boolean getStatus() {
		return status;
	}

	public void setStatus(boolean status) {
		this.status = status;
	}

	public void setPassword(String password) {
		this.password = password;
	}

	public String getRole() {
		return role;
	}

	public void setRole(String role) {
		this.role = role;
	}

}
