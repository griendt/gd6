using gd6.Models;

namespace gd6.Engine.Commands;

public interface IHasOrigin
{
    public Territory Origin { get; set; }
}