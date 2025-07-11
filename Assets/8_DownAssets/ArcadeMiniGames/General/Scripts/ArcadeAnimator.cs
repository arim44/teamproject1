using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeMiniGames
{
    public class ArcadeAnimator : MonoBehaviour
    {
        public Transform Joystick;
        private const float maxAngle = 30;

        public Transform ContinueButton;
        public Transform AButton;
        public Transform BButton;
        public Transform XButton;
        public Transform YButton;
        private const float buttonDepth = 0.01f;
        private const float buttonReturnSpeed = 0.3f;

        private InputController inputController;

        void Start()
        {
            inputController = GetComponent<InputController>();
        }

        void Update()
        {
            Joystick.localRotation = Quaternion.Euler(-inputController.Vertical * maxAngle, 0, inputController.Horizontal * maxAngle);
            AnimateButton(ContinueButton, inputController.Continue);
            AnimateButton(AButton, inputController.A);
            AnimateButton(BButton, inputController.B);
            AnimateButton(XButton, inputController.X);
            AnimateButton(YButton, inputController.Y);
        }

        private void AnimateButton(Transform button, bool isPressed)
        {
            if (isPressed)
            {
                button.localPosition = new Vector3(0, -buttonDepth, 0);
            }
            else if (button.localPosition.y < 0)
            {
                button.localPosition += Vector3.up * buttonReturnSpeed * Time.deltaTime;
                if (button.localPosition.y > 0)
                {
                    button.localPosition = Vector3.zero;
                }
            }
        }
    }
}
