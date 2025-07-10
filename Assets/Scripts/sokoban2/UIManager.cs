using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroSokoban
{
    public class UIManager : MonoBehaviour
    {
        [Header("Util")]
        private TMP_Text txtNotice;    //�˸� �ؽ�Ʈ(����)
        [SerializeField] private GameObject objStart;   //��ŸƮUI ������Ʈ(���Ӹ�尡 Start�϶� �Ҵ�)
        [SerializeField] private GameObject objMain;    //����UI ������Ʈ(���Ӹ�尡 Main�϶� �Ҵ�)
        //[SerializeField] private GameObject objSokoban; //���ڹ�UI ������Ʈ(���Ӹ�尡 ����� �϶� �Ҵ�)
               
        [Header("Main")]
        // �޴�, ����, ���۹�ư
        [SerializeField] private Button btnStartRetrokoban;      //��Ʈ���ڹ� ���۹�ư
        [SerializeField] private GameObject pnlStartMenu;        //��Ʈ���ڹ� �޴� ȭ��
        [SerializeField] private GameObject[] pnlNotice;         //���� ȭ���
        [SerializeField] private GameObject pnlRetrokobanClaer;  //��Ʈ���ڹ�Ŭ���� ȭ��

        [Header("Sokoban")]
        // ���ڹݳ� ī��Ʈ �ٿ�, ��Ʈ�̹���, ���۹�ư
        [SerializeField] private TitleCanvas titleCanvas;  //Ÿ��Ʋ ĵ����
        [SerializeField] private TMP_Text txtCountDown;     //���ڹݳ� ī��Ʈ�ٿ� �ؽ�Ʈ
        [SerializeField] private GameObject[] hpHearts;     // ��Ʈ

        //Ÿ��Ʋ ȭ��, Ÿ�ӿ��� ȭ��, ���ӿ��� ȭ��, Ŭ���� ȭ��, ����ȭ��(����)
        //[SerializeField] private GameObject titleUI;        //Ÿ��Ʋ UI
        [SerializeField] private GameObject pnlTimeOver;    //Ÿ�ӿ��� ȭ��
        [SerializeField] private GameObject pnlGameOver;    //���ӿ��� ȭ��
        [SerializeField] private GameObject pnlInfoNextStage;    //���ӿ��� ȭ��
        [SerializeField] private GameObject pnlGameClear;   //����Ŭ���� ȭ��
        [SerializeField] private GameObject pnlGameExit;    //�������� ȭ��

        //���۹�ư, �޴���ư, ���÷��̹�ư, �ؽ�Ʈ ��ư
        [SerializeField] private Button btnStartSokoban;    //���ڹ� ���۹�ư
        [SerializeField] private Button btnMenuSokoban;     //���ڹ� �޴���ư
        [SerializeField] private Button btnReplaySokoban;   //���ڹ� ���÷��� ��ư
        [SerializeField] private Button btnNextstageSokoban;     //���ڹ� ������������ ��ư

        private GameManager _gameManager;       // ���ӸŴ���
        private SokobanManager _skobanManager;  // ���ڹ� �Ŵ���

        [SerializeField] private int pnlNoticeCount = 0;    //����â ī��Ʈ


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
            //ó���� �� ���� ������ ��常 �ѱ�
            SetUIActive(GameMode.Start, false);
            SetUIActive(GameMode.Main, false);
            SetUIActive(GameMode.Sokoban, false);


            //���� ��忡 ���� UI ����
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
            hpHearts[heartNumber].SetActive(false);
        }

        // ��Ʈ��Ȱ��(�ʱ�ȭ)
        public void ShowAllHeart()
        {
            for(int i = 0; i < hpHearts.Length; i++)
            {
                hpHearts[i].SetActive(true);
            }
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
        public void SetTimeOverUI(bool active)
        {
            pnlTimeOver.SetActive(active);
        }

        public void SetInfoNextStageUI(bool active)
        {
            pnlInfoNextStage.SetActive(active);
        }

        // ���� Ŭ����� UI�ѽ�
        public void SetClearSokobanUI(bool active)
        {
            pnlGameClear.SetActive(active);
        }

        //���ӿ����� ui 
        public void SetGameOverUI(bool active)
        {
            pnlGameOver.SetActive(active);
        }

        //���ڹ� ���� �� UIȭ�� �� ����
        private void CloseAllSokobanUI()
        {
            pnlTimeOver.SetActive(false);
            pnlGameOver.SetActive(false);
            pnlGameClear.SetActive(false);
            pnlInfoNextStage.SetActive(false);
            pnlGameExit.SetActive(false);
        }


        ////////////// ��ư ���� ��� //////////////

        // ���γ� ��ư
        // ��Ʈ���ڹ� ���� ��ư
        public void OnStartRetrokobanClicked()
        {
            pnlStartMenu.SetActive(false);
            OpenNotice(0);
        }

        public void OpenNotice(int number)
        {
            pnlNotice[number].SetActive(true);
        }

        // next ��ư Ŭ��
        public void OnNextButtonClicked()
        {
            // ���� ���� ����
            pnlNotice[pnlNoticeCount].SetActive(false);
            // ����â ī��Ʈ +1
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

        public void OnExitButtonClicked()
        {
            _gameManager.SetgameMode(GameMode.Exit);
        }

        // ���ڹ� ���ӳ� ��ư
        // �ΰ��� UI���� �ڷΰ��� ��ư�� Ŭ��
        public void OnMenuClicked()
        {
            // Ÿ��Ʋĵ���� Ȱ��
            titleCanvas?.SetActive(true);
            
            // �������� �ʱ�ȭ
            //_skobanManager.GameReset();

            //���� �޴���ư ������ �ʱ�ȭ ���� ������
            // ī��Ʈ�ٿ� ����
            _skobanManager.StopCountdown();

            //�����ִ� ui ����
            CloseAllSokobanUI();
        }

        // �ΰ��� Ÿ��Ʋ�� ���۹�ư Ŭ��
        public void OnSokobanPlayClicked()
        {
            // �������� �ʱ�ȭ
            _skobanManager.GameReset();
            // Ÿ��Ʋĵ���� ��Ȱ��
            titleCanvas?.SetActive(false);
        }

        // ���÷��� ��ư Ŭ��
        public void OnRePlayClicked()
        {
            int activeHeartCount = CheckActiveHeartCount();

            // Ȱ���� ��Ʈ�� ������ ���̴� ��� �����ϱ� Ȱ������ �ٲ�� ��
            if (activeHeartCount > 0)
            {
                // ��Ʈ����, �������� ����, Ÿ�̸Ӹ���
                _skobanManager.StageReset();
                pnlTimeOver.SetActive(false);
            }
            else
            {
                // ��Ʈ�迭�� Ȱ���� ��Ʈ ������ ���ӿ���ȭ��
                pnlGameOver.SetActive(true);
            }
        }

        //��Ʈ�迭 ��Ʈ�ｺ���� �ؾ߰���
        private int CheckActiveHeartCount()
        {
            // Ȱ�� ������ ��Ʈ �� Ȯ��
            int activeHearts = 0;
            foreach(var heart in hpHearts)
            {
                if(heart.activeSelf)
                    activeHearts++;
            }
            return activeHearts;
        }

        //���ڹ� ������������ �ؽ�Ʈ ��ư Ŭ��
        public void OnNextStageClicked()
        {
            _skobanManager.ClickNextStageButton();
        }

        // ���ڹ� exit ��ư Ŭ��
        public void OnExitSokoban()
        {
            // Exit�г�(�ȭ��) Ȱ��
            pnlGameExit.SetActive(true);
            //���� ����
            pnlNotice[5].SetActive(true);
        }

        public void OffExitSokoban()
        {
            // �ȭ�� ��Ȱ��
            pnlGameExit.SetActive(false);
        }
    }
}
