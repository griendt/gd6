# Movement Phase

This phase is where the magic happens: units can move across the map. The basics are simple:

1. Each unit may move up to a certain amount of _steps_ per Turn. See the Speed stat of the [Unit types](#unit-types) below.
2. Each step, all units (from all players) move simultaneously.
3. Whenever a unit successfully moves into a territory, you gain ownership of that territory.
4. Back-tracking is allowed (e.g. `A→B→A`).
5. Whenever relevant, the move you mention _first_ in your move list is considered more important, therefore executed first.

It is possible -- even likely -- that players will submit _conflicting_ moves. There are two types of mechanics for this:

1. **Skirmish**: is _symmetrical_ (penalties are the same for all involved players). A set of moves becomes a *Skirmish* when:
    - two or more players try to move into a territory that none of them own; **OR**
    - there is a closed loop of attacks (for example, `A→B`, `B→C`, `C→D`, `D→A`).
      This includes the case where two players attack each other directly (`A→B` and `B→A`).
2. **Invasion**: is _asymmetrical_ (there is an _attacker_ and a _defender_). A move is an *Invasion* when:
    - A player tries to move into a territory that is owned by another player.
    
Any other move is considered a **Distribution** (to already owned territories or expansions to new ones).

!!! info "Resolution order"
    * Whenever possible, moves that *give no conflicts* are resolved before conflicting moves.
    * Whenever relevant, Skirmishes are resolved before Invasions.

!!! info "Territory loyalty"
    Whenever a territory is neutralized or overtaken, its Loyalty will be immediately reset to `0`.
    Loyalty is relevant to determine which territories have belonged to a player the longest; see [Construction Phase](2_construction.md) for more details.

## Unit types

Each unit type has two statistics: health and speed. The table is as follows:

| Unit    | Health | Speed |
|---------|--------|-------|
| Army    | 1      | 2     |

[//]: # (| Cavalry | 1      | 4     |)

[//]: # (| Heavy   | 2      | 1     |)

[//]: # (| Spy     | 0      | 1     |)

## Distribution

In a Distribution, the units simply move from the origin to the target.
If the target did not yet belong to you, then you will gain ownership of it immediately after the move.

[//]: # (However, if there are Landmines present in the target, and the target is not already owned by the player, then Landmines will trigger, one-by-one, dealing `1` damage each, until either there are no more units or Landmines left.)

[//]: # (For example, if a neutral territory contains `3` mines and a player sends `5` armies, then the mines kill three of them and only two enter the land.)

[//]: # (As another example, if the neutral territory contains `7` mines instead, then five of them will detonate, killing all the armies, and another `2` mines remain dormant.)

## Skirmishes

A skirmish is resolved quite simply according to these rules:

1. Damage is applied to whichever unit's movement had the highest priority (i.e. first stated in the move list).
2. Each player's unit suffers one damage. If the unit has `0` health, it is killed immediately and the next unit suffers the damage instead.
3. If there are still two or more players involved in the skirmish, repeat steps 1 and 2, until at most one player is left.

??? "Neutral resolution"
    It is possible that two players send the same units into a skirmish.
    This effectively means they will neutralize each other. It also means that the destination(s) that these units were intending to go to, will remain untouched.

    For example, if two players attempt to occupy the same neutral territory using the same amount of Armies,
    then after the skirmish the territory will still be neutral.

??? "Units remaining after skirmish"
    It is possible that one player "wins" the skirmish and still has units left after the skirmish is resolved. 
    Then these units will proceed to move as usual. This can be an expansion or distribution (no further battle) or an Invasion.

[//]: # ()
[//]: # (??? question "How do Spies work in a Skirmish?")

[//]: # (    Spies have no health. This means they effectively cannot fight. In any conflict, such as *Skirmish*, they effectively do nothing except die immediately.)

[//]: # (    For example, if one player sends five Spies and the other player an Army, then all the Spies are killed and the Army survives!)

## Invasion

An invasion is resolved by following these steps in order:

1. The attacker will suffer `2` damage as an invasion penalty before being able to deal any damage.

[//]: # (2. If the defender has Landmines, then Landmines will explode one-by-one, dealing `1` damage each, until either there are no attackers or no Landmines left.)
3. The attacker and defender both suffer `1` damage simultaneously. Repeat this step until at most one party has any units left.

* If the defending units are all defeated, the territory becomes neutral. If the target contains a Bivouac, it is destroyed.
* If the invader still has units left, they will proceed to move as usual.
* Some mechanics alter the way an Invasion is processed, such as a Watchtower, as described below.

[//]: # ()
[//]: # (??? question "What if there are multiple unit types in the target territory?")

[//]: # (    If there are multiple unit types in the target territory, there is no player-defined "priority" like there is in a Skirmish.)

[//]: # (    In this case, it is assumed that the defender's units are lined up in this predetermined order:)

[//]: # (    )
[//]: # (    1. Spy units;)

[//]: # (    2. Heavy units;)

[//]: # (    3. Army units;)

[//]: # (    4. Cavalry units.)

[//]: # (   )
[//]: # (    This makes Heavy units an excellent defensive choice &#40;especially to protect your Cavalry&#41;, and spies a dangerous choice to leave too close to enemy lines.)

[//]: # ()
[//]: # (!!! example)

[//]: # (    Suppose an attacker is attacking with 3 Armies and 1 Heavy unit &#40;in that order&#41; and the defense contains 1 Heavy unit. )

[//]: # (    Then the Invasion is resolved as follows:)

[//]: # (    )
[//]: # (    1. The attacker suffers two damage as invasion penalty, killing the first 2 Army units.)

[//]: # (    2. The last attacking Army is killed while damaging the Heavy unit.)

[//]: # (    3. The attacking Heavy unit and the wounded defending Heavy unit both suffer damage. The defender is now defeated. The attacker has a wounded Heavy unit left that occupies the target territory.)

[//]: # ()
[//]: # (??? question "What happens to wounded units after battle resolution?")

[//]: # (    It is possible that a Heavy unit gets `1` damage only. This means it will not be slain. In any future moves &#40;even in the same Turn&#41;,)

[//]: # (    the unit will regain its full health. In other words, the sustained damage applies only for each Invasion or Skirmish individually.)

### Watchtower

If the defending player has a Watchtower on their territory, then that Watchtower must first be broken through. In order to do this,
the attacker is inflicted `2` additional damage (in addition to the regular invasion penalty).

* If the attacker sends enough units to break through the Watchtower, then the Watchtower is destroyed.
* If the attacker still has armies left, they will proceed to invade as usual.

!!! info "Watchtower in a neutral territory"
    If a Watchtower stands on a neutral land, then there is no need to break the Watchtower first in order to occupy the territory.
    So, the Watchtower penalty only applies to invasions.

!!! example
    Suppose an attacker sends `6` armies to a territory that contains a Watchtower and one defending army. Then the invasion proceeds as follows (and in this order):
    
    1. The attacker pays a penalty of `2` armies, so that `4` will continue the attack;
    2. The attacker pays an additional penalty of `2` armies to destroy the Watchtower, and has `2` left to attack;
    3. One attacking army dies to slay the defending unit. The target becomes neutral and the attacker now has `1` army left.
    4. The last remaining army occupies the target territory. The attack was successful!

??? question "What happens to a partially damaged Watchtower?"
    It is possible that a Watchtower gets `1` damage only, for example if the attacker sent only `3` Armies.
    In that case, the Watchtower is left standing. In any future moves (even in the same Turn), 
    the Watchtower will regain its full integrity. In other words, the sustained damage applies only for each Invasion or Skirmish individually.

[//]: # ()
[//]: # (### Spies)

[//]: # ()
[//]: # (If the attacker has a Spy unit on their territory, or on a territory adjacent to it, then the attacking units can be provided with military intelligence.)

[//]: # (This allows them to operate more efficiently. As a result, the initial penalty for invasion will no longer apply.)

[//]: # (Military intelligence does *not* cancel the Watchtower perks.)

[//]: # ()
[//]: # (??? example)

[//]: # (    Suppose an attacker sends `6` armies to a territory that contains a Watchtower and one defending army. )

[//]: # (    Suppose also that the origin of the attack is adjacent to territory with a Spy, that is under control of the attacker.)

[//]: # (    Then the invasion proceeds as follows &#40;and in this order&#41;:)

[//]: # ()
[//]: # (    1. The attacker *does not* pay an initial penalty &#40;intelligence effect&#41;;)

[//]: # (    2. The attacker *does* pay a penalty of `2` armies to destroy the Watchtower, and has `4` left to attack;)

[//]: # (    3. One attacking army dies to slay the defending unit. The target becomes neutral and the attacker now has `3` armies left.)

[//]: # (    4. These `3` armies occupy the target territory. The attack was very successful!)