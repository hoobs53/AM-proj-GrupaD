<?php

header("Content-Type: text/html");

if(isset($_POST["port"])){
	$port = $_POST["port"];
}

if(isset($_POST["api"])){
	$api = $_POST["api"];
}

if(isset($_POST["sample_time"])){
	$sample = $_POST["sample_time"];
}

if(isset($_POST["sample_amount"])){
	$sample_amount = $_POST["sample_amount"];
}

$result = array("port" => $port, "api" => $api, "sample_time" => $sample, "sample_amount" => $sample_amount);
file_put_contents("config.json", json_encode($result));