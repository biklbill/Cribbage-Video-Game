<?php
//create connection
$conn = new mysqli("localhost", "root", "root", "cribbage");

//check connection is successful
if ($conn->connect_error) {
    die("114: change password connection failed - " . $conn->connect_error); //error code #114 - change password connection failed
}

$userID = $_POST["userID"];
$newPassword = $_POST["newPassword"];

$updatePasswordQuery = "UPDATE accounts SET Password = '$newPassword' WHERE UserID = '$userID'";

if ($conn->query($updatePasswordQuery) === TRUE) {
    echo "0";
} else {
    echo "115: error updating password - " . $conn->error; //error code #115 - error updating password
}

$conn->close();
?>