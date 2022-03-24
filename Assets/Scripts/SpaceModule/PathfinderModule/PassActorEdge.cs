
using System;
using System.Collections.Generic;
using ActorModule.Core;

namespace SpaceModule.PathfinderModule
{
    [Serializable]
    public class PassActorEdge
    {
        public List<(int, int)> LeftNodePositions = new List<(int, int)>();
        public List<(int, int)> RightNodePositions = new List<(int, int)>();
        public ObjectPassState PassState;
        public ActorController Actor;
    }
}