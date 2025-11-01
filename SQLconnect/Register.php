<?php
//create connections
$conn = new mysqli("localhost", "root", "root", "cribbage");

//check connection is successful
if ($conn->connect_error) {
	die("101: register connection failed - " . $conn->connect_error); //error code #101 - register connection failed
}

$username = $_POST["username"];
$emailAddress = $_POST["emailAddress"];
$password = $_POST["password"];

//check if username exists
$usernameCheckQuery = "SELECT Username FROM accounts WHERE Username = '$username'";
$usernameCheck = $conn->query($usernameCheckQuery) or die("102: username check query failed"); //error code #102 - username check query failed

if ($usernameCheck->num_rows > 0) {
	die("201: username already exists"); //error code #201 - username already exists
}

//check if email address is registered
$emailAddressCheckQuery = "SELECT EmailAddress FROM accounts WHERE EmailAddress = '$emailAddress'";
$emailAddressCheck = $conn->query($emailAddressCheckQuery) or die("103: email address check query failed"); //error code #103 - email address check query failed

if ($emailAddressCheck->num_rows > 0) {
	die("202: email address already registered"); //error code #202 - email address already registered
}

//add account to the table
$insertAccountQuery = "INSERT INTO accounts (Username, EmailAddress, Password) VALUES ('$username', '$emailAddress', '$password')";
$conn->query($insertAccountQuery) or die("104: insert user query failed"); //error code #104 - insert user query failed

echo "0";

$conn->close(); //execution successful
?>