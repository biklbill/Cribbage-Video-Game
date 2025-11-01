<?php
//create connecion
$conn = new mysqli("localhost", "root", "root", "cribbage");

//check connection is successful
if ($conn->connect_error) {
    die("135: retrieve rooms connection failed - " . $conn->connect_error); //error code #135 - update score connection failed
}

$ownID = $_POST["ownID"];
$ownScore = $_POST["ownPoints"];
$opponentID = $_POST["opponentID"];
$opponentScore = $_POST["opponentPoints"];

//update own
$updateOwnQuery = "UPDATE accounts SET Score = Score + '$ownScore', GamesPlayed = GamesPlayed + 1, GamesWon = GamesWon + 1 WHERE UserID = '$ownID'";
$updateOwn = $conn->query($updateOwnQuery) or die("136: update own query failed"); //error code #136 - update own query failed

//update opponent
$updateOpponentQuery = "UPDATE accounts SET Score = Score + '$opponentScore', GamesPlayed = GamesPlayed + 1 WHERE UserID = '$opponentID'";
$updateOpponent = $conn->query($updateOpponentQuery) or die("137: update opponent query failed"); //error code #137 - update opponent query failed

echo "0";

$conn->close(); //execution successful
?>