namespace engine.Models;

public enum Unit
{
    Army,
    Cavalry,
}


static class UnitExtensions
{
    public static int Health(this Unit unit) => 1;
    public static int Strength(this Unit unit) => 1;
    public static int Speed(this Unit unit) =>
        unit switch
        {
            Unit.Cavalry => 4,
            _ => 2,
        };
}