using System;
using System.Collections.Generic;
using ActorModule.Core;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SpaceModule.PathfinderModule
{
    [Serializable]
    public class PassActorEdgeHandler
    {
        public readonly List<PassActorEdge> PassActorEdges = new List<PassActorEdge>();

        public PassActorEdge GetPassActorEdge(ActorController actor)
        {
            Debug.Log(actor);
            foreach (var edge in PassActorEdges)
            {
                if (edge.Actor == actor)
                    return edge;
            }
            
            var newEdge = new PassActorEdge(){Actor = actor};
            PassActorEdges.Add(newEdge);
            return newEdge;
        }
    }
}