using engine.Engine;

namespace tests.Phases;

public class NaturalPhaseTest : BaseTest
{
    [SetUp]
    public void SetUp()
    {
        T(1).Owner = Players.Player1;
        T(1).IsWasteland = true;
    }

    [TestCase(1, 0, true)]
    [TestCase(3, 2, false)]
    [TestCase(47, 46, false)]
    [TestCase(0, 0, true)]
    public void ItRemovesArmyFromWasteland(int numBefore, int numAfter, bool shouldBeNeutralized)
    {
        T(1).Units.AddArmies(numBefore);

        new Turn { World = World, Commands = [] }.Process();

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Units.Armies, Is.EqualTo(numAfter));
            Assert.That(T(1).Owner, shouldBeNeutralized ? Is.Null : Is.EqualTo(Players.Player1));
        });
    }

}