﻿namespace SpaceModule.PathfinderModule
{
    public enum MapCellType
    {
        Empty = 0,
        Ground = 1,
        Platform = 2,
        Ladder = 4,
        EnemyActor  = 8,         // 被敌方单位占领，阻断移动
        FriendActor = 16         // 被友方单位占领，不可停留
    }

    public enum ObjectStayState
    {
        Stand,          // 能够站立，能够进行walk行为的唯一cell
        CantHold,       // 在空中，而且空间不足，不能容纳，作为横向跳跃的阻断信息之一
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
        CantPass
    }

    public class MapCell
    {
        public MapCell((int,int)intPos)
        {
            IntPos = intPos;
        }

        public MapCellType Type;
        public ObjectStayState StayState;
        public ObjectPassState PassState;
        public (int, int) IntPos;
        public int height;
        public int fallHeight;  //  下降时的高度
    }
}
