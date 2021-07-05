var xdata; ///< x-axis labels array: time stamps
var ydata; ///< y-axis data array: x
var coordinates; ///<coordinates array

var xresponse;
var yresponse;

var chartContext;  ///< chart context i.e. object that "owns" chart
var chart;         ///< Chart.js object

var timer; ///< request timer

const resource_url = 'http://192.168.0.103/resource.php'; ///< server app with RPY orientation in JSON format
const get_conf_url = 'http://192.168.0.103/get_config.php';

function updateData(y){
	$("#x_coordinates").text(y["x"]);
	$("#y_coordinates").text(y["y"]);

	chart.data.datasets[0].data.push(y);
	chart.data.datasets[0].data.splice(0,1);	
	chart.update();
}

function startTimer(){
	timer = setInterval(ajaxJSON, sampleTimeMsec);
}

function stopTimer(){
	clearInterval(timer);
}

function ajaxJSON() {
	$.ajax(resource_url, {
		type: 'POST', dataType: 'json', data: {"filename":"joystick"},
		success: function(responseJSON, status, xhr) {
			updateData(responseJSON);
		}
	});
}

function setUrl(ipAddress) {
	resource_url = "http://" + ipAddress + "/resource.php";
	get_conf_url = "http://" + ipAddress + "/get_config.php";
}

function loadConf(data){
	sampleTimeMsec = data["sample_time"];  ///< sample time in msec
	ipAddress = data["ip"];   ///< ip address
	chartInit();
	setUrl(ipAddress);
}

function configInit(){
		$.ajax(get_conf_url, {
		type: 'GET', dataType: 'json',
		success: function(responseJSON){
			loadConf(responseJSON);
		},
		error: function(cos) {
			console.log(cos);
		}
	})
}

/**
* @brief Chart initialization
*/
function chartInit()
{
	// empty array
	xdata = [];
	ydata = []; 
	coordinates = [];

	// get chart context from 'canvas' element
	chartContext = $("#chart")[0].getContext('2d');

	chart = new Chart(chartContext, {
		// The type of chart: linear plot
		type: 'scatter',

		// Dataset: 'xdata' as labels, 'ydata' as dataset.data
		data: {
			labels: 'scatter dataset',
			datasets:[{
				label: 'point',
				backgroundColor: 'rgb(255, 0, 0)',
				borderColor: 'rgb(255, 0, 0)',
				data: [{
					x: 0,
					y: 0
				}]
			}]
		},
		// Configuration options
		options: {
			legend: {
				display: false
			},
			responsive: true,
			maintainAspectRatio: false,
			animation: false,
			scales: {
				yAxes: [{
					scaleLabel: {
						display: true,
						labelString: 'y'
					},
					ticks:{ 
						suggestedMin: -25,
						suggestedMax: 25
					}
				}],
				xAxes: [{
					scaleLabel: {
						display: true,
						labelString: 'x'
					},
					ticks:{ 
						suggestedMin: -25,
						suggestedMax: 25
					}
				}]
			}
		}
	});
}

$(document).ready(() => { 
	configInit();
	$("#start").click(startTimer);
	$("#stop").click(stopTimer);
});