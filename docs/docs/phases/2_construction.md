# Construction Phase

## Building an HQ
This must be the first thing any player does in their first *Turn* playing the game. This is the only way to start playing.

An *HQ* may be built on any land territory, so long as:

* the territory does not already contain an *HQ*;
* the territory does not neighbour an existing *HQ* or a territory that itself neighbours an existing *HQ*;
* the territory is currently neutral;
* the player does not already have an *HQ*.

??? warning "Clashes with other players"
    Unlike previous GD games, building an *HQ* may interfere with other players' orders. Specifically, it can interfere with other players building *HQ*s. To ensure that two *HQ*s will always have at least `2` distance between each other, the following extra rules apply.
    
    * If two players try to build an *HQ* in the same territory, neither order will succeed.
    * If two players try to build *HQ*s in adjacent territories, neither order will succeed.
    * If two players try to build *HQ*s in territories which share a common neighbour, neither order will succeed.

??? question "What if *HQ* building fails?"
    Normally, failed orders are simply discarded (as if they were not given). Building *HQ*s forms an exception to this. Otherwise, some players may start expanding already when others cannot, which gives them an unfair early game advantage. Instead, the following will happen:

    * Any *HQ*s that can be built without problems, will be built and a map with the results will be made public. All orders will be made public also (even those that are in conflict). Orders in conflict are not processed. You can think of this as a separation of the *Turn* into multiple turns.
    * Players that failed to build an *HQ* can now choose a location again, given the new state of the map. This does not need to be a different location than the initial order, as long as it obeys all other rules for building *HQ*s. These players may also adjust their other orders if necessary.
    * If there are conflicts again after this, the above process repeats, until all *HQ*s are built.
    * After all *HQ*s are built, the rest of the *Turn* proceeds normally.

## Spawning units
Each *Turn*, each player may spawn a certain amount of *Army* units. These units may be spawned across one or more land territories that:

* contain an *HQ* that is under your control; **OR**
* are adjacent to an *HQ* that is under your control, and are not controlled by anyone **else**.

This means that spawning on a neutral empty land next to your *HQ* is allowed. This will automatically grant you ownership of it. 
Furthermore, if you already own the land next to your *HQ* and have some units stationed there, you may add additional ones.

!!! tip
    This is why *HQ*s must always have distance `2` or more between them, to avoid *Skirmishes* during unit spawning.

* The amount of units that you may spawn equals `2` plus `1` for each `3` territories that are under your control at the start of this *Turn*. That means in the very beginning you will have only `2` units, and if you own `11` territories, you will be able to spawn `5` units.

## Fortress

At the expense of `20` Influence Points, you can build a Fortress on a land territory that:

1. is under your control;
2. does not already contain some construct (other than *HQ*);
3. is not a [Toxic Wasteland](/phases/1_natural/#toxic-wasteland).

A fortress will give you an extra line of defense; see the [Fortress rules](/phases/4_movement/#fortress).

## Bivouac

At the expense of `30` Influence Points, plus an additional `10` Influence Points for each Bivouac under your control, you can build a Bivouac on a land territory that:

1. is under your control;
2. does not already contain some construct (other than *HQ*);
3. is not a [Toxic Wasteland](/phases/1_natural/#toxic-wasteland).

A Bivouac will increase your influx of Armies; see the [Bivouac rules](/phases/1_natural/#bivouac).