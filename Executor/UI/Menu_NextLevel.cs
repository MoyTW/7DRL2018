using Executor.Dungeon;
using Executor.EntityBuilders;

using RLNET;
using System;

namespace Executor.UI
{
    class Menu_NextLevel : IDisplay
    {
        private Menu_Main mainMenu;
        private Menu_Floor floorMenu;
        private int nextWave;

        public Menu_NextLevel(Menu_Main mainMenu, Menu_Floor floorMenu, int nextWave)
        {
            this.mainMenu = mainMenu;
            this.floorMenu = floorMenu;
            this.nextWave = nextWave;
        }

        public IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress)
        {
            if (keyPress != null)
            {
                RogueSharp.Random.IRandom iRand = new RogueSharp.Random.DotNetRandom();

                return this.floorMenu;
            }
            else if (keyPress != null)
                return this.mainMenu;
            else
                return this;
        }

        public void Blit(RLConsole console)
        {
            if (this.nextWave< 10)
            {
                console.SetBackColor(0, 0, console.Width, console.Height, RLColor.Black);
                console.Print(console.Width / 2 - 19, console.Height / 2 - 1, "You have progressed to the next arena!", RLColor.White);
                console.Print(console.Width / 2 - 10, console.Height / 2 + 1, "The next level is " + this.nextWave, RLColor.White);
            }
            else
            {
                console.SetBackColor(0, 0, console.Width, console.Height, RLColor.Black);
                console.Print(console.Width / 2 - 15, console.Height / 2 - 1, "You have won! Congratulations!", RLColor.White);
            }
        }
    }
}
