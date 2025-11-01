<?php
//create connecion
$conn = new mysqli("localhost", "root", "root", "cribbage");

//check connection is successful
if ($conn->connect_error) {
    die("119: retrieve rooms connection failed - " . $conn->connect_error); //error code #119 - retrieve rooms connection failed
}

$getRoomsQuery = "SELECT RoomID, PlayerID FROM players WHERE IsHost = 1";
$getRooms = $conn->query($getRoomsQuery) or die("120: get rooms query failed"); //error code #120 - get rooms query failed

if ($getRooms->num_rows > 0) {
    echo "0";

    //output data of each row
    while ($row = $getRooms->fetch_assoc()) {
        $roomID = $row["RoomID"];
        $playerID = $row["PlayerID"];

        $getRoomDataQuery = "SELECT RoomName, Password, IsFull FROM rooms WHERE RoomID = '$roomID'";
        $getPlayerDataQuery = "SELECT Username FROM accounts WHERE UserID = '$playerID'";

        $getRoomData = $conn->query($getRoomDataQuery) or die("121: get rooms query failed"); //error code #121 - get room data query failed
        $getPlayerData = $conn->query($getPlayerDataQuery) or die("122: get rooms query failed"); //error code #122 - get player data query failed

        $roomData = $getRoomData->fetch_assoc();
        $playerData = $getPlayerData->fetch_assoc();

        echo $roomID . "\t" . $hostID . "\t" . $roomData["RoomName"] . "\t" . $playerData["Username"] . "\t" . $roomData["Password"] . "\t" . $roomData["IsFull"] . "\t";
    }
} else {
    echo "206: no rooms currently"; //error code #206 - no rooms currently
}

$conn->close();
?>