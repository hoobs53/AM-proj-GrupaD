var sampleTimeMsec;  		///< sample time in msec
var ipAddress				///< IP address

var chartContext;  ///< chart context i.e. object that "owns" chart
var chart;         ///< Chart.js object

var timer; ///< request timer

var resource_url = 'http://192.168.0.103/resource.php'; ///< server app with joystick data
var get_conf_url = 'http://192.168.0.103/get_config.php'; ///< server app with config data 


// updates data to the chart
function updatePlot(y){
	$("#x_coordinates").text(y["x"]);
	$("#y_coordinates").text(y["y"]);

	chart.data.datasets[0].data.push(y);
	chart.data.datasets[0].data.splice(0,1);	
	chart.update();
}

function startTimer(){
	timer = setInterval(getResources, sampleTimeMsec);
}

function stopTimer(){
	clearInterval(timer);
}

// recieving joystick data from server app using POST method
function getResources() {
	$.ajax(resource_url, {
		type: 'POST', dataType: 'json', data: {"filename":"joystick"},
		success: function(responseJSON, status, xhr) {
			updatePlot(responseJSON);
		}
	});
}

// recieving config data from server app using GET method
function configInit(){
		$.ajax(get_conf_url, {
		type: 'GET', dataType: 'json',
		success: function(responseJSON){
			loadConf(responseJSON);
		}
	})
}

// geting values from received JSON
function loadConf(data){
	sampleTimeMsec = data["sample_time"];  ///< sample time in msec
	ipAddress = data["ip"];   ///< ip address
	chartInit();
	setUrl(ipAddress);
}

// setting URL with received IP from config file
function setUrl(ipAddress) {
	resource_url = "http://" + ipAddress + "/resource.php";
	get_conf_url = "http://" + ipAddress + "/get_config.php";
}


// Chart initialization
function chartInit()
{
	// get chart context from 'canvas' element
	chartContext = $("#chart")[0].getContext('2d');

	chart = new Chart(chartContext, {
		// The type of chart: scatter plot
		type: 'scatter',

		// Dataset: data in the point format {x, y}
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
					// fixed y axis to show range between <-25, 25>
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
					// fixed x axis to show range between <-25, 25>
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