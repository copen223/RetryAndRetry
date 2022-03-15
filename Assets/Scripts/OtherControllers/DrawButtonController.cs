using CardModule.Controllers;
using UnityEngine;

namespace OtherControllers
{
    public class DrawButtonController : MonoBehaviour
    {
        public GameObject Hand;
        public void Draw()
        {
            HandController.instance.DrawCard();
        }

        public void Return()
        {
            HandController.instance.ReturnCard();
        }

    }
}
