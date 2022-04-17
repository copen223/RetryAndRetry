namespace SpaceModule.PathfinderModule
{
    public enum MapCellType
    {
        Empty = 0,
        Ground = 1,
        Platform = 2,
        Ladder = 4,
       // EnemyActor  = 8,         // 被敌方单位占领，阻断移动
       // FriendActor = 16         // 被友方单位占领，不可停留
    }

    public enum MapCellActorType
    {
        None = 0,
        EnemyActor = 1,
        FriendActor = 2
    }

    public enum ObjectStayState
    {
        Stand,          // 能够站立，能够进行walk行为的唯一cell
        Block,          // 障碍
        Climb,
        Fall
    }

    public enum ObjectPassState
    {
        None,
        /// <summary>
        /// 可行走通过
        /// </summary>
        WalkPass,
        /// <summary>
        /// 不能穿过
        /// </summary>
        CantPass,
        /// <summary>
        /// 穿过敌方
        /// </summary>
        PassEnemy,
        /// <summary>
        /// 穿过友方
        /// </summary>
        PassFriend
    }

    public class MapCell
    {
        public MapCell((int,int)intPos)
        {
            IntPos = intPos;
        }

        public MapCellType Type;
        public MapCellActorType ActorType;
        public ObjectStayState StayState;
        public ObjectPassState PassState;
        // 该块是否因左右Actor碰撞被标记
        public bool IfActorOccupied;
        public bool IfCanHold;
        public (int, int) IntPos;
        public int height;
        public int fallHeight;  //  下降时的高度
    }
}
