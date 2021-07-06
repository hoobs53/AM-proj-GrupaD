var getdata_url = "http://192.168.0.103/cgi-bin/get_pixels.py"; ///< server app with config data
var setdata_url = "http://192.168.0.103/cgi-bin/led_display.py"; ///< server app with config data
var config_url = "http://192.168.0.103/config.json" ///< server app with config data

var currentLedState = new Array(64); ///< array with states of physical LEDs
var displayLedState = new Array(64); ///< array with states of display LEDs

// setting URLs using recieved IP from server app using GET method
function loadParams(){
	$.ajax(config_url, {
		type: 'GET', dataType: 'json',
		success: function(responseJSON) {
			let ip = responseJSON["ip"];
			$("#ip").val(ip);
			
			getdata_url = "http://" + ip + "cgi-bin/get_pixels.py";
			setdata_url = "http://" + ip + "cgi-bin/led_display.py";
			config_url = "http://" + ip + "cgi-bin/config.json";
		}
	});
}

// comparing LED arrays to see if changed
function checkIfChanged(){
	for(let i = 0; i < displayLedState.length; i++){
		let test = currentLedState[i][0];
		let test2 = displayLedState[i][0];
		if( currentLedState[i][0] != displayLedState[i][0] || currentLedState[i][1]!= displayLedState[i][1] || currentLedState[i][2]!= displayLedState[i][2]){
			return true;
		}
	}
	return false;
}

// updating status text
function updateStatus(){
	if(checkIfChanged()){
		$("#status").text("UNSAVED CHANGES*");
	}
	else {
		$("#status").text("");
	}
}

// changing color of the visualisation
function changeColorBox() {
	let r = $("#seekbarR").val();
	let g = $("#seekbarG").val();
	let b = $("#seekbarB").val();
	let color = "rgb(" + r + ", " + g + ", " + b + ")";
	$("div.colorBox").css("background-color", color);
}

// running listeners for changing seekerbars states
function runListeners() {
	seekbarR.addEventListener('click', changeColorBox);
	seekbarB.addEventListener('click', changeColorBox);
	seekbarG.addEventListener('click', changeColorBox);
}

// changing color of LEDs in web matrix
function changeColor() {
	let r = $("#seekbarR").val();
	let g = $("#seekbarG").val();
	let b = $("#seekbarB").val();
	let color = "rgb(" + r + ", " + g + ", " + b + ")";
	$(this).css("background-color", color);
	let id = $(this).attr('id').split('_')[1]
	displayLedState[id][0] = r;
	displayLedState[id][1] = g;
	displayLedState[id][2] = b;
	updateStatus();
}

// sets all LED colors to (0, 0, 0)
function clearDisplay() {
	for (var i=0; i < 64; i++) {
		$("#led_"+i).css("background-color", "#000000");
		currentLedState[i] = [0, 0 ,0];
		displayLedState[i] = [0, 0 ,0];
	}
	updateStatus();
	setLeds();
}

// diplay initialization
function initDisplay(responseJSON){
	let r, g, b, color;
	let row, col;
	for(let i=0; i<responseJSON.length; i++){
		currentLedState[i] = [...responseJSON[i]];
		displayLedState[i] = [...responseJSON[i]];
		r = responseJSON[i][0];
		g = responseJSON[i][1];
		b = responseJSON[i][2];
		color = "rgb(" + r + ", " + g + ", " + b + ")";
		let btn = $("<input>", {
			id: "led_"+i, 
			type: "button",
			"class":"btn btn-default form-control"
		});
		btn.css("background-color", color);
		btn.css("border", "1px solid #000000");
		btn.click(changeColor);
		if(i % 8 == 0){
			row = $("<div>",{"class":"row pad_top"});
		}
		let col = $("<div>",{"class":"col", "id":"led_div_"+i});					
		let input_group = $("<div>",{"class":"input-group"});
		input_group.append(btn);
		col.append(input_group);
		row.append(col);
		$("#led_matrix").append(row);
	}
}

// sending LED data to physical LEDs
function sendData() {
	let x, y, id;
	let r, g, b;
	let data = new Array(5);
	let obj = {};
	for(let i=0; i< displayLedState.length; i++){
	data = new Array(5);
	x = i % 8;
	y = Math.floor(i/8);
	id = "LED" + x + y;

	r = displayLedState[i][0];
	g = displayLedState[i][1];
	b = displayLedState[i][2];
	
	currentLedState[i][0] = r;
	currentLedState[i][1] = g;
	currentLedState[i][2] = b;
	
	data[0] = x;
	data[1] = y;
	data[2] = r;
	data[3] = g;
	data[4] = b;
	obj[id] = JSON.stringify(data);
	}
	return obj;
}

// setting LED colors
function setLeds() {
	params = sendData();
	console.log(params);
	$.post(setdata_url, params, function(data){
		console.log(data);
	});
	updateStatus();
}

// recieving LED data from server app using GET method
function init() {
	$.ajax(getdata_url, {
	type: 'GET', dataType: 'json',
	success: function(responseJSON, status, xhr) {
		initDisplay(responseJSON);
	}
	});
};

$(document).ready(() => { 
	init();
	runListeners();
	$("#send").click(setLeds);
	$("#clear").click(clearDisplay);
});