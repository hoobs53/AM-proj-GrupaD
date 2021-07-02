var url_save = "http://192.168.0.103/save_params.php";
var url_load = "http://192.168.0.103/config.json";

function setDefaultConfig() {
	$("#ip").val("192.168.0.103");
	$("#sample_time").val(200);
	$("#sample_amount").val(100);
	setConfig();
}

function setConfig(){
	var ip = $("#ip").val();
	var sampletime = $("#sample_time").val();
	var sample_amount = $("#sample_amount").val();
	params = {};
	params["ip"] = ip;
	params["sample_time"] = sampletime;
	params["sample_amount"] = sample_amount;
	obj = JSON.stringify(params);
	url_save = "http://" + ip + "/save_params.php";
	url_load = "http://" + ip + "/config.json";
	$.post(url_save, params);
}

function loadParams(){
	$.ajax(url_load, {
		type: 'GET', dataType: 'json',
		success: function(responseJSON) {
			let ip = responseJSON["ip"];
			let sample = responseJSON["sample_time"];
			let sample_amount = responseJSON["sample_amount"];
			$("#ip").val(ip);
			$("#sample_time").val(sample);
			$("#sample_amount").val(sample_amount);
			url_save = "http://" + ip + "/save_params.php";
			url_load = "http://" + ip + "/config.json";
		}
	});
}

$(document).ready(() => {
	loadParams();
	$("#set").click(setConfig);
	$("#default").click(setDefaultConfig);
});