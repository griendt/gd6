using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandExecution;

public class PromoteArmyExecutionTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        T(1).Owner = Players.Player1;
        Players.Player1.InfluencePoints = 300;
    }

    [TestCase(1, 1, Unit.Cavalry)]
    [TestCase(3, 2, Unit.Cavalry)]
    [TestCase(1, 1, Unit.Spy)]
    [TestCase(3, 2, Unit.Spy)]
    public void ItPromotesArmies(int numArmiesBefore, int numPromotions, Unit unitType)
    {
        T(1).Units.AddArmies(numArmiesBefore);
        
        new PromoteArmy
        {
            Issuer = Players.Player1, 
            Origin = T(1), 
            UnitType = unitType,
            Quantity = numPromotions,
        }.Process(World);

        Assert.Multiple(() =>
        {
            Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(300 - unitType.PromotionCost() * numPromotions));
            Assert.That(T(1).Units.Armies, Is.EqualTo(numArmiesBefore - numPromotions));
            Assert.That(T(1).Units.OfType(unitType), Is.EqualTo(numPromotions));
        });
    }
}