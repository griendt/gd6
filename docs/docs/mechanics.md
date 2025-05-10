# Game Mechanics

## The important stuff
1. Each *Turn*, all living players must send me their orders. When the *Turn* is over, all these orders are processed simultaneously.
2. Orders are **only** valid if they are sent privately to me. Orders may (also) be posted elsewhere, including in public, but they will be ignored in processing. You might want this to inform others, or to deceive them. Bluffing and lying is allowed. It's all part of the game. Playing dirty is encouraged!
3. Each *Turn* consists of a number of *Phases* that are processed sequentially. This is required to have a deterministic outcome. See below.

## The Phases
1. [Natural Phase](phases/1_natural.md): Anything that happens to the map independently of any orders. Tides, disasters, aliens... who knows?
2. [Construction Phase](phases/2_construction.md): Players may spawn units and build *Constructs*.
3. [Inventory Phase](phases/3_inventory.md): Players may use one or more of their available items.
4. [Movement Phase](phases/4_movement.md): The most complex step, this involves the movement of all units. Movements can be *Expansions* (to new territory) and may cause *Skirmishes* and *Invasions*.
5. [Final Phase](phases/5_final.md): Any rewards or other things that occur at the end of the *Turn* belongs to this *Phase*. Players cannot make any orders in this *Phase*.

!!! warning "Spawning units is construction"
    Spawning units also occurs during *Construction Phase*, unlike previous GD iterations. Also, there is no separate *Battle Phase* anymore. *Invasions* may now occur before *Expansions* and vice versa.

!!! warning "Phase order change"
    The Inventory and Construction phases have swapped positions compared to previous GD games.
    In most cases this is not relevant, but it may be important sometimes. For example, buildings that are being built can be destroyed the same *Turn* by a Dynamite!

??? note "The little details..."
    1. At the start of each *Turn* I will specify the maximum duration of that *Turn*. The *Turn* may end sooner, in case all living players have sent me a finalized set of orders. This may speed up the game a bit.
    2. I will give feedback on your orders and request you to correct them if there are any invalid orders, or if they do not make sense. This feedback may be shorter or lacking if you send them very close to the *Turn* deadline. So please, as a rule of thumb, send some draft moves well beforehand!
    3. If I point out errors or invalid moves to a player before the *Turn* deadline and the moves are not corrected within a reasonable time, I may choose to end the *Turn* anyway and discard any invalid moves.
    4. If, after the *Turn* ends it turns out that some moves were invalid (but I did not spot it before), I may choose to extend the *Turn* instead and allow the issuer to fix them.
    5. Sometimes, real life gets in the way. If a player requests for an extension of the *Turn* deadline, it will be granted, provided it is done reasonably before the deadline and the delay won't be too long.
    6. If a player does not send any moves before the *Turn* deadline, with no notification, I may end the *Turn* anyway. This will effectively mean that the player will do **absolutely nothing** that *Turn*. This is highly discouraged and might severely impact the game for all players. Please don't do this.