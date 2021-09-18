using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SpaceModule.PathfinderModule
{
    public enum MapCellType
    {
        Empty = 0,
        Ground = 1,
        Platform = 2,
        Ladder = 4,
        Actor  = 8
    }

    public enum ObjectStayState
    {
        Stand,
        CantStand,
        Climb,
        Fall
    }

    public class MapCell
    {
        public MapCell((int,int)intPos)
        {
            IntPos = intPos;
        }

        public MapCellType Type;
        public ObjectStayState StayState;
        public (int, int) IntPos;
    }
}
