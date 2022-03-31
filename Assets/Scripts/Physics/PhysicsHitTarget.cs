using Tools;
using UnityEngine;

namespace Physics
{
    [RequireComponent(typeof(BoxCollider2D))]
   public class PhysicsHitTarget : MonoBehaviour
   {
      private MyFactory<PhysicsHitTarget> factory;
      [SerializeField] private BoxCollider2D boxCollider;

      public void Init(MyFactory<PhysicsHitTarget> _factory, Vector2 size)
      {
          factory = _factory;
          boxCollider.size = size;
      }

      public void ReturnToFactory()
      {
          factory.RemoveTarget(this);
      }
      

   }
}
