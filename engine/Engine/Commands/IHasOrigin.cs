using gdvi.Models;

namespace gdvi.Engine.Commands;

public interface IHasOrigin
{
    public Territory Origin { get; set; }
}