using gdvi.Models;

namespace gdvi.Engine;

public class Turn(World world)
{
    private static readonly Phase[] Phases = [Phase.Natural, Phase.Inventory, Phase.Construction, Phase.Movement, Phase.Final];
    public List<Command> Commands = [];

    public void Process()
    {
        foreach (var phase in Phases) {
            ProcessPhase(phase);
        }
    }

    private void ProcessPhase(Phase phase)
    {
        throw new NotImplementedException();
    }
}