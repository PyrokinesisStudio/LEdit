<?php
require("configuration.php");

try {
    $conn = new PDO("mysql:host=" . $ms_hostName . ";port=" . $ms_port . ";dbname=". $ms_databaseName, $ms_username, $ms_password);
    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    if ($ms_devMode >= 1) {
        error_reporting(E_ALL);
        ini_set('display_errors', 1);
    }
} 
catch(PDOException $e) {
    echo "Error: <br>" . $e->getMessage();
    error_reporting(E_ALL);
    ini_set('display_errors', 1);
}
?>