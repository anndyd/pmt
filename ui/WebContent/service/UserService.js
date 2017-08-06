sap.ui.define([
          'jquery.sap.global',
		  "sap/it/pmt/ui/service/BaseService"
          ], function(jQuery, BaseService) {
  "use strict";
  var bs = new BaseService();
  return BaseService.extend("sap.it.pmt.ui.service.UserService", {
    doAction: function(oData) {
        var dtd = $.Deferred();
        bs.asyncReq({
          url: "/user/",
          type: "POST",
          contentType: "application/json",
          data: JSON.stringify(oData)
        }).done(function(data) {
          dtd.resolve(data);
        });
        return dtd.promise();
    }
  
  });
 });
