using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeMiniGames
{
    public abstract class BaseController : MonoBehaviour
    {
        [SerializeField]
        private GameObject GameContainer;

        public GameObject gameOverObject;
        protected bool gameOver;
        public GameObject newHiscoreObject;

        public GameObject PlayField;
        public GameObject StartScreen;
        public GameObject HighscoreScreen;
        public Number[] HiScores;

        public Color NewHighScoreColor = new Color(0.18f, 1, 0, 1);

        public Number Score;

        public AudioClip ButtonSound;
        public AudioClip DieSound;
        public AudioSource Music;

        protected InputController inputController;

        public bool StartActivated;
        public bool activated { get; private set; } //activated means it's turned on

        protected bool playing;
        protected bool waiting;

        /// <summary>
        /// Gets called when GameOver
        /// </summary>
        /// <param name="score">the player's score</param>
        /// <param name="newHighScore">if the player managed to get a new HighScore</param>
        /// <param name="place">placement in the rankings (1 means 1st place, 2 is 2nd place, ...), will be -1 if player failed to get a new highscore</param>
        public delegate void GameEndAction(float score, bool newHighScore, int place);
        public event GameEndAction OnGameEnd;

        protected void Start()
        {
            inputController = GetComponent<InputController>();

            if (StartActivated)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        public void Activate()
        {
            if (gameOver || waiting)
            {
                StartCoroutine(ActivateAfterGameOver());
            }
            else
            {
                GameContainer.SetActive(true);
                gameOverObject.SetActive(false);
                HighscoreScreen.SetActive(false);
                StartScreen.SetActive(true);
                newHiscoreObject.SetActive(false);
                PlayField.SetActive(false);

                activated = true;

                OnActivate();
            }
        }

        protected virtual void OnActivate()
        {
        }

        public IEnumerator ActivateAfterGameOver()
        {
            while (gameOver || waiting)
            {
                yield return null;
            }
            Activate();
        }

        public void Deactivate()
        {
            if (playing)
            {
                StartCoroutine(GameOver(true));
            }
            activated = false;
            GameContainer.SetActive(false);
        }

        protected IEnumerator GameOver(bool forceQuit = false, AudioClip sfx = null)
        {
            playing = false;
            gameOver = true;

            if (forceQuit)
            {
                if (Music)
                {
                    Music.Stop();
                }
            }
            else
            {
                if (sfx)
                {
                    AudioSource.PlayClipAtPoint(sfx, Music.transform.position, 0.45f);
                }
                else if (DieSound)
                {
                    AudioSource.PlayClipAtPoint(DieSound, Music.transform.position, 0.45f);
                }

                if (Music)
                {
                    float currentTime = 0;
                    float startVolume = Music.volume;
                    float factor;
                    while (currentTime < 0.75f)
                    {
                        currentTime += Time.deltaTime;
                        factor = currentTime / 0.75f;
                        factor = 1 - factor;
                        Music.volume = factor * factor * startVolume;
                        yield return null;
                    }
                    Music.Stop();
                    Music.volume = startVolume;
                }
                else yield return new WaitForSeconds(0.75f);

                gameOverObject.SetActive(true);

                for (int i = 0; i < HiScores.Length; i++)
                {
                    HiScores[i].SetColor();
                }

                yield return new WaitForSeconds(1.25f);

                int ranking = -1;
                for (int i = 0; i < HiScores.Length; i++)
                {
                    if (Score.Value > HiScores[i].Value)
                    {
                        ranking = i + 1;
                        for (int j = HiScores.Length - 1; j > i; j--)
                        {
                            HiScores[j].Value = HiScores[j - 1].Value;
                        }
                        HiScores[i].Value = Score.Value;
                        HiScores[i].SetColor(NewHighScoreColor);
                        newHiscoreObject.SetActive(true);
                        yield return new WaitForSeconds(1.0f);
                        break;
                    }
                }
                OnGameEnd?.Invoke(Score.Value, ranking > 0, ranking);

                yield return new WaitForSeconds(0.75f);
            }

            OnGameOver();

            PlayField.SetActive(false);
            gameOverObject.SetActive(false);
            newHiscoreObject.SetActive(false);
            HighscoreScreen.SetActive(true);

            gameOver = false;
        }

        public void SetHiScores(params int[] score)
        {
            for (int i = 0; i < score.Length && i < HiScores.Length; i++)
            {
                HiScores[i].Value = score[i];
            }
        }

        public int[] GetHiScores()
        {
            int[] scores = new int[HiScores.Length];
            for (int i = 0; i < HiScores.Length && i < HiScores.Length; i++)
            {
                scores[i] = HiScores[i].Value;
            }
            return scores;
        }

        void LateUpdate()
        {
            if (!activated)
            {
                return;
            }

            if (playing)
            {
                PlayUpdate();
            }
            else if (!gameOver && inputController.ContinueStart)
            {
                if (ButtonSound)
                {
                    AudioSource.PlayClipAtPoint(ButtonSound, Music.transform.position, 0.75f);
                }
                if (HighscoreScreen.activeSelf)
                {
                    HighscoreScreen.SetActive(false);
                    StartScreen.SetActive(true);
                }
                else
                {
                    playing = true;
                    StartScreen.SetActive(false);
                    PlayField.SetActive(true);
                    if (Music)
                    {
                        Music.Play();
                    }
                    Score.ResetValue();
                    StartGame();
                }
            }
        }

        protected abstract void StartGame();

        protected abstract void PlayUpdate();

        protected abstract void OnGameOver();
    }
}
