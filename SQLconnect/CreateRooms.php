<?php
//create connecion
$conn = new mysqli("localhost", "root", "root", "cribbage");

//check connection is successful
if ($conn->connect_error) {
    die("123: retrieve rooms connection failed - " . $conn->connect_error); //error code #123 - create rooms connection failed
}

$roomName = $_POST["roomName"];
$password = $_POST["password"];
$playerID = $_POST["playerID"];

//check if room name exists
$roomNameCheckQuery = "SELECT RoomName FROM rooms WHERE RoomName = '$roomName'";
$roomNameCheck = $conn->query($roomNameCheckQuery) or die("124: room name check query failed"); //error code #124 - room name check query failed

if ($roomNameCheck->num_rows > 0) {
    die("207: room name already exists"); //error code #207 - room name already exists
}


if ($password == "") {
    //add public room to the table
    $insertRoomQuery = "INSERT INTO rooms (RoomName) VALUES ('$roomName')";
    $conn->query($insertRoomQuery) or die("125: insert public room query failed"); //error code #125 - insert public room query failed
} else {
    //add private room to the table if it is private
    $insertRoomQuery = "INSERT INTO rooms (RoomName, Password) VALUES ('$roomName', '$password')";
    $conn->query($insertRoomQuery) or die("126: insert private room query failed"); //error code #126 - insert private room query failed
}

$getRoomIDQuery = "SELECT RoomID FROM rooms WHERE RoomName = '$roomName'";
$getRoomID = $conn->query($getRoomIDQuery) or die("127: get roomID query failed"); //error code #127 - get roomID query failed

if ($getRoomID->num_rows > 1) {
    echo "128: more than one room found"; //error code #128 - more than one room found
    exit();
}

//get roomID from query
$roomID = $getRoomID->fetch_assoc()["RoomID"];

//add player to room
$insertPlayerQuery = "INSERT INTO players (RoomID, PlayerID, IsHost) VALUES ('$roomID', '$playerID', 1)";
$conn->query($insertPlayerQuery) or die("129: insert player query failed"); //error code #129 - insert player query failed

echo "0\t" . $roomID;
$conn->close(); //execution successful
?>