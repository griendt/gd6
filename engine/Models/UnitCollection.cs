using System.Diagnostics.Contracts;
using engine.Engine.Exceptions;

namespace engine.Models;

public class UnitCollection
{
    private readonly Dictionary<Unit, int> _units = [];

    public int OfType(Unit unitType) => _units.GetValueOrDefault(unitType, 0);
    public int Armies => OfType(Unit.Army);
    public int Cavalries => OfType(Unit.Cavalry);
    
    public Unit Pop()
    {
        if (Armies > 0) {
            return Pop(Unit.Army);
        }

        if (Cavalries > 0) {
            return Pop(Unit.Cavalry);
        }

        throw new NoUnitToPop();
    }

    public Unit? TryPop()
    {
        try {
            return Pop();
        }
        catch (NoUnitToPop) {
            return null;
        }
    }

    public void Add(Unit unitType, int quantity = 1) => _units[unitType] = _units.GetValueOrDefault(unitType, 0) + quantity;
    public bool IsEmpty => _units.Values.Sum() <= 0;

    // Convenience methods
    public void AddArmy() => Add(Unit.Army);
    public void AddArmies(int quantity) => Add(Unit.Army, quantity);
    public void AddCavalry() => Add(Unit.Cavalry);
    public void AddCavalries(int quantity) => Add(Unit.Cavalry, quantity);
    
    public Unit Pop(Unit unitType)
    {
        if (_units.GetValueOrDefault(unitType, 0) <= 0) {
            throw new Exception("Cannot pop from this type");
        }

        _units[unitType]--;
        return unitType;
    }
}