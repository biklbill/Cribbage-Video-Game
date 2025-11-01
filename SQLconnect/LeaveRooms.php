<?php
//create connecion
$conn = new mysqli("localhost", "root", "root", "cribbage");

//check connection is successful
if ($conn->connect_error) {
    die("142: leave rooms connection failed - " . $conn->connect_error); //error code #142 - leave rooms connection failed
}

$roomID = $_POST["roomID"];
$playerID = $_POST["playerID"];

//update room status to available
$updateStatusQuery = "UPDATE rooms SET IsFull = 0 WHERE RoomID = '$roomID'";
$conn->query($updateStatusQuery) or die("143: update room status query failed"); //error code #143 - update room status query failed

//update players in the room
$updatePlayersQuery = "DELETE FROM players WHERE playerID = '$playerID'";
$conn->query($updatePlayersQuery) or die("144: update players query failed"); //error code #144 - update players query failed

echo "0";

$conn->close(); //execution successful
?>