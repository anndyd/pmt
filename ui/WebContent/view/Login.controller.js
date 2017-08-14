sap.ui.define([ 'jquery.sap.global',
  "sap/it/pmt/ui/view/base/BaseController",
  "sap/it/pmt/ui/service/UserService",
  'sap/ui/model/json/JSONModel' ], function(jQuery,
  BaseController, UserService, JSONModel) {
  "use strict";
  var us = new UserService();
  var txtInterval;
  return BaseController.extend("sap.it.pmt.ui.view.Login", {
    onInit : function(oEvent) {
      var that = this;

      var oModel = new JSONModel({
        empId : ""
      });

      that.getView().setModel(oModel);
      that.getView().bindElement("/");
    },
    onPress : function(evt) {
      var that = this;
      var param = that.getView().getModel().getData().empId;
      us.login(param).done(
        function(data) {
          if (data && !data.error) {
            util.sessionInfo = data;
          } else {
            alert("You don't have permission to access!", {
              styleClass : "srMessageBoxStyle srMessageBoxError"
            });
          }
        }
      );
    }
  });

});