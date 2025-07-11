using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeMiniGames
{
    public class Number : MonoBehaviour
    {
        public GameObject[] scoreDigits;
        private Material[] scoreDigitMaterials;
        private int _value = 0;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                Initialize();
                Set(value);
            }
        }
        private bool init;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!init)
            {
                scoreDigitMaterials = new Material[scoreDigits.Length];
                for (int i = 0; i < scoreDigits.Length; i++)
                {
                    scoreDigitMaterials[i] = scoreDigits[i].GetComponent<MeshRenderer>().material;
                }
                init = true;
            }
        }

        public void ResetValue()
        {
            _value = 0;
            var offset = new Vector2(0, 0);
            for (int i = 0; i < scoreDigitMaterials.Length; i++)
            {
                scoreDigitMaterials[i].mainTextureOffset = offset;
            }
        }

        private void Set(int value)
        {
            if (_value != value)
            {
                _value = value;
                int remainingDigits = value;
                var offset = new Vector2(0, 0);
                for (int i = 0; i < scoreDigitMaterials.Length; i++)
                {
                    if (remainingDigits >= 1)
                    {
                        var digit = remainingDigits % 10;
                        offset.x = digit * 0.1f;
                        scoreDigitMaterials[i].mainTextureOffset = offset;
                        remainingDigits = remainingDigits / 10;
                    }
                    else
                    {
                        offset.x = 0;
                        scoreDigitMaterials[i].mainTextureOffset = offset;
                    }
                }
            }
        }

        public void SetColor()
        {
            SetColor(Color.white);
        }
        public void SetColor(Color color)
        {
            Initialize();
            for (int i = 0; i < scoreDigits.Length; i++)
            {
                scoreDigitMaterials[i].color = color;
            }
        }
    }
}
