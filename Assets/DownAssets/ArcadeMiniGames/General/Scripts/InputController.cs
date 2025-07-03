using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeMiniGames
{
    //The InputController uses Unity's old Input system, 
    //in order for this to work the "Active Input Handling" in Project Settings > Player > Other Settings > Configuration needs to be set to either "Input Manager (Old)" or "Both"
    //all of the strings here need to be a valid input axis defined in the Input Manager.

    public class InputController : MonoBehaviour
    {
        [SerializeField]
        private string ContinueButton = "Submit"; //by default this is Enter
        [SerializeField]
        private string QuitButton = "Cancel"; //by default this is Escape
        [SerializeField]
        private string AButton = "Fire1"; //by default this is Left Control
        [SerializeField]
        private string BButton = "Jump"; //by default this is space
        [SerializeField]
        private string XButton = "Fire3"; //by default this is Left Shift
        [SerializeField]
        private string YButton = "Fire2"; //by default this is Left Alt

        [SerializeField]
        private bool useAxis = true;
        [SerializeField]
        private string HorizontalAxis = "Horizontal";
        [SerializeField]
        private string VerticalAxis = "Vertical";
        [SerializeField]
        private string LeftButton;
        [SerializeField]
        private string RightButton;
        [SerializeField]
        private string UpButton;
        [SerializeField]
        private string DownButton;

        [HideInInspector]
        public bool Continue;
        [HideInInspector]
        public bool Quit;
        [HideInInspector]
        public bool A;
        [HideInInspector]
        public bool B;
        [HideInInspector]
        public bool X;
        [HideInInspector]
        public bool Y;

        [HideInInspector]
        public bool Left;
        [HideInInspector]
        public bool Right;
        [HideInInspector]
        public bool Up;
        [HideInInspector]
        public bool Down;

        //the start-versions are only true during the frame when the player started pressing the button:
        [HideInInspector]
        public bool ContinueStart;
        [HideInInspector]
        public bool QuitStart;
        [HideInInspector]
        public bool AStart;
        [HideInInspector]
        public bool BStart;
        [HideInInspector]
        public bool XStart;
        [HideInInspector]
        public bool YStart;

        [HideInInspector]
        public bool LeftStart;
        [HideInInspector]
        public bool RightStart;
        [HideInInspector]
        public bool UpStart;
        [HideInInspector]
        public bool DownStart;

        [HideInInspector]
        public float Horizontal;
        [HideInInspector]
        public float Vertical;

        private bool inUse; //inUse means the player is using the arcade
        /// <summary>
        /// This enables/disables the arcade to read button-presses
        /// </summary>
        public bool InUse 
        {
            get
            {
                return inUse;
            }
            set
            {
                inUse = value;
                if (!inUse)
                {
                    Continue = false;
                    Quit = false;
                    A = false;
                    B = false;
                    X = false;
                    Y = false;
                    Left = false;
                    Right = false;
                    Up = false;
                    Down = false;
                    ContinueStart = false;
                    QuitStart = false;
                    AStart = false;
                    BStart = false;
                    XStart = false;
                    YStart = false;
                    LeftStart = false;
                    RightStart = false;
                    UpStart = false;
                    DownStart = false;
                    Horizontal = 0;
                    Vertical = 0;
                }
            }
        }

        public bool StartInUse;

        void Awake()
        {
            InUse = StartInUse;
        }

        void Update()
        {
            if (InUse)
            {
                Continue = Input.GetButton(ContinueButton);
                Quit = Input.GetButton(QuitButton);
                A = Input.GetButton(AButton);
                B = Input.GetButton(BButton);
                X = Input.GetButton(XButton);
                Y = Input.GetButton(YButton);

                ContinueStart = Input.GetButtonDown(ContinueButton);
                QuitStart = Input.GetButtonDown(QuitButton);
                AStart = Input.GetButtonDown(AButton);
                BStart = Input.GetButtonDown(BButton);
                XStart = Input.GetButtonDown(XButton);
                YStart = Input.GetButtonDown(YButton);

                if (useAxis)
                {
                    Horizontal = Input.GetAxis(HorizontalAxis);
                    Vertical = Input.GetAxis(VerticalAxis);
                    var horizontalRaw = Input.GetAxisRaw(HorizontalAxis);
                    var verticalRaw = Input.GetAxisRaw(VerticalAxis);
                    var newLeft = horizontalRaw < 0;
                    var newRight = horizontalRaw > 0;
                    var newUp = verticalRaw > 0;
                    var newDown = verticalRaw < 0;
                    SetDirectionalInput(newLeft, newRight, newUp, newDown);
                }
                else
                {
                    var newLeft = Input.GetButton(LeftButton);
                    var newRight = Input.GetButton(RightButton);
                    var newUp = Input.GetButton(UpButton);
                    var newDown = Input.GetButton(DownButton);
                    SetDirectionalInput(newLeft, newRight, newUp, newDown);
                    if (Left == Right) Horizontal = 0;
                    else if (Left) Horizontal = -1;
                    else Horizontal = 1;
                    if (Up == Down) Vertical = 0;
                    else if (Down) Vertical = -1;
                    else Vertical = 1;
                }
            }
        }

        private void SetDirectionalInput(bool newLeft, bool newRight, bool newUp, bool newDown)
        {
            if (newLeft && !Left)
            {
                LeftStart = true;
            }
            else
            {
                LeftStart = false;
            }
            if (newRight && !Right)
            {
                RightStart = true;
            }
            else
            {
                RightStart = false;
            }
            if (newUp && !Up)
            {
                UpStart = true;
            }
            else
            {
                UpStart = false;
            }
            if (newDown && !Down)
            {
                DownStart = true;
            }
            else
            {
                DownStart = false;
            }
            Left = newLeft;
            Right = newRight;
            Up = newUp;
            Down = newDown;
        }
    }
}
