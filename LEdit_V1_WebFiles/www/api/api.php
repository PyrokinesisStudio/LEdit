<?php
/***************\
* LEdit V1 API  * 
* by Team Shift * 
\***************/
require "lib/connect.php";
error_reporting(1);
ini_set('display_errors', 1);

$action = "NONE";

if (isset($_GET['action'])) {
	$action = $_GET['action'];
}

switch ($action) {
	case "NONE":
		echo "False";
		break;
	case "RequestUserData":
		require_once "lib/mysql.php";
		$ms = new MySQL;
		$ms->GetUserData($_GET['request']);
		break;
	case "RequestIndex":
		require_once "lib/mysql.php";
		$ms = new MySQL;
		$ms->GetIndex($_GET['type']);
		break;
	case "RequestFileData":
		require_once "lib/mysql.php";
		$ms = new MySQL;
		$ms->GetFileData($_GET['file']);
		break;
	case "CheckRank":
		require_once "lib/mysql.php";
		$ms = new MySQL;
		$ms->CheckRank($_GET['user']);
		break;
	////////////////////////////////////////
	case "UploadFileData":
		require_once "lib/dual_handler.php";
		$h = new Dual_Handler;
		$h->UploadFileData($_GET['file'], urldecode($_POST['data']), $_GET['username']);
		break;
	case "CreateFile":
		require_once "lib/dual_handler.php";
		$h = new Dual_Handler;
		$h->CreateFile($_GET['file'], $_GET['username'], urldecode($_POST['data'])); 
		break;
	case "CreateFolder":
		require_once "lib/dual_handler.php";
		$h = new Dual_Handler;
		$h->CreateFolder($_GET['folder'], $_GET['username']);
		break;
	case "DeleteFile":
		require_once "lib/dual_handler.php";
		$h = new Dual_Handler;
		$h->DeleteFile($_GET['file'], $_GET['username']); 
		break;
	case "DeleteFolder":
		require_once "lib/dual_handler.php";
		$h = new Dual_Handler;
		$h->DeleteFolder($_GET['folder'], $_GET['username']);
		break;
}