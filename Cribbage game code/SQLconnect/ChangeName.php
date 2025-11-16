<?php
//create connection
$conn = new mysqli("localhost", "root", "root", "cribbage");

//check connection is successful
if ($conn->connect_error) {
    die("111: change name connection failed - " . $conn->connect_error); //error code #111 - change name connection failed
}

$userID = $_POST["userID"];
$newName = $_POST["newName"];

//check if new username exists
$newNameCheckQuery = "SELECT Username FROM accounts WHERE Username = '$newName'";
$newNameCheck = $conn->query($newNameCheckQuery) or die("112: new name check query failed"); //error code #112 - new name check query failed

if ($newNameCheck->num_rows > 0) {
    die("204: new name already exists"); //error code #204 - name already exists
}

$updateNameQuery = "UPDATE accounts SET Username = '$newName' WHERE UserID = '$userID'";

if ($conn->query($updateNameQuery) === TRUE) {
    echo "0";
} else {
    echo "113: error updating name - " . $conn->error; //error code #113 - error updaing name
}

$conn->close();
?>