sap.ui.define([
		"sap/it/pmt/ui/view/base/BaseController",
		"sap/ui/model/json/JSONModel"
	], function (BaseController, JSONModel) {
		"use strict";
		var userName = '';
		var login = function () {
		    var url = "/user/active";
		    $.ajax({
		        url: url,
		        type: 'POST',
		        async: false
		    }).done(function (data) {
		        if (data && !data.error) {
		            util.sessionInfo = data;
		            userName = util.sessionInfo.userFullName;
		        } else {
		            alert("You don't have permission to access!", { styleClass: "srMessageBoxStyle srMessageBoxError" });
		        }
		    });
		};

		return BaseController.extend("sap.it.pmt.ui.view.App", {

			onInit : function () {
				var oViewModel,
					fnSetAppNotBusy,
					iOriginalBusyDelay = this.getView().getBusyIndicatorDelay();

				oViewModel = new JSONModel({
					busy : true,
					delay : 0
				});
				this.setModel(oViewModel, "appView");

				fnSetAppNotBusy = function() {
					oViewModel.setProperty("/busy", false);
					oViewModel.setProperty("/delay", iOriginalBusyDelay);
				};
				fnSetAppNotBusy();
//				this.getOwnerComponent().getModel().metadataLoaded()
//						.then(fnSetAppNotBusy);

				// apply content density mode to root view
				this.getView().addStyleClass(this.getOwnerComponent().getContentDensityClass());
				
			},
			
			onAfterRendering: function() {
				// set data
				login();
//				var oModel = new JSONModel();
//				oModel.setData({
//					currentUser : userName
//				});
//				this.getView().setModel(oModel);
				this.getView().byId("userText").setText("Current User [" + userName + "]");
			}

		});

	}
);