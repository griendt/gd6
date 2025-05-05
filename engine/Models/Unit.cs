namespace engine.Models;

public enum Unit
{
    Army,
    Cavalry,
}


static class UnitExtensions
{
    public static int Health(this Unit unit) => 1;
    public static int Damage(this Unit unit) => 1;
}