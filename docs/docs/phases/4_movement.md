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

!!! info Resolution order
    Whenever possible, moves that *give no conflicts* are resolved before conflicting moves.
    And whenever relevant, Skirmishes are resolved before Invasions.

## Skirmishes
