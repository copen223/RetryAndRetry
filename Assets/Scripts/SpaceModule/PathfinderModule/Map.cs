using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.SpaceModule.PathfinderModule
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
