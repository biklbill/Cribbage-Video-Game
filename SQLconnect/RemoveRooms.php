<?php
//create connecion
$conn = new mysqli("localhost", "root", "root", "cribbage");

//check connection is successful
if ($conn->connect_error) {
    die("140: remove room connection failed - " . $conn->connect_error); //error code #140 - remove room connection failed
}

$roomID = $_POST["roomID"];

$removeRoomQuery = "DELETE FROM rooms WHERE RoomID = '$roomID'";
$removeRoom = $conn->query($removeRoomQuery) or die("141: remove room query failed"); //error code #141 - remove room query failed

echo "0";

$conn->close();
?>