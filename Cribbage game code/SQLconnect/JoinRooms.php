<?php
//create connection
$conn = new mysqli("localhost", "root", "root", "cribbage");

//check connection is successful
if ($conn->connect_error) {
    die("130: retrieve rooms connection failed - " . $conn->connect_error); //error code #130 - retrieve rooms connection failed
}

$roomID = $_POST["roomID"];
$playerID = $_POST["playerID"];

//check if roomID exists
$roomIDCheckQuery = "SELECT RoomID FROM players WHERE RoomID = '$roomID'";
$roomIDCheck = $conn->query($roomIDCheckQuery) or die("131: roomID check query failed"); //error code #131 - roomID check query failed

if ($roomIDCheck->num_rows != 1) {
    die("132: zero or more than one room found"); //error code #132 - zero or more than one room found
}

//add player to room
$insertPlayerQuery = "INSERT INTO players (RoomID, PlayerID, IsHost) VALUES ('$roomID', '$playerID', 0)";
$conn->query($insertPlayerQuery) or die("133: insert player query failed"); //error code #133 - insert player query failed

//update room status to full
$updateStatusQuery = "UPDATE rooms SET IsFull = 1 WHERE RoomID = '$roomID'";
$conn->query($updateStatusQuery) or die("134: update room status query failed"); //error code #134 - update room status query failed

echo "0";

$conn->close(); //execution successful
?>