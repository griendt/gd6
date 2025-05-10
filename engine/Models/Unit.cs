namespace engine.Models;

public enum Unit
{
    Army,
    Cavalry,
    Heavy,
    Spy,
}

public static class UnitExtensions
{
    public static int Health(this Unit unit) =>
        unit switch
        {
            Unit.Heavy => 2,
            Unit.Spy => 0,
            _ => 1,
        };

    public static int Strength(this Unit unit) =>
        unit switch
        {
            Unit.Heavy => 1,
            Unit.Spy => 0,
            _ => 1,
        };

    public static int Speed(this Unit unit) =>
        unit switch
        {
            Unit.Cavalry => 4,
            Unit.Heavy => 1,
            Unit.Spy => 1,
            _ => 2,
        };

    public static int PromotionCost(this Unit unit) =>
        unit switch
        {
            Unit.Cavalry => 3,
            Unit.Heavy => 3,
            Unit.Spy => 30,
            _ => 0,
        };

    public static int MinimumPromotionLoyalty(this Unit unit) =>
        unit switch
        {
            Unit.Spy => 2,
            _ => 0,
        };

    public static IEnumerable<Unit> DefenseOrder()
    {
        yield return Unit.Spy;
        yield return Unit.Heavy;
        yield return Unit.Army;
        yield return Unit.Cavalry;
    }
}