using UnityEngine;


namespace RetroSokoban
{
    public class HeartHealth : MonoBehaviour
    {
        // 하트 갯수 차감
        [SerializeField] private int startHeartCount = 3;
        [SerializeField] private int heartCount;

        // heartCount 가 0되는지 계속 체크해봐야함 0이 되면 게임모드end?


        private void Start()
        {
            SetHeartCount();
        }

        // 처음 들어올때 하트갯수 초기화 시켜야함
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
