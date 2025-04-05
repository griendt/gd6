using engine.Models;

namespace engine.Engine;

public interface IProcessable
{
    public void Process(World world);
}