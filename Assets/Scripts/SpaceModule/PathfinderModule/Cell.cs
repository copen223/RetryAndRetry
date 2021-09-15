using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SpaceModule.PathfinderModule
{
    public enum MapCellType
    {
        Empty,
        Ground,
        Ladder,
        Actor
    }

    public enum ObjectStayState
    {
        Stand,
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
