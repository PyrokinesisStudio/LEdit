<?php
class MySQL {
    function GetUserData($request) {
        require "connect.php";
        $q = $conn->prepare("SELECT " . $request . " FROM `users`");
        $q->execute();

        $first = true;
        while ($r = $q->fetch(PDO::FETCH_ASSOC)) {
            if ($first == false) {
                echo " " . $r[$request];
            } else {
                echo $r[$request];
                $first = false;
            }
        }
    }
    function GetIndex($type) {
        require "connect.php";
        $q = $conn->prepare("SELECT * FROM " . $type); // SQL Statement
        $q->execute();
        
        $type = substr($type, 0, strlen($type) - 1); // Converts it from plural form to singular
        $first = true;
        while ($r = $q->fetch(PDO::FETCH_ASSOC)) { // Loops through all results
            if ($first == false) { 
                echo " " . $r[$type]; 
            } else {
                echo $r[$type];
                $first = false;
            }
        }
    }
    function GetFileData($file) {
        require "connect.php";
        $q = $conn->prepare("SELECT * FROM `files` WHERE file=:file");
        $q->bindParam(":file", $file);
        $q->execute();

        while ($r = $q->fetch(PDO::FETCH_ASSOC)) {
            echo $r['data'];
        }
    }
    function UploadFileData($file, $data, $user) {
        require "connect.php";
        $q1 = $conn->prepare("UPDATE `files` SET data=:data WHERE file=:file");
        $q1->bindParam(":data", $data);
        $q1->bindParam(":file", $file);
        $q1->execute();

        $q2 = $conn->prepare("INSERT INTO tracker (id, user, ip, file, data) VALUES (NULL, :user, 'none', :file, :data)");
        $q2->bindParam(":user", $user);
        $q2->bindParam(":file", $file);
        $q2->bindParam(":data", $data);
        $q2->execute();
    }
    function CreateFile($filename, $username, $data) {
        require "connect.php";
        $continue = true;
        $q0 = $conn->prepare("SELECT file FROM files");
        $q0->execute();

        while ($res = $q0->fetch(PDO::FETCH_ASSOC)) {
            if ($res['file'] == $filename) {
                $continue = false;
                break;
            }
        }

        if ($continue) {
            $q1 = $conn->prepare("INSERT INTO files (data, file, last_user) VALUES (:fdata, :file, :user)");
            $q1->bindParam(":fdata", $data);
            $q1->bindParam(":file", $filename);
            $q1->bindParam(":user", $username);
            $q1->execute();

            $q2 = $conn->prepare('INSERT INTO tracker (id, user, ip, file, data) VALUES (NULL, :usern, "none", :file, :data)');
            $q2->bindParam(":usern", $username);
            $q2->bindParam(":file", $filename);
            $q2->bindParam(":data", $data);
            $q2->execute();
        }
    }
    function DeleteFile($file, $username) {
        require "connect.php";
        $q1 = $conn->prepare("DELETE FROM files WHERE file=:file");
        $q1->bindParam(":file", $file);
        $q1->execute();      

        $q2 = $conn->prepare('INSERT INTO tracker (id, user, ip, file, data) VALUES (NULL, :usern, "none", :file, "FILE DELETE")');
        $q2->bindParam(":usern", $username);
        $q2->bindParam(":file", $file);
        $q2->execute();
    }
    function CreateFolder($folder, $username) {
        require "connect.php";

        $continue = true;
        $q0 = $conn->prepare("SELECT folder FROM folders");
        $q0->execute();

        while ($res = $q0->fetch(PDO::FETCH_ASSOC)) {
            if ($res['folder'] == $folder) {
                $continue = false;
                break;
            }
        }

        if ($continue) {
            $q1 = $conn->prepare("INSERT INTO folders (folder) VALUES (:folder)");
            $q1->bindParam(":folder", $folder);
            $q1->execute();

            $q2 = $conn->prepare('INSERT INTO tracker (id, user, ip, file, data) VALUES (NULL, :usern, "none", :folder, "FOLDER")');
            $q2->bindParam(":usern", $username);
            $q2->bindParam(":folder", $folder);
            $q2->execute();
        }
    }
    function DeleteFolder($folder, $username) {
        require "connect.php";
        $q1 = $conn->prepare("DELETE FROM folders WHERE folder=:folder");
        $q1->bindParam(":folder", $folder);
        $q1->execute();

        $q2 = $conn->prepare('INSERT INTO tracker (id, user, ip, file, data) VALUES (NULL, :usern, "none", :folder, "FOLDER DELETE")');
        $q2->bindParam(":usern", $username);
        $q2->bindParam(":folder", $folder);
        $q2->execute();
    }
    function GetRank($username) {
        require "connect.php";
        $q1 = $conn->prepare("SELECT * FROM `users` WHERE `username`=:username");
        $q1->bindParam(":username", $username);
        $q1->execute();

        while ($res = $q1->fetch(PDO::FETCH_ASSOC)) {
            echo $res['authLevel'];
        }
    }
}
?>