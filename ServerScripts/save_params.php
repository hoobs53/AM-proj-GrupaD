<?php

header("Content-Type: text/html");

if(isset($_POST["ip"])){
	$ip = $_POST["ip"];
}

if(isset($_POST["sample_time"])){
	$sample = $_POST["sample_time"];
}

if(isset($_POST["sample_amount"])){
	$sample_amount = $_POST["sample_amount"];
}

$result = array("ip" => $ip, "sample_time" => $sample, "sample_amount" => $sample_amount);
file_put_contents("config.json", json_encode($result));