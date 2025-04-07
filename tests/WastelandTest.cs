using engine.Engine;

namespace tests;

public class WastelandTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        T(3).Owner = Players.Player3;
        T(3).IsWasteland = true;
    }

    [TestCase(1, 0, true)]
    [TestCase(3, 2, false)]
    [TestCase(47, 46, false)]
    [TestCase(0, 0, true)]
    public void ItRemovesArmyFromWasteland(int numBefore, int numAfter, bool shouldBeNeutralized)
    {
        T(3).Units.AddArmies(numBefore);

        new Turn
        {
            World = World,
            Commands = [],
        }.Process();

        Assert.Multiple(() =>
        {
            Assert.That(T(3).Units.Armies, Is.EqualTo(numAfter));
            Assert.That(T(3).Owner, shouldBeNeutralized ? Is.Null : Is.EqualTo(Players.Player3));
        });
    }
}