sap.ui.define([ 'jquery.sap.global',
		"sap/it/pmt/ui/view/base/BaseController",
		"sap/it/pmt/ui/service/UserService",
		"sap/m/MessageToast",
		'sap/ui/model/json/JSONModel' ], function(jQuery, 
		BaseController, UserService, MessageToast, JSONModel) {
	"use strict";
	var us = new UserService();
	var txtInterval;
	return BaseController.extend("sap.it.pmt.ui.view.Main", {

		onInit : function(oEvent) {
			var that = this;
			that._showFormFragment();
			
			var oModel = new JSONModel({
				empId : util.sessionInfo.currentUser,
				empFullname : "",
				smsCode : "",
				oldPassword : "",
				newPassword : "",
				action : ""
			});
			
			that.getView().setModel(oModel);
			var nm = that.getResourceBundle().getText("nonMatch");
			
			var hModel = new JSONModel({
				HTML : "<font color=\"red\">" + nm + "</font>"
			});
			that.getView().setModel(hModel, "nm");
			that.getView().bindElement("/");
			that.getView().bindElement("nm>/");
		},
		
		onExit : function() {
  		  if (this.oPageFragment) {
  		    this.oPageFragment.destroy();
  		  }
		},
		
		onPasswordChange : function(evt) {
		    var v = evt.getParameters().value;
		    if (v && v.length > 0) {
		    	var eq = this.getView().byId("newPassword").getValue() === this.getView().byId("confirmPassword").getValue();
		    	this.getView().byId("prompt").setVisible(!eq);
		    	this.getView().byId("change").setEnabled(eq);
		    }
		},
		onPress : function(evt) {
//			if (txtInterval) {
//				clearInterval(txtInterval);
//			}
			var that = this;
			var param = that.getView().getModel().getData();
			param.action = evt.getSource().getId().match("((?!-).)+$")[0];
			us.doAction(param).done(
					function(data) {
						MessageToast.show(
								data
//								that.getResourceBundle().getText("resetDone")
								);
					}
			);
		},
		onPressSms : function(evt) {
			var btn1 = this.getView().byId("sms1");
			var btn2 = this.getView().byId("sms2");
			var btn3 = this.getView().byId("sms3");
			btn1.setEnabled(false);
			btn2.setEnabled(false);
			btn3.setEnabled(false);
			var txtL = this.getResourceBundle().getText("smsLiveText");
			var txtB = this.getResourceBundle().getText("smsButton");
			var i = 10;
			txtInterval = setInterval(function(){
				i--;
				btn1.setText(String.format(txtL, i));
				btn2.setText(String.format(txtL, i));
				btn3.setText(String.format(txtL, i));
				if (i<=0) {
					btn1.setEnabled(true);
					btn1.setText(txtB);
					btn2.setEnabled(true);
					btn2.setText(txtB);
					btn3.setEnabled(true);
					btn3.setText(txtB);
					clearInterval(txtInterval);
				}
			}, 1000);
		},
		

		_showFormFragment : function () {
			var oPage = this.getView().byId("mainPage");
			oPage.destroyContent();
			util.sessionInfo.mobile = "123";
			if (util.sessionInfo.mobile) {
				this.oPageFragment = sap.ui.xmlfragment(this.getView().getId(), "sap.it.pmt.ui.view.fragment.PmtMain", this);
			} else {
				this.oPageFragment = sap.ui.xmlfragment(this.getView().getId(), "sap.it.pmt.ui.view.fragment.PmtNoPhone", this);
			}
			oPage.addContent(this.oPageFragment);
		}

	});

});