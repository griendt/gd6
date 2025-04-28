# Movement Phase

This phase is where the magic happens: units can move across the map. The basics are simple:

1. Each unit may move up to `2` _steps_ per Turn.
2. Each step, all units (from all players) move simultaneously.
3. Whenever a unit successfully moves into a territory, you gain ownership of that territory.
4. Back-tracking is allowed (e.g. `A→B→A`).
5. Whenever relevant, the move you mention _first_ in your move list is considered more important, therefore executed first.

It is possible -- even likely -- that players will submit _conflicting_ moves. There are two types of mechanics for this:

1. **Skirmish**: is _symmetrical_ (penalties are the same for all involved players). A set of moves becomes a *Skirmish* when:
    - two or more players try to move into a territory that none of them own; **OR**
    - there is a closed loop of attacks (for example, `A→B`, `B→C`, `C->D`, `D→A`).
      This includes the case where two players attack each other directly (`A→B` and `B→A`).
2. **Invasion**: is _asymmetrical_ (there is an _attacker_ and a _defender_). A move is an *Invasion* when:
    - A player tries to move into a territory that is owned by another player.

!!! info "Resolution order"
    * Whenever possible, moves that *give no conflicts* are resolved before conflicting moves.
    * Whenever relevant, Skirmishes are resolved before Invasions.

!!! info "Territory loyalty"
    Whenever a territory is neutralized or overtaken, its Loyalty will be immediately reset to `0`.
    Loyalty is relevant to determine which territories have belonged to a player the longest; see [Construction Phase](2_construction.md) for more details.

## Skirmishes

A skirmish is resolved quite simply: each player involved in the skirmish loses one army.
If there are still two or more players involved in the skirmish, repeat, until at most one player is left.

??? "Neutral resolution"
    It is possible that two players send the same amount of units into a skirmish.
    This effectively means they will neutralize each other. It also means that the destination(s) that these units were intending to go to, will remain untouched.

    For example, if two players attempt to occupy the same neutral territory using the same amount of Armies,
    then after the skirmish the territory will still be neutral.

??? "Units remaining after skirmish"
    It is possible that one player "wins" the skirmish and still has units left after the skirmish is resolved. 
    Then these units will proceed to move as usual. This can be an expansion or distribution (no further battle) or an Invasion.

## Invasion

An invasion comes at the cost of a certain penalty: the attacker will lose two Armies before being able to deal any damage.

After this initial penalty, an Invasion goes similarly to a skirmish: both the attacker and the defender lose one Army each until at most one player is left standing.

* If the defending armies are all defeated, the territory becomes neutral. If the target contains a Bivouac, it is destroyed.
* If the invader still has armies left, they will proceed to move as usual.
* Some mechanics alter the way an Invasion is processed, such as a Watchtower, as described below.

### Watchtower

If the defending player has a Watchtower on their territory, then that Watchtower must first be broken through. In order to do this,
the attacker has to lose two Armies (in addition to the regular invasion penalty).

* If the attacker sends enough units to break through the Watchtower, then the Watchtower is destroyed.
* If the attacker still has armies left, they will proceed to invade as usual.

??? example
    Suppose an attacker sends `6` armies to a territory contains a Watchtower and one defending army. Then the invasion proceeds as follows (and in this order):
    
    1. The attacker pays a penalty of `2` armies, so that `4` will continue the attack;
    2. The attacker pays an additional penalty of `2` armies to destroy the Watchtower, and has `2` left to attack;
    3. One attacking army dies to slay the defending unit. The target becomes neutral and the attacker now has `1` army left.
    4. The last remaining army occupies the target territory. The attack was successful!

??? tip "Watchtower in a neutral territory"
    If a Watchtower stands on a neutral land, then there is no need to break the Watchtower first in order to occupy the territory.
    So, the Watchtower penalty only applies to invasions.