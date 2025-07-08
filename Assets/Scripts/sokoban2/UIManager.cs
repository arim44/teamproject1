using ArcadeMiniGames;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UI;

namespace RetroSokoban
{
    public class UIManager : MonoBehaviour
    {
        [Header("Util")]
        [SerializeField] private TMP_Text txtNotice;    //�˸� �ؽ�Ʈ(����)
        [SerializeField] private GameObject objStart;   //��ŸƮUI ������Ʈ(���Ӹ�尡 Start�϶� �Ҵ�)
        [SerializeField] private GameObject objMain;    //����UI ������Ʈ(���Ӹ�尡 Main�϶� �Ҵ�)
        [SerializeField] private GameObject objSokoban; //���ڹ�UI ������Ʈ(���Ӹ�尡 ����� �϶� �Ҵ�)

        [Header("Start")]
        // ��Ʈ�� 1
        [SerializeField] private GameObject objIntro;   //��Ʈ���г� => �ʿ������ �����

        [Header("Main")]
        // �޴�, ����, ���۹�ư
        [SerializeField] private Button btnStartRetrokoban;      //��Ʈ���ڹ� ���۹�ư
        [SerializeField] private GameObject pnlStartMenu;        //��Ʈ���ڹ� �޴� ȭ��
        [SerializeField] private GameObject[] pnlNotice;         //���� ȭ���
        [SerializeField] private GameObject pnlRetrokobanClaer;  //��Ʈ���ڹ�Ŭ���� ȭ��

        [Header("Sokoban")]
        // ���ڹݳ� ī��Ʈ �ٿ�, ��Ʈ�̹���, ���۹�ư
        [SerializeField] private TitleCanvas titleCanvas;   //Ÿ��Ʋ ĵ����
        [SerializeField] private TMP_Text txtCountDown;     //���ڹݳ� ī��Ʈ�ٿ� �ؽ�Ʈ
        [SerializeField] private GameObject[] hpHearts;     // ��Ʈ

        //Ÿ�ӿ��� ȭ��, ���ӿ��� ȭ��, Ŭ���� ȭ��, ����ȭ��(����)
        [SerializeField] private GameObject pnlTimeOver;    //Ÿ�ӿ��� ȭ��
        [SerializeField] private GameObject pnlGameOver;    //���ӿ��� ȭ��
        [SerializeField] private GameObject pnlGameClear;   //����Ŭ���� ȭ��
        [SerializeField] private GameObject pnlGameExit;    //�������� ȭ��

        //���۹�ư, �޴���ư, ���÷��̹�ư, �ؽ�Ʈ ��ư
        [SerializeField] private Button btnStartSokoban;    //���ڹ� ���۹�ư
        [SerializeField] private Button btnMenuSokoban;     //���ڹ� �޴���ư
        [SerializeField] private Button btnReplaySokoban;   //���ڹ� ���÷��� ��ư
        [SerializeField] private Button btnNextstageSokoban;     //���ڹ� ������������ ��ư


        private GameManager _gameManager;       // ���ӸŴ���
        private SokobanManager _skobanManager;  // ���ڹ� �Ŵ���

        private int pnlNoticeCount = 0;


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
                    Debug.Log("SetUIMode :Main");
                    SetUIActive(GameMode.Main, true);
                    break;

                case GameMode.Sokoban:
                    Debug.Log("SetUIMode :Sokoban");
                    SetUIActive(GameMode.Sokoban, true);
                    break;
            }
        }

        // �迭�� ���� ��Ʈ ��ȣ�޾Ƽ� ����
        public void HideHeart(int heartNumber)
        {
            // ��Ȱ���� �ѰŶ� �ı��Ѱ� �ƴϾ ���� �ɸ� ����.... �ٽ� ����
            if (hpHearts != null)
            {
                //hpHearts[heartNumber].SetActive(false);
                Destroy(hpHearts[heartNumber]);
            }
            else Debug.LogWarning("��Ʈ�� �����ϴ�");
        }

        // Ÿ��Ʋ ĵ����
        public void SetActive(bool active)
        {
            titleCanvas?.SetActive(active);
            Invoke(nameof(InvokeShowNotice), 1.5f);
        }

        private void InvokeShowNotice()
        {
            pnlNotice[0].SetActive(true);
        }

        //Ÿ�ӿ�����ui �ѱ�
        public void SetTimeOverActive()
        {
            pnlTimeOver.SetActive(true);
        }


        ////////////// ��ư ���� ��� //////////////
        
        // ���γ� ��ư
        // ��Ʈ���ڹ� ���� ��ư
        public void OnStartRetrokobanClicked()
        {
            pnlStartMenu.SetActive(false);
            OpenNotice(0);
        }

        private void OpenNotice(int number)
        {
            pnlNotice[number].SetActive(true);
        }

        // next ��ư Ŭ��
        public void OnNextButtonClicked()
        {
            // ���� ���� ����
            pnlNotice[pnlNoticeCount].SetActive(false);
            pnlNoticeCount += 1;
            //���� ���� �ѱ�
            OpenNotice(pnlNoticeCount);
        }

        public void OnCloseNoticeClicked()
        {
            pnlNotice[pnlNoticeCount].SetActive(false);
            pnlNoticeCount += 1;

            //// pnlNotice �ȿ� �ִ� ���� �ٲ� => ������ �ȵ�
            //for (int i = 0; i < pnlNoticeCount; i++)
            //{
            //    pnlNotice[i].SetActive(false);
            //}            
        }
       
        // ���ڹ� ���ӳ� ��ư
        // �ΰ��� UI���� �ڷΰ��� ��ư�� Ŭ��
        public void OnMenuClicked()
        {
            // Ÿ��Ʋĵ���� Ȱ��
            titleCanvas?.SetActive(true);
            // �������� �ʱ�ȭ
            _skobanManager.GameReset();
        }

        // �ΰ��� Ÿ��Ʋ�� ���۹�ư Ŭ��
        public void OnPlayClicked()
        {
            // �������� �ʱ�ȭ
            _skobanManager.GameReset();
            // Ÿ��Ʋĵ���� ��Ȱ��
            titleCanvas?.SetActive(false);
        }

        // ���÷��� ��ư Ŭ��
        public void OnRePlayClicked()
        {    
            if(hpHearts != null)
            {
                // ��Ʈ����, �������� ����, Ÿ�̸Ӹ���
                _skobanManager.StageReset();
                pnlTimeOver.SetActive(false);
            }
            else
            {
                // ��Ʈ�迭�� ���ӿ���ȭ��
                pnlGameOver.SetActive(true);
            }
        }


    }
}
