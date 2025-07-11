using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeMiniGames
{
    public class TriggerActivator : MonoBehaviour
    {
        public BaseController ArcadeController;

        private void OnTriggerEnter(Collider other)
        {
            ArcadeController.Activate();
        }
        private void OnTriggerExit(Collider other)
        {
            ArcadeController.Deactivate();
        }
    }
}
