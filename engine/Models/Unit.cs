namespace engine.Models;

public enum Unit
{
    Army,
    Cavalry,
    Heavy,
}

internal static class UnitExtensions
{
    public static int Health(this Unit unit) => 1;
    public static int Strength(this Unit unit) => 1;
    public static int Speed(this Unit unit) =>
        unit switch
        {
            Unit.Cavalry => 4,
            Unit.Heavy => 1,
            _ => 2,
        };
    public static int PromotionCost(this Unit unit) =>
        unit switch
        {
            Unit.Cavalry => 3,
            Unit.Heavy => 3,
            _ => 0,
        };
}