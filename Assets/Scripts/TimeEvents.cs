using System.Collections;
using UnityEngine;

// Thực hiên tất cả qua Coroutines (có sẵn trong unity => biểu diễn thời gian)
public class TimeEvents : MonoBehaviour
{
    
    public static TimeEvents timeEvents;

    void Awake()
    {
        timeEvents = this;
    }

    // Chờ AI di chuyển cờ
    public void StartWaitForAI()
    {
        StartCoroutine(WaitForAI());
    }

    // AI đánh sau 1 giây chờ => phương thức MovePieceAi của GameManager được gọi
    IEnumerator WaitForAI()
    {
        Application.targetFrameRate = 120;

        yield return new WaitForSeconds(1.0f);

        Chess.MoveAIPiece();
        Interface.interfaceClass.EnableButtonPause(true);

        Application.targetFrameRate = 60;
    }
}