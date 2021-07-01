var ip = "192.168.0.103";
var url = "http://" + ip + "/get_measurements.php";

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

function deleteRow(table_id) {
	var table = document.getElementById(table_id);
	var rowCount = document.getElementById(table_id).rows.length;
	for (var i = 0; i < rowCount - 1; i++) {
		table.deleteRow(-1);
	}
}

function emptyCheck(value) {
  Object.keys(value).length === 0
    && value.constructor === Object;
}

function refresh() {
	$.ajax(url, {
		type: 'GET', dataType: 'json',
		success: function(responseJSON) {
			var measurement = responseJSON["measurements"];
			var orientation = responseJSON["orientation"];
			var joystick = responseJSON["joystick"];
						
				deleteRow("m_table");
				deleteRow("o_table");
				deleteRow("j_table");
				
				createRow("m_table", measurement);
				createRow("o_table", orientation);
				createRow("j_table", joystick);
				
				if(emptyCheck(joystick)) {
				console.log("nie ziala");
				}
		},
		error: function(cokolwiek) {
		console.log(cokolwiek);
		}
	});
}


function test() {
	while ($('#m_table tr').length > 1) {
		deleteRow();
	}
}

$(document).ready(() => {
	console.log(url);
	$("#refresh").click(refresh);
});