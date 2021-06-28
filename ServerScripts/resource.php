<?php

if(isset($_POST['filename']))
{
	$jsondata=$_POST['filename'];
	$content = @file_get_contents($jsondata . ".json");
	echo $content;
}