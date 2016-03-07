<?php

$db['default']['hostname'] = 'sqlite:spray.sqlite';
$db['default']['username'] = '';
$db['default']['password'] = '';
$db['default']['database'] = '';
$db['default']['dbdriver'] = 'pdo';
$db['default']['dbprefix'] = '';
$db['default']['pconnect'] = TRUE;
$db['default']['db_debug'] = TRUE;
$db['default']['cache_on'] = FALSE;
$db['default']['cachedir'] = '';
$db['default']['char_set'] = 'utf8';
$db['default']['dbcollat'] = 'utf8_general_ci';
$db['default']['swap_pre'] = '';
$db['default']['autoinit'] = TRUE;
$db['default']['stricton'] = FALSE;

// Opens/Creates the spray.sqlite database file
$db = new PDO('sqlite:spray.sqlite');
$db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

// Creates a table named 'spray' in it, with one field: 'spray'
$createQuery = $db -> prepare('CREATE TABLE "spray"(spray INTEGER )');
$createQuery -> execute();

?>
