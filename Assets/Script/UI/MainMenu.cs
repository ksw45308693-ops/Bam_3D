using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필수!

public class MainMenu : MonoBehaviour
{
    // 버튼에 연결할 함수
    public void StartGame()
    {
        // "SampleScene"은 여러분이 작업하던 게임 씬 이름이어야 합니다!
        // 만약 이름이 다르면 똑같이 적어주세요.
        SceneManager.LoadScene("SampleScene");
    }
}