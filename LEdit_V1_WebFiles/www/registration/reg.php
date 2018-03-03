<?php
    /* CONFIG OPTION(S) */
    $enabled = true; // This should be false when this isn't in use, just to be safe
    $api_dir = "../api/lib/connect.php";
    ///////
    require($api_dir);
?>

<form action="reg.php" method="POST">
    <input type="text" id="username" name="username" placeholder="Username" required /><br>
    <input type="password" id="password" name="password" placeholder="Password" required /><br>
    <input type="text" id ="text" name="submitted" value=1 style="display:none;" />
    <input type="submit" />
</form> 

<?php
if (isset($_POST['submitted'])) {
    $check = $conn->prepare("SELECT * FROM users");
    $check->execute();
    while ($res = $check->fetch(PDO::FETCH_ASSOC)) {
        if ($res['username'] == strtoupper($_POST['username'])) {
            echo "Username already taken, please try again!";
            return false;
        }
    }
    // Username free
    $uname = strtoupper($_POST["username"]);
    $pwd = password_hash($_POST["password"], PASSWORD_BCRYPT);
    $ins = $conn->prepare("INSERT INTO users (id, username, password) VALUES (null, :username, :password)");
    $ins->bindParam(":username", $uname);
    $ins->bindParam(":password", $pwd);
    $ins->execute();

    echo "User account successfully created<br>";
    echo "Details:<br>
    Username - " . $_POST['username'] . "<br>
    Password - " . $_POST['password'] . "<br>
    <br>
    REMEMBER! This script should <b>ONLY</b> be run by localhost, and anyone outside of localhost <b>SHOULD NOT</b> be able to access it!
    ";
}

?>