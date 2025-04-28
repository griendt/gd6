using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandExecution;

public class UseDynamiteExecutionTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        T(1).Owner = Players.Player1;
    }

    [TestCase(1, 0)]
    [TestCase(47, 46)]
    [TestCase(0, 0)]
    public void ItKillsAUnit(int numBefore, int numAfter)
    {
        T(2).Units.AddArmies(numBefore);

        new UseDynamite { Issuer = Players.Player1, Origin = T(1), Target = T(2) }.Process(World);

        Assert.That(T(2).Units.Armies, Is.EqualTo(numAfter));
    }

    [TestCase(0)]
    [TestCase(1)]
    public void ItRendersTerritoryNeutralIfNoUnitsLeft(int numBefore)
    {
        T(2).Owner = Players.Player2;
        T(2).Units.AddArmies(numBefore);

        new UseDynamite { Issuer = Players.Player1, Origin = T(1), Target = T(2) }.Process(World);

        Assert.That(T(2).Owner, Is.Null);
    }

    [TestCase(Construct.Bivouac)]
    [TestCase(Construct.Watchtower)]
    [TestCase(Construct.Library)]
    public void ItDestroysConstructs(Construct construct)
    {
        T(2).Constructs.Add(construct);

        new UseDynamite { Issuer = Players.Player1, Origin = T(1), Target = T(2) }.Process(World);

        Assert.That(T(2).Constructs, Does.Not.Contain(construct));
    }
}