<?php
//create connecion
$conn = new mysqli("localhost", "root", "root", "cribbage");

//check connection is successful
if ($conn->connect_error) {
    die("108: leader board connection failed - " . $conn->connect_error); //error code #108 - leader board connection failed
}

$leaderBoardQuery = "SELECT Username, Score FROM accounts ORDER BY Score DESC LIMIT 20";
$result = $conn->query($leaderBoardQuery) or die("109: leader board data query failed"); //error code #109 - get leader board data query failed

if ($result->num_rows > 0) {
    echo "0";

    //output data of each row
    while ($row = $result->fetch_assoc()) {
        echo $row["Username"] . "\t" . $row["Score"] . "\t";
    }
} else {
    echo "110: no leader board data found"; //error code #110 - no leader board data found
}

$conn->close();
?>