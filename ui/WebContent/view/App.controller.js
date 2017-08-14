sap.ui.define([
		"sap/it/pmt/ui/view/base/BaseController",
		"sap/ui/model/json/JSONModel"
	], function (BaseController, JSONModel) {
		"use strict";
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
				this.getView().byId("userText").setText("Current User [" + util.sessionInfo.userFullName + "]");
			}

		});

	}
);