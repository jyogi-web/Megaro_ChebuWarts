using UnityEngine;

public class FallDetector : MonoBehaviour
{
    // インスペクターからGameManagerを割り当てます
    [SerializeField] private GameManager gameManager;

    // トリガー（すり抜ける判定）に何かが触れた時に呼ばれる処理
    private void OnTriggerEnter(Collider other)
    {
        // 触れたオブジェクトのタグが "Player" だった場合
        if (other.CompareTag("Player"))
        {
            Debug.Log("プレイヤーが落下しました！");

            // GameManagerにゲーム終了を知らせる
            if (gameManager != null)
            {
                gameManager.FinishGame();
            }
        }
    }
}