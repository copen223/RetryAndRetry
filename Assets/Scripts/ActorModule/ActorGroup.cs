using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ActorModule
{
    public class ActorGroup
    {
        string groupName;
        int groupId;
        public GroupType type;

        public ActorGroup(string _name, int _id, GroupType _type)
        {
            groupName = _name;
            groupId = _id;
            type = _type;
        }

        public enum GroupType
        {
            Player,
            Enemy
        }

        public bool IsPlayer { get { return type == GroupType.Player; } }
        public bool IsEnemy { get { return type == GroupType.Enemy; } }
    }
}
