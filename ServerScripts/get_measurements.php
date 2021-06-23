<?php

header("Content-Type: application/json");

$measurements = file_get_contents("pht.json");
$measurements = json_decode($measurements, true);

$orientation = file_get_contents("rpy.json");
$orientation = json_decode($orientation, true);

$joystick = file_get_contents("joystick.dat");
$joystick = json_decode($joystick, true);

$measurement_result = array();
$measurement_result["0"] = array("name" => "temperature", "value" => $measurements["temperature"], "unit" => "C");
$measurement_result["1"] = array("name" => "humidity", "value" => $measurements["humidity"], "unit" => "%");
$measurement_result["2"] = array("name" => "pressure", "value" => $measurements["pressure"], "unit" => "hPa");

$orientation_result = array();
$orientation_result["0"] = array("name" => "roll", "value" => $orientation["roll"], "unit" => "deg");
$orientation_result["1"] = array("name" => "pitch", "value" => $orientation["pitch"], "unit" => "deg");
$orientation_result["2"] = array("name" => "yaw", "value" => $orientation["yaw"], "unit" => "deg");

$joystick_result = array();
$joystick_result["0"] = array("name" => "x", "value" => $joystick["x"], "unit" => "-");
$joystick_result["1"] = array("name" => "y", "value" => $joystick["y"], "unit" => "-");
$joystick_result["2"] = array("name" => "mid_counter", "value" => $joystick["mid_counter"], "unit" => "-");

$result = array("measurements" => $measurement_result, "orientation" => $orientation_result, "joystick" => $joystick_result);

echo json_encode($result);