using UnityEngine;

namespace RetroSokoban
{
    public class CountdownTimer : MonoBehaviour
    {
        // �� ������������ ������ �ð� �޾Ƽ� ī��Ʈ �ٿ�
        // ī��Ʈ �ٿ� ������ ����ȭ���� �ٽý��� ��ư�� ������ ��Ʈ ����
        // ui�Ŵ����� Ÿ�̸��ؽ�Ʈ�� ���

        [SerializeField] private UIManager _uiManager;
        [SerializeField] private SokobanManager _sokobanManager;

        [SerializeField] private float startTime = 180f;    //�ʱ� ī��Ʈ�ٿ�(��) 3��

        private float currentTime;          // ����ð�
        private bool isCounting = false;    //ī���ÿ���


        private void Update()
        {
            if (isCounting)
            {
                currentTime -= Time.deltaTime;  // ���� �ð����� �帣�� �ð� ��

                // ����ð��� 0���� ������
                if (currentTime < 0)
                {
                    // ���� �ð� 0
                    currentTime = 0;
                    isCounting = false;
                    print("ī��Ʈ�ٿ� ����");

                    // Ÿ�ӿ�������
                    _sokobanManager.TimeOver();
                }
                // �ð���� ���
                UPdateTimerText();
            }
        }

        // ���ӸŴ������� ����
        public void SetManager(UIManager uiManager, SokobanManager sokobanManager)
        {
            _uiManager = uiManager;
            _sokobanManager = sokobanManager;
        }

        public void CountdownInitialized()
        {
            currentTime = startTime;
            isCounting = true;
        }

        private void UPdateTimerText()
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);   //����ð��� 60�� ���� ��
            int seconds = Mathf.FloorToInt(currentTime % 60);   //����ð��� 60���� ���� ������

            _uiManager.SetCountDown(minutes, seconds);
        }
    }
}
