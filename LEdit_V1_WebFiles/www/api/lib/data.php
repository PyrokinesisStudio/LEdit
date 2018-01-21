<?php
class Data
{
    public function DeleteFolder($path)
    {
        require "configuration.php";
        $d = new Data;
        $oldPath = preg_replace("/\\\\/", "/", $path);
        $path = $project_folder . $oldPath;
        rrmdir($path);
        rrmdir($gmod_path . $oldPath);

    }

    public function DeleteFile($path)
    {
        require "configuration.php";
        $oldPath = preg_replace("/\\\\/", "/", $path);
        $path = $project_folder . $oldPath;
        if (unlink($path)) {

        } else {
            echo "unlink error for stage 1 delete file";
        }
        if (!unlink($gmod_path . $oldPath)) {
            echo "unlink error for stage 2 delete file";
        }
    }

    public function CreateFile($fData, $path)
    {
        require "configuration.php";
        $d = new Data;
        $oldPath = preg_replace("/\\\\/", "/", $path);
        $path = $project_folder . $oldPath;

        $file = fopen($path, 'w') or die('Failed to open file - CREATE:  ' . $path);
        fwrite($file, $fData);
        fclose($file);
        $d->CopyFile($path, $gmod_path . $oldPath);

    }

    public function CreateFolder($folder)
    {
        require "configuration.php";
        $d = new Data;
        $oldFolder = preg_replace("/\\\\/", "/", $folder);
        $folder = $project_folder . $oldFolder;

        mkdir($folder, 0777);
        $d->CopyFolder($gmod_path . $oldFolder);
    }

    //////////////////////////////////////
    //        INTERNAL FUNCTIONS        //
    //////////////////////////////////////

    public function CopyFile($pathToCopy, $pathToSend)
    {
        $path = fopen($pathToCopy, 'r') or die('fail');
        $data = fread($path, filesize($pathToCopy));

        $file = fopen($pathToSend, 'w') or die('Failed to open file - COPY: ' . $pathToSend);
        ftruncate($file, 0);
        fwrite($file, $data);
        fclose($path);
        fclose($file);
    }

    public function CopyFolder($pathToSend)
    {
        // DOES NOT COPY THE DATA INSIDE THE FOLDER!!!
        mkdir($pathToSend, 0777);
    }

    public function ReadLog()
    {
        $path = fopen("../../../gmod_development/garrysmod/console.log", 'r') or die('fail');
        $data = fread($path, filesize("../../../gmod_development/garrysmod/console.log"));
        echo $data;
    }
}

function rrmdir($dir)
{
    if (is_dir($dir)) {
        $objects = scandir($dir);
        foreach ($objects as $obj) {
            if ($obj != "." && $obj != "..") {
                if (filetype($dir . "/" . $obj) == "dir") {
                    rrmdir($dir . "/" . $obj);
                } else {
                    unlink($dir . "/" . $obj);
                }
            }
        }
        if (is_array($obj)) {
            reset($obj);
        }
        rmdir($dir);
    }
}
