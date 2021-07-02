var sampleTimeSec;      ///< sample time in sec
var sampleTimeMsec;  	///< sample time in msec
var maxSamplesNumber;   ///< maximum number of samples
var ipAddress;			///< ip address

var xdata; ///< x-axis labels array: time stamps
var ydataP; ///< y-axis data array: Pressure
var ydataH; ///< y-axis data array: Humidity
var ydataT; ///< y-axis data array: Temperature
var lastTimeStamp; ///< most recent time stamp 

var chartContext;  ///< chart context i.e. object that "owns" chart
var chart;         ///< Chart.js object

var timer; ///< request timer

var resource_url = "http://192.168.0.103/resource.php"; ///< server app with RPY orientation in JSON format
var get_conf_url = "http://192.168.0.103/get_config.php"; ///< server app with configuration in JSON format

function addData(y){
	if(ydataP.length > maxSamplesNumber)
	{
		removeOldData();
		lastTimeStamp += sampleTimeSec;
		xdata.push(lastTimeStamp.toFixed(4));
	}
	ydataP.push(y["pressure"]);
	ydataH.push(y["humidity"]);
	ydataT.push(y["temperature"]);
	chart.update();
}

function removeOldData(){
	xdata.splice(0,1);
	ydataP.splice(0,1);
	ydataH.splice(0,1);
	ydataT.splice(0,1);
}

function startTimer(){
	timer = setInterval(ajaxJSON, sampleTimeMsec);
}

function stopTimer(){
	clearInterval(timer);
}

function ajaxJSON() {
	$.ajax(resource_url, {
		type: 'POST', dataType: 'json', data: {"filename":"pht"},
		success: function(responseJSON, status, xhr) {
			addData(responseJSON);
		}
	});
}

function setUrl(ipAddress) {
	resource_url = "http://" + ipAddress + "/resource.php";
	get_conf_url = "http://" + ipAddress + "/get_config.php";
}

function loadConf(data){
	sampleTimeMsec = data["sample_time"];  ///< sample time in msec
	sampleTimeSec = sampleTimeMsec/1000;
	maxSamplesNumber = parseInt(data["sample_amount"]);       ///< maximum number of samples
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
	// array with consecutive integers: <0, maxSamplesNumber-1>
	xdata = [...Array(maxSamplesNumber).keys()]; 
	// scaling all values ​​times the sample time 
	xdata.forEach(function(p, i) {this[i] = (this[i]*sampleTimeSec).toFixed(4);}, xdata);

	// last value of 'xdata'
	lastTimeStamp = +xdata[xdata.length-1]; 

	// empty array
	ydataP = []; 
	ydataH = []; 
	ydataT = []; 

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
				label: 'Pressure',
				yAxisID: 'A',
				backgroundColor: 'rgb(255, 0, 0)',
				borderColor: 'rgb(255, 0, 0)',
				data: ydataP,
				lineTension: 0
			},
			{
				fill: false,
				label: 'Humidity',
				yAxisID: 'B',
				backgroundColor: 'rgb(0, 255, 0)',
				borderColor: 'rgb(0, 255, 0)',
				data: ydataH,
				lineTension: 0
			},
			{
				fill: false,
				label: 'Temperature',
				yAxisID: 'B',
				backgroundColor: 'rgb(0, 0, 255)',
				borderColor: 'rgb(0, 0, 255)',
				data: ydataT,
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
					id: 'A',
					position: 'left',
					scaleLabel: {
						display: true,
						labelString: 'Measurements [hPA]'
					}
				},{
					id: 'B',
					position: 'left',
					scaleLabel: {
						display: true,
						labelString: 'Measurements [%, *C]'
					}
				}],
				xAxes: [{
					scaleLabel: {
						display: true,
						labelString: 'Time [sec]'
					}
				}]
			}
		}
	});
	
	ydataP = chart.data.datasets[0].data;
	ydataH = chart.data.datasets[1].data;
	ydataT = chart.data.datasets[2].data;
	xdata = chart.data.labels;
	

}

$(document).ready(() => { 
	configInit();
	$("#start").click(startTimer);
	$("#stop").click(stopTimer);
});