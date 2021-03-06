sap.ui.define([
          'jquery.sap.global', 
          "sap/ui/base/Object",
          "sap/m/MessageBox"
          ], function(jQuery, Object, MessageBox) {
  "use strict";
  var processStatus = function(err) {
      switch (err.status) {
        case 400:
        case 500:
        	var resTxt = err.responseText.replace(/(<([^>]+)>)/g, "");
        	MessageBox.alert(resTxt, {styleClass: "srMessageBoxStyle srMessageBoxError"});
          break;
        case 401:
        case 403:
        	login();
//          window.location.href = "login.html";
          break;
        case 404:
          MessageBox.alert("404 Not Found", {styleClass: "srMessageBoxStyle srMessageBoxError"});
          break;
        default:
          break;
      }
    };
  var login = function() {
		var url = "/srserver/user/active";
		$.ajax({
		    url: url,
		    type: 'POST'
		}).always(function(data){
			if (!data || data === "") {
				MessageBox.alert("You don't have permission to access!", {styleClass: "srMessageBoxStyle srMessageBoxError"});
			}
		});
  };

  return Object.extend("sap.it.pmt.ui.service.BaseService", {
    asyncReq: function(options) {
      var dtd = $.Deferred();
      options.async = true;
      options.selfHandleFail = options.selfHandleFail || false;
      options.tryCount = 0;
      options.retryLimit = 3;
      
      if (!!window.ActiveXObject || "ActiveXObject" in window) {
        $.ajaxSetup({ cache: false });
      }
      
      $.ajax(options).done(function(data) {
        dtd.resolve(data);
      }).done(function(data) {
        dtd.resolve(data);
      }).fail(function(err) {
        processStatus(err);
        dtd.reject(err);
      }).always(function(){
        sap.ui.core.BusyIndicator.hide();
      });
      return dtd.promise();
    }
  });
});
