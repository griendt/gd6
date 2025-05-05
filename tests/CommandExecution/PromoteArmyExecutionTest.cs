using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandExecution;

public class PromoteArmyExecutionTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        T(1).Owner = Players.Player1;
        Players.Player1.InfluencePoints = 30;
    }

    [TestCase(1, 1)]
    [TestCase(3, 2)]
    public void ItPromotesAnArmyToCavalry(int numArmiesBefore, int numPromotions)
    {
        T(1).Units.AddArmies(numArmiesBefore);
        
        new PromoteArmy
        {
            Issuer = Players.Player1, 
            Origin = T(1), 
            UnitType = Unit.Cavalry,
            Quantity = numPromotions,
        }.Process(World);

        Assert.Multiple(() =>
        {
            Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(30 - 3 * numPromotions));
            Assert.That(T(1).Units.Armies, Is.EqualTo(numArmiesBefore - numPromotions));
            Assert.That(T(1).Units.Cavalries, Is.EqualTo(numPromotions));
        });
    }
}