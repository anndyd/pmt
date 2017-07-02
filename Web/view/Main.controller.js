sap.ui.define([ 'jquery.sap.global',
		"sap/it/pmt/ui/view/base/BaseController",
		"sap/m/MessageToast",
		'sap/ui/model/json/JSONModel' ], function(jQuery, 
		BaseController, MessageToast, JSONModel) {
	"use strict";
	return BaseController.extend("sap.it.pmt.ui.view.Main", {

		onInit : function(oEvent) {
			var that = this;
			var oModel = new JSONModel();
			that.getView().setModel(oModel);
		}
	});

});