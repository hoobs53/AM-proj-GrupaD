function createRow(table_id, id) {
  var table = document.getElementById(table_id);
  var row = table.insertRow(-1);
  var cellName = row.insertCell(0);
  var cellData = row.insertCell(1);
  var cellUnit = row.insertCell(2);
  cellName.innerHTML = id[0];
  cellData.innerHTML = id[1];
  cellUnit.innerHTML = id[2];
}

function deleteRow(table_id) {
	while ($('#m_table tr').length > 1) {
		document.getElementById("m_table").deleteRow(-1);
	}
}

function emptyCheck(value) {
  Object.keys(value).length === 0
    && value.constructor === Object;
}

function refresh() {
	$.ajax("http://192.168.1.101/config.json", {
		type: 'GET', dataType: 'json',
		success: function(responseJSON) {
			var measurement = responseJSON["measurement"];
			var orientation = responseJSON["orientation"];
			var joystick = responseJSON["joystick"];
		}
	});
	
	deleteRow("m_table");
	deleteRow("o_table");
	deleteRow("y_table");
	
	if(emptyCheck("m_table"){ createRow("m_table", measurement); }
	if(emptyCheck("o_table"){ createRow("o_table", orientation); }
	if(emptyCheck("y_table"){ createRow("y_table", joystick); }
}
function test() {
	while ($('#m_table tr').length > 1) {
		
//	if(document.getElementById("m_table").rows.length >0){
		deleteRow("m_table");
	}
}



$(document).ready(() => {
	$("#refresh").click(test);
});