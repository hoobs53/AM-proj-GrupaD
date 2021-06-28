function setConfig(){
	var ip = $("#ip").val();
	var sampletime = $("#sample").val();
	var sample_amount = $("#sample_amount").val();
	params = {};
	params["ip"] = ip;
	params["sample_time"] = sampletime;
	params["sample_amount"] = sample_amount;
	obj = JSON.stringify(params);
	$.post("http://192.168.1.104/save_params.php", params);
}

function loadParams(){
	$.ajax("http://192.168.1.104/config.json", {
		type: 'GET', dataType: 'json',
		success: function(responseJSON) {
			let ip = responseJSON["ip"];
			let sample = responseJSON["sample_time"];
			let sample_amount = responseJSON["sample_amount"];
			$("#ip").val(ip);
			$("#sample").val(sample);
			$("#sample_amount").val(sample_amount);
		}
	});
}

$(document).ready(() => {
	loadParams();
	$("#set").click(setConfig);
});