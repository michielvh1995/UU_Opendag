<?php
  // Use a variable to set the spray or no
  $yesNo = $_GET["spray"];

  // Opens the spray.sqlite database file
  $db = new PDO('sqlite:spray.sqlite');
  $db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

  $sprayQuery = $db -> prepare('INSERT INTO [spray] VALUES ("1")');

  // Only tell the db its allowed to spray, when it really is
  if($yesNo == "true")
    {
      $sprayQuery->execute();
      echo 'Er wordt gesprayed';
    }
?>
