using gdvi.Models;

namespace gdvi.Engine.Commands;

public interface IHasTarget
{
    public Territory Target { get; set; }
}