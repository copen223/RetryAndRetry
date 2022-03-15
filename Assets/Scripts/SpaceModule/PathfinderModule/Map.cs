using System.Collections.Generic;

namespace SpaceModule.PathfinderModule
{
    public class Map
    {
        public Dictionary<(int, int), MapCell> map_dic;
        public Map()
        {
            map_dic = new Dictionary<(int, int), MapCell>();
        }
    }
}
