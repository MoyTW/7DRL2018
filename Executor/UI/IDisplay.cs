using Executor.Dungeon;
using RLNET;

namespace Executor.UI
{
    interface IDisplay
    {
        IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress);
        void Blit(RLConsole console);
    }

    interface IFloorDisplay : IDisplay
    {
        FloorState Floor { get; }
    }
}
