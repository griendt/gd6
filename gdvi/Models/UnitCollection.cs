namespace gdvi.Models;

public class UnitCollection
{
    private Dictionary<Unit, int> _units = [];

    public int Armies => _units.GetValueOrDefault(Unit.Army, 0);

    public Unit Pop()
    {
        if (Armies > 0) {
            return Pop(Unit.Army);
        }

        throw new Exception("Cannot pop");
    }

    public void Add(Unit unitType, int quantity = 1)
    {
        var current = _units.GetValueOrDefault(unitType, 0);
        _units[unitType] = current + quantity;
    }

    public bool IsEmpty => _units.Values.Sum() <= 0;

    // Convenience methods
    public void AddArmy() => Add(Unit.Army);
    public void AddArmies(int quantity) => Add(Unit.Army, quantity);

    private Unit Pop(Unit unitType)
    {
        if (_units.GetValueOrDefault(unitType, 0) <= 0) {
            throw new Exception("Cannot pop from this type");
        }

        _units[unitType]--;
        return unitType;
    }
}