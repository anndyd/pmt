package com.sap.it.pmt.entity;

import java.io.Serializable;

public class Employee implements Serializable {
	private static final long serialVersionUID = 1081545087359714262L;

	private String empId;
	private String empFullname;
	private String mobile;
	private String smsCode;
	private String oldPassword;
	private String newPassword;
	private String action;
	
    public String getEmpId() {
		return empId;
	}
	public void setEmpId(String empId) {
		this.empId = empId;
	}
	public String getEmpFullname() {
		return empFullname;
	}
	public void setEmpFullname(String empFullname) {
		this.empFullname = empFullname;
	}
	public String getMobile() {
        return mobile;
    }
    public void setMobile(String mobile) {
        this.mobile = mobile;
    }
    public String getSmsCode() {
		return smsCode;
	}
	public void setSmsCode(String smsCode) {
		this.smsCode = smsCode;
	}
	public String getOldPassword() {
		return oldPassword;
	}
	public void setOldPassword(String oldPassword) {
		this.oldPassword = oldPassword;
	}
	public String getNewPassword() {
		return newPassword;
	}
	public void setNewPassword(String newPassword) {
		this.newPassword = newPassword;
	}
	public String getAction() {
		return action;
	}
	public void setAction(String action) {
		this.action = action;
	}
    
    
}
