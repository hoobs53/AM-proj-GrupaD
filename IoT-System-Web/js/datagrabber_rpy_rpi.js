var sampleTimeSec;          ///< sample time in sec
var sampleTimeMsec;  		///< sample time in msec
var maxSamplesNumber;       ///< maximum number of samples
var ipAddress				///< IP address

var xdata;  ///< x-axis labels array: time stamps
var ydataR; ///< y-axis data array: Roll
var ydataP; ///< y-axis data array: Pitch
var ydataY; ///< y-axis data array: Yaw
var lastTimeStamp; ///< most recent time stamp 

var chartContext;  ///< chart context i.e. object that "owns" chart
var chart;         ///< Chart.js object

var timer; ///< request timer

var resource_url = 'http://192.168.0.103/resource.php'; ///< server app with RPY orientation in JSON format
var get_conf_url = 'http://192.168.0.103/get_config.php'; ///< server app with config data 


// updates data to the chart
function updatePlot(y){
	if(ydataR.length > maxSamplesNumber)
	{
		removeOldData();
		lastTimeStamp += sampleTimeSec;
		xdata.push(lastTimeStamp.toFixed(4));
	}
	ydataR.push(y["roll"]);
	ydataP.push(y["pitch"]);
	ydataY.push(y["yaw"]);
	chart.update();
}

// removes data from the chart
function removeOldData(){
	xdata.splice(0,1);
	ydataR.splice(0,1);
	ydataP.splice(0,1);
	ydataY.splice(0,1);
}

function startTimer(){
	timer = setInterval(getResources, sampleTimeMsec);
}

function stopTimer(){
	clearInterval(timer);
}

// recieving RPY orientation from server app using POST method
function getResources() {
	$.ajax(resource_url, {
		type: 'POST', dataType: 'json', data: {"filename":"rpy"},
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
		},
		error: function(cos) {
			console.log(cos);
		}
	})
}

// geting values from received JSON
function loadConf(data){
	sampleTimeMsec = data["sample_time"];  ///< sample time in msec
	sampleTimeSec = sampleTimeMsec/1000;
	maxSamplesNumber = parseInt(data["sample_amount"]);       ///< maximum number of samples
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
	// array with consecutive integers: <0, maxSamplesNumber-1>
	xdata = [...Array(maxSamplesNumber).keys()]; 
	// scaling all values ​​times the sample time 
	xdata.forEach(function(p, i) {this[i] = (this[i]*sampleTimeSec).toFixed(4);}, xdata);

	// last value of 'xdata'
	lastTimeStamp = +xdata[xdata.length-1]; 

	// empty array
	ydataR = []; 
	ydataP = []; 
	ydataY = []; 

	// get chart context from 'canvas' element
	chartContext = $("#chart")[0].getContext('2d');

	chart = new Chart(chartContext, {
		// The type of chart: linear plot
		type: 'line',

		// Dataset: 'xdata' as labels, 'ydata' as dataset.data
		data: {
			labels: xdata,
			datasets: [{
				fill: false,
				label: 'Roll',
				backgroundColor: 'rgb(255, 0, 0)',
				borderColor: 'rgb(255, 0, 0)',
				data: ydataR,
				lineTension: 0
			},
			{
				fill: false,
				label: 'Pitch',
				backgroundColor: 'rgb(0, 255, 0)',
				borderColor: 'rgb(0, 255, 0)',
				data: ydataP,
				lineTension: 0
			},
			{
				fill: false,
				label: 'Yaw',
				backgroundColor: 'rgb(0, 0, 255)',
				borderColor: 'rgb(0, 0, 255)',
				data: ydataY,
				lineTension: 0
			}
			]
		},

		// Configuration options
		options: {
			responsive: true,
			maintainAspectRatio: false,
			animation: false,
			scales: {
				yAxes: [{
					scaleLabel: {
						display: true,
						labelString: 'Orientation [deg]'
					}
				}],
				xAxes: [{
					scaleLabel: {
						display: true,
						labelString: 'Time [s]'
					}
				}]
			}
		}
	});
	
	ydataR = chart.data.datasets[0].data;
	ydataP = chart.data.datasets[1].data;
	ydataY = chart.data.datasets[2].data;
	xdata = chart.data.labels;
	

}

$(document).ready(() => { 
	configInit();
	$("#start").click(startTimer);
	$("#stop").click(stopTimer);
});