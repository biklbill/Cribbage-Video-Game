<?php
//create connections
$conn = new mysqli("localhost", "root", "root", "cribbage");

//check connection is successful
if ($conn->connect_error) {
    die("116: change email connection failed - " . $conn->connect_error); //error code #116 - change email connection failed
}

$userID = $_POST["userID"];
$newEmail = $_POST["newEmail"];

//check if new username exists
$newEmailCheckQuery = "SELECT EmailAddress FROM accounts WHERE EmailAddress = '$newEmail'";
$newEmailCheck = $conn->query($newEmailCheckQuery) or die("117: new email check query failed"); //error code #117 - new email check query failed

if ($newEmailCheck->num_rows > 0) {
    die("205: new email already exists"); //error code #205 - email already registered
}

$updateEmailQuery = "UPDATE accounts SET EmailAddress = '$newEmail' WHERE UserID = '$userID'";

if ($conn->query($updateEmailQuery) === TRUE) {
    echo "0";
} else {
    echo "118: error updating Email - " . $conn->error; //error code #118 - error updaing Email
}

$conn->close();
?>