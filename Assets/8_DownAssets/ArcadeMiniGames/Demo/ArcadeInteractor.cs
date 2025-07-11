using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeMiniGames
{
    /// <summary>
    /// For Demo purposes.
    /// Not intended to be used as is, but you're free to reuse any of the code here.
    /// </summary>
    public class ArcadeInteractor : MonoBehaviour
    {
        public Transform CenterPosition;
        public float radius;

        public string UseButton = "Use"; //This needs to be a valid input axis defined in the Input Manager, otherwise this will not work in a build
        public string QuitButton = "Use"; //This needs to be a valid input axis defined in the Input Manager, otherwise this will not work in a build

        public Transform CameraPosition;

        private BaseController arcadeController;

        private bool inTransition;
        [SerializeField]
        private float transitionDuration = 1;
        private float transitionProgress;
        private Vector3 startPos;
        private Quaternion startRot;

        public Text UseButtonDisplay;
        private bool wasInRange;

        private bool arcadeInUse;

        private InputController inputController;

        void Start()
        {
            inputController = GetComponent<InputController>();

            arcadeController = GetComponent<BaseController>();

            arcadeController.SetHiScores(9999, 1337, 99, 10); //just some values for demonstration purposes
            arcadeController.OnGameEnd += OnGameEnd; //also just for demonstration purposes

            UseButtonDisplay.gameObject.SetActive(false);

            inputController.InUse = false; //this makes the arcade unable to read button-presses
        }

        private void OnGameEnd(float score, bool newHighScore, int place)
        {            
            if (newHighScore)
            {
                Debug.Log("You got a new HighScore! Placement: " + place + ", score: " + score);
            }
        }

        void Update()
        {
            if (inTransition)
            {
                transitionProgress += Time.deltaTime / transitionDuration;
                if (arcadeInUse)
                {
                    Camera.main.transform.position = Vector3.Lerp(startPos, CameraPosition.position, transitionProgress);
                    Camera.main.transform.rotation = Quaternion.Lerp(startRot, CameraPosition.rotation, transitionProgress);
                    if (transitionProgress>=1)
                    {
                        //arcadeController.Activate();
                        inTransition = false;
                        Camera.main.transform.position = CameraPosition.position;
                        Camera.main.transform.rotation = CameraPosition.rotation;
                        SimplePlayer.Instance.UnlockCamera(CameraPosition.rotation);
                        UseButtonDisplay.gameObject.SetActive(true);
                        UseButtonDisplay.text = "Press E to quit";
                    }
                }
                else
                {
                    Camera.main.transform.position = Vector3.Lerp(startPos, CameraPosition.position, 1-transitionProgress);
                    if (transitionProgress >= 1)
                    {
                        //arcadeController.Deactivate();
                        inTransition = false;
                        Camera.main.transform.position = startPos;
                        UseButtonDisplay.gameObject.SetActive(true);
                        UseButtonDisplay.text = "Press E to use";
                        inputController.InUse = false; //this makes the arcade unable to read button-presses

                        //Enable the player character here, as well as any other thing you disabled when starting the arcade game
                        SimplePlayer.Instance.enabled = true;
                    }
                }
            }
            else if (!arcadeInUse)
            {
                if ((Camera.main.transform.position - CenterPosition.position).sqrMagnitude < radius * radius)
                {
                    if (!wasInRange)
                    {
                        wasInRange = true;
                        UseButtonDisplay.gameObject.SetActive(true);
                    }
                    if (GetButtonDown(UseButton))
                    {
                        inTransition = true;
                        transitionProgress = 0;
                        startPos = Camera.main.transform.position;
                        startRot = Camera.main.transform.rotation;
                        UseButtonDisplay.gameObject.SetActive(false);
                        inputController.InUse = true; //this allows the arcade to read button-presses
                        arcadeInUse = true;

                        //Disable the player character here, as well as any other thing you don't want active during the arcade game
                        SimplePlayer.Instance.enabled = false;
                        SimplePlayer.Instance.LockCamera();
                    }
                }
                else if (wasInRange)
                {
                    wasInRange = false;
                    UseButtonDisplay.gameObject.SetActive(false);
                }                    
            }
            else
            {
                if (GetButtonDown(QuitButton))
                {
                    inTransition = true;
                    transitionProgress = 0;
                    UseButtonDisplay.gameObject.SetActive(false);
                    arcadeInUse = false;
                }
            }
        }

        private bool GetButtonDown(string name)
        {
            bool result;
            try
            {
                result = Input.GetButtonDown(name);
            }
            catch (ArgumentException)
            {
                //For sake of the demo, we'll just use the E-button
                //But you should setup a correct buttonname to use
                result = Input.GetKeyDown(KeyCode.E);
            }
            return result;
        }
    }
}
