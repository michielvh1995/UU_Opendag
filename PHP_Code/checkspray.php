<?php
  // Opens the spray.sqlite database file
  $db = new PDO('sqlite:spray.sqlite');
  $db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

  $readQuery = $db -> prepare('SELECT COUNT(*) FROM [spray] WHERE [spray] == "1"');

  $readQuery -> execute();
  $rij = $readQuery->fetch();

  if ($rij[0] != NULL)
  {
    echo($rij[0]);
  }

  $deleteQuery = $db -> prepare('DELETE FROM [spray] WHERE [spray] == "1"');
  $deleteQuery -> execute();

?>
