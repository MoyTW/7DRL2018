using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.GameQueries
{
    [Serializable()]
    public class GameQuery_Position : GameQuery
    {
        private int x, y, z;
        private bool blocksMovement;

        public int X { get { return x; } }
        public int Y { get { return y; } }
        public int Z { get { return z; } }
        public bool BlocksMovement { get { return blocksMovement; } }

        public void RegisterPosition(int x, int y, int z, bool blocksMovement)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.blocksMovement = blocksMovement;
            this.Completed = true;
        }
    }
}
