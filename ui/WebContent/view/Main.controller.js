sap.ui.define([ 'jquery.sap.global',
		"sap/it/pmt/ui/view/base/BaseController",
		"sap/it/pmt/ui/service/UserService",
		"sap/m/MessageToast",
		'sap/ui/model/json/JSONModel' ], function(jQuery, 
		BaseController, UserService, MessageToast, JSONModel) {
	"use strict";
	var us = new UserService();
	return BaseController.extend("sap.it.pmt.ui.view.Main", {

		onInit : function(oEvent) {
			var that = this;
			var oModel = new JSONModel({
				empId : util.sessionInfo.currentUser,
				empFullname : "",
				smsCode : "",
				oldPassword : "",
				newPassword : "",
				action : ""
			});
			
			that.getView().setModel(oModel);
			var nm = that.getOwnerComponent().getModel("i18n").getResourceBundle().getText("nonMatch")
			
			var hModel = new JSONModel({
				HTML : "<font color=\"red\">" + nm + "</font>"
			});
			that.getView().setModel(hModel, "nm");
			that.getView().bindElement("/");
			that.getView().bindElement("nm>/");
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
		}
	});

});