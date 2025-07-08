using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroSokoban
{
    public class UIManager : MonoBehaviour
    {
        [Header("Util")]
        [SerializeField] TMP_Text txtNotice;    //�˸� �ؽ�Ʈ(����)
        [SerializeField] GameObject objStart;   //��ŸƮUI ������Ʈ(���Ӹ�尡 Start�϶� �Ҵ�)
        [SerializeField] GameObject objMain;    //����UI ������Ʈ(���Ӹ�尡 Main�϶� �Ҵ�)
        [SerializeField] GameObject objSokoban; //���ڹ�UI ������Ʈ(���Ӹ�尡 ����� �϶� �Ҵ�)

        [Header("Start")]
        // ��Ʈ�� 1
        [SerializeField] GameObject objIntro;   //��Ʈ���г� => �ʿ������ �����

        [Header("Main")]
        // �޴�, ����, ���۹�ư
        [SerializeField] Button btnStartRetrokoban;      //��Ʈ���ڹ� ���۹�ư

        [Header("Sokoban")]
        // ���ڹݳ� ī��Ʈ �ٿ�, ��Ʈ�̹���, ���۹�ư
        [SerializeField] private TitleCanvas titleCanvas;
        [SerializeField] TMP_Text txtCountDown;     //���ڹݳ� ī��Ʈ�ٿ� �ؽ�Ʈ
        [SerializeField] Button btnStartSokoban;    //���ڹ� ���۹�ư
        [SerializeField] GameObject[] hpHearts;      // ��Ʈ

        private GameManager _gameManager;       // ���ӸŴ���
        private SokobanManager _skobanManager;  // ���ڹ� �Ŵ���


        private void Awake()
        {
            titleCanvas = FindAnyObjectByType<TitleCanvas>(FindObjectsInactive.Include);
        }

        // ��ũ��Ʈ ��������
        public void SetManager(GameManager gameManager, SokobanManager skobanManager)
        {
            _gameManager = gameManager;
            _skobanManager = skobanManager;
        }

        /// <summary>
        /// UI�� ���� ���
        /// </summary>
        /// <param name="message"></param>
        public void SetNotice(string message)
        {
            txtNotice.text = message;
        }
        /// <summary>
        /// ���ڹ� �� ī��Ʈ�ٿ� �ؽ�Ʈ ����
        /// </summary>
        public void SetCountDown(int minutes, int seconds)
        {
            txtCountDown.text = $"{minutes} : {seconds}";
            //txtCountDown.text = string.Format("{{0:D2}:{1:D2}", minutes, seconds);
        }
        /// <summary>
        /// ���Ӹ�忡 ���� Ȱ��ȭ
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="value"></param>
        public void SetUIActive(GameMode mode, bool value)
        {
            //���� ��忡 ���� Ȱ��ȭ
            switch (mode)
            {
                case GameMode.Start:
                    objStart.SetActive(value);
                    break;

                case GameMode.Main:
                    objMain.SetActive(value);
                    break;

                case GameMode.Sokoban:
                    objSokoban.SetActive(value);
                    break;
            }
        }

        /// <summary>
        /// ���Ӹ�忡 ���� UI��带 �����Ѵ�
        /// </summary>
        /// <param name="mode"></param>
        public void SetUIMode(GameMode mode)
        {
            //ó���� �� ���� ��忡 �������� �ѱ�
            SetUIActive(GameMode.Start, false);
            SetUIActive(GameMode.Main, false);
            SetUIActive(GameMode.Sokoban, false);


            //���� ��忡 ���� ��� �ٲ�
            switch (mode)
            {
                case GameMode.Start:
                    Debug.Log("SetUIMode :Start");
                    SetUIActive(GameMode.Start, true);
                    break;

                case GameMode.Main:
                    Debug.Log("SetUIMode :Lobby");
                    SetUIActive(GameMode.Main, true);
                    break;

                case GameMode.Sokoban:
                    SetUIActive(GameMode.Sokoban, true);
                    break;
            }
        }

        // �迭�� ���� ��Ʈ ��ȣ�޾Ƽ� ����
        public void HideHeart(int heartNumber)
        {
            hpHearts[heartNumber].SetActive(false);
        }

        // ��ư ���� ���

        // �ΰ��� UI���� �ڷΰ��� ��ư�� Ŭ��
        public void OnBackClicked()
        {
            titleCanvas?.SetActive(true);
            //gameObject.SetActive(false);            
        }

        // �ΰ��� Ÿ��Ʋ�� ���۹�ư Ŭ��
        public void OnPlayClicked()
        {
            titleCanvas?.SetActive(false);
        }


    }
}
