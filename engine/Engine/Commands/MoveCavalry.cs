using engine.Models;

namespace engine.Engine.Commands;

public class MoveCavalry : MoveUnit
{
    public override Unit UnitType() => Unit.Cavalry;
    public override int MaxMovements() => 4;
}