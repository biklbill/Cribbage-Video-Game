<?php
//create connecion
$conn = new mysqli("localhost", "root", "root", "cribbage");

//check connection is successful
if ($conn->connect_error) {
    die("105: login connection failed - " . $conn->connect_error); //error code #101 - login connection failed
}

$usernameOrEmailAddress = $_POST["usernameOrEmailAddress"];
$password = $_POST["password"];

if (ctype_alnum($usernameOrEmailAddress)) {
    $loginCheckQuery = "SELECT UserID, Username, EmailAddress, Password, Score, GamesPlayed, GamesWon FROM accounts WHERE Username = '$usernameOrEmailAddress'";
} else {
    $loginCheckQuery = "SELECT UserID, Username, EmailAddress, Password, Score, GamesPlayed, GamesWon FROM accounts WHERE EmailAddress = '$usernameOrEmailAddress'";
}
$loginCheck = $conn->query($loginCheckQuery) or die("106: get user data query failed"); //error code #106 - get user data query failed

if ($loginCheck->num_rows > 1) {
    echo "107: more than one username or email address found"; //error code #107 - more than one username or email address found
    exit();
}

//get login info from query
$loginInfo = $loginCheck->fetch_assoc();

if ($loginCheck->num_rows < 1 || $password != $loginInfo["Password"]) {
    echo "203: username or password incorrect"; //error code #203 - username or password incorrect
    exit();
}

echo "0\t" . $loginInfo["UserID"];

$conn->close();
?>