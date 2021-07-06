var ipAddress; ///< IP address

var resource_url = 'http://192.168.0.103/get_measurements.php'; ///< server app with measurement data
var get_conf_url = 'http://192.168.0.103/get_config.php'; ///< server app with config data 

// setting URL with received IP from config file
function configInit(){
		$.ajax(get_conf_url, {
		type: 'GET', dataType: 'json',
		success: function(responseJSON){
			ipAddress = data["ip"];
			resource_url = "http://" + ipAddress + "/get_measurements.php";
			get_conf_url = "http://" + ipAddress + "/get_config.php";
		}
	})

}

// creating cells and rows in tables
function createRow(table_id, id) {
  var table = document.getElementById(table_id);
  for (var i in id) {
	  var row = table.insertRow(-1);
		var cellName = row.insertCell(0);
		var cellData = row.insertCell(1);
		var cellUnit = row.insertCell(2);
		cellName.innerHTML = id[i]["name"];
		cellData.innerHTML = id[i]["value"];
		cellUnit.innerHTML = id[i]["unit"];
	  }
}

// deleting rows from tables
function deleteRow(table_id) {
	var table = document.getElementById(table_id);
	var rowCount = document.getElementById(table_id).rows.length;
	for (var i = 0; i < rowCount - 1; i++) {
		table.deleteRow(-1);
	}
}

// loading new data from server app using GET method
function refresh() {
	$.ajax(resource_url, {
		type: 'GET', dataType: 'json',
		success: function(responseJSON) {
			var measurement = responseJSON["measurements"];
			var orientation = responseJSON["orientation"];
			var joystick = responseJSON["joystick"];
						
				deleteRow("m_table"); //< "m_table" - measurement table
				deleteRow("o_table"); //< "o_table" - orientation table
				deleteRow("j_table"); //< "j_table" - joystick table
				
				createRow("m_table", measurement);
				createRow("o_table", orientation);
				createRow("j_table", joystick);
		}
	});
}

$(document).ready(() => {
	configInit();
	$("#refresh").click(refresh);
});