# Movement Phase
Each unit under your control can move to one adjacent land territory. There are a few different movement types that each have their own resolution rules:

* Units may move freely to territories that are under your control; this is called a *Distribution*. Nothing can stop or alter a *Distribution*.
* Units may also move freely to territories that currently contain no units; this is called an *Expansion*. If two or more players attempt to *Expand* into the same territory, they will *Skirmish*; see below for the details.
* Units may move to a territory that is under control of another player; this is called an *Invasion*. This is the most complex part of the game; see below for the details.
* If a player moves to the same destination from two or more origins, these movements will be processed **simultaneously**, and the units will be considered as one bigger "legion" for resolving *Skirmishes* and *Invasions*.
* If there are not enough units to move from the origin to fulfill the entire move order, then the maximum amount of available units will still proceed.
 
??? info "The gnarly details..."
    * Whenever relevant, *Skirmishes* are resolved before *Invasions*.
    * If a player moves from the same origin to two or more targets, they will be processed **one by one in the order they are provided**. In some cases I may ask you to explicitly specify the priorities.

## Skirmish
When two or more players are involved in a *Skirmish*, the following rules will resolve the conflict:

* For each involved player, remove simultaneously one *Army*.
* Repeat the above process until at most one player has any *Armies* remaining. These units, if any, will then proceed to occupy the target territory. If there is no winner, then the target territory remains untouched.

## Invasion
* Invading comes with a penalty, as the target has their homeland for defense. This penalty is `2` *Armies*. However, this penalty needs to be paid **only** if there is at least one defending unit.
* After this initial penalty, one *Army* can slay one enemy *Army*, until at most one party has any *Armies* remaining, just like in a *Skirmish*. 
* If the defender has no units remaining after the *Invasion* resolves, the territory is rendered neutral.
* If the attacker still has units left after the *Invasion* resolves, the remainder is considered an *Expansion* to the target.
* When *Expanding* to a target after a successful *Invasion*, any units or constructs remaining in the target will become ownership of the attacker.
* If the attacker also sends *Archers* in the attack, then they kill nothing as they cannot attack at melee range. They cannot be used for the penalty and they will not slay any enemy units.
??? example
    Suppose a player is launching an attack with `5` units on a territory that has `2` units. Then after paying the initial penalty, `3` attackers remain; they successfully slay the `2` with one unit remaining. Now the target is neutral and can freely be occupied with the last remaining unit.

??? example "Example with *Archers*"
    Suppose a player is attacking with `4` *Armies* and `3` *Archers*, and the defending side has `1` *Army* and `5` Archers.
    
    The attacker first pays a penalty of `2` *Armies*. The attacker then loses one more *Army* to kill the enemy *Army*.
    Now, the attacker has `1` *Army* left and its `3` *Archers*, while the defender has only his `5` *Archers*.

    These *Archers* cannot defend and the *Invasion* is successful. The attacker occupies the territory, which now contain `1` Army and `8` *Archers*.
    
    In particular, the attacker claims ownership of the leftover enemy *Archer* units!

* When two players attempt to *Invade* each other's territories at the same time (`A🠊B` + `B🠊A`), then this pair of orders is considered a *Skirmish* instead. After resolving this *Skirmish* it is possible that one of the two orders has units remaining. In that case, the remainder will continue as an *Invasion* to its target.
