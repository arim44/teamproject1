using UnityEngine;


namespace RetroSokoban
{
    public class HeartHealth : MonoBehaviour
    {
        // ��Ʈ ���� ����
        [SerializeField] private int startHeartCount = 3;
        [SerializeField] private int heartCount;

        // heartCount �� 0�Ǵ��� ��� üũ�غ����� 0�� �Ǹ� ���Ӹ��end?


        private void Start()
        {
            SetHeartCount();
        }

        // ó�� ���ö� ��Ʈ���� �ʱ�ȭ ���Ѿ���
        public void SetHeartCount()
        {
            heartCount = startHeartCount;
        }

        public int CalculateHeartCount()
        {
            heartCount--;
            if (heartCount == 0) heartCount = 0;
            return heartCount;
        }
    }
}
