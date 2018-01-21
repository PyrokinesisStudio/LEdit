<?php
class Dual_Handler {
    function UploadFileData($file, $fileData, $user) {
        require_once "mysql.php";
        $ms = new MySQL;
        $ms->UploadFileData($file, $fileData, $user);
        
        require_once "data.php";
        $data = new Data;
        $data->CreateFile($fileData, $file);

        echo "True";
    }
    function CreateFile($file, $username, $fileData) {
        require_once "mysql.php";
        $ms = new MySQL;
        $ms->CreateFile($file, $username, $fileData);

        require_once "data.php";
        $data = new Data;
        $data->CreateFile($fileData, $file);

        echo "True";
    }
    function CreateFolder($folder, $username, $data) {
        require_once "mysql.php";
        $ms = new MySQL;
        $ms->CreateFolder($folder, $username);

        require_once "data.php";
        $data = new Data;
        $data->CreateFolder($folder);

        echo "True";
    }
    function DeleteFile($file, $username) {
        require_once "mysql.php";
        $ms = new MySQL;
        $ms->DeleteFile($file, $username);

        require_once "data.php";
        $data = new Data;
        $data->DeleteFile($file);

        echo "True";
    }
    function DeleteFolder($folder, $username) {
        require_once "mysql.php";
        $ms = new MySQL;
        $ms->DeleteFolder($folder, $username);

        require_once "data.php";
        $data = new Data;
        $data->DeleteFolder($folder);

        echo "True";
    }
}
?>