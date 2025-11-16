<?php
//create connection
$conn = new mysqli("localhost", "root", "root", "cribbage");

//check connection is successful
if ($conn->connect_error) {
    die("138: update user data connection failed - " . $conn->connect_error); //error code #138 - update user data connection failed
}

$userID = $_POST["userID"];

$updateUserDataQuery = "SELECT Username, EmailAddress, Password, Score, GamesPlayed, GamesWon FROM accounts WHERE UserID = '$userID'";
$updateUserData = $conn->query($updateUserDataQuery) or die("139: update user data query failed"); //error code #139 - update user data query failed

//get user data from query
$userData = $updateUserData->fetch_assoc();

echo "0\t" . $userData["Username"] . "\t" . $userData["EmailAddress"] . "\t" . $userData["Password"] . "\t" . $userData["Score"] . "\t" . $userData["GamesPlayed"] . "\t" . $userData["GamesWon"];

$conn->close();
?>