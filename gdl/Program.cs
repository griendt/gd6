// See https://aka.ms/new-console-template for more information

// 
/*
A command set by a player should look like the following.
A move can have dashes at the end for comments.


Set Psycho17 // Start moveset for player Psycho17
Con 1 3A    // Construct 3 armies in territory 1
Con 1 For   // Construct a fortress in territory 1
Inv Dyn 1→2 // Throw a dynamite from territory 1 to territory 2
Mov 1→2→3   // Move a unit from 1 to 2 to 3
Mov 2→4     // Move a unit from 2 to 4
Mov 5→6 3A  // Move three armies from 5 to 6. Should expand into three separate MoveArmy moves with same(like) priority.
 */


Console.WriteLine("Hello, World!");