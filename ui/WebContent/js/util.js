var util = {
    isLogin: false,
    sessionInfo: {},
    badgeIdLength: 10,
    nodataTimeout: 5000
};

(function() {
	if (!String.format) {
		String.format = function(format) {
			var args = Array.prototype.slice.call(arguments, 1);
			return format.replace(/{(\d+)}/g, function(match, number) {
				return typeof args[number] != 'undefined'
					? args[number]
					: match;
			});
		};
	}
})();