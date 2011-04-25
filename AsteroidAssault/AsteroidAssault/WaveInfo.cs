using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacepiXX
{
    class WaveInfo
    {
        public int SpawnsCount;
        public Enemy.EnemyType Type;

        public WaveInfo(int spawns, Enemy.EnemyType type)
        {
            this.SpawnsCount = spawns;
            this.Type = type;
        }

        public void DecrementSpawns()
        {
            this.SpawnsCount -= 1;
        }
    }
}
