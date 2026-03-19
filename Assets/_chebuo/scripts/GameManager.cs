using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private float time = 0f;
    public float currentTime;
    public bool active;
    [SerializeField] Text t_text;
    [SerializeField] Text e_counter;

    float countdown=3;
    [SerializeField] Text countdownText;
    [SerializeField] Text killText;
    [SerializeField] Text timeText;

    [SerializeField] GameObject finishScreen;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] float fallThreshold = -20f;
    [SerializeField] float gameOverPanelDistance = 2f; // パネルとカメラの距離(m)

    private Transform playerTransform;
    private Transform cameraTransform;
    private Canvas gameOverCanvas;
    private bool gameOver = false;

    void Start()
    {
        StartCoroutine(StartCount());
        finishScreen.SetActive(false);
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
            // gameOverScreenの親CanvasをWorld Spaceに設定
            gameOverCanvas = gameOverScreen.GetComponentInParent<Canvas>();
        }

        var xrOrigin = GameObject.Find("XR Origin (XR Rig)");
        if (xrOrigin != null) playerTransform = xrOrigin.transform;

        // VRカメラ（HMD）のTransformを取得
        var mainCam = Camera.main;
        if (mainCam != null) cameraTransform = mainCam.transform;

        Debug.Log(playerTransform);
    }

    void Update()
    {
        if (gameOver)
        {
            UpdateGameOverPanelPosition();
            return;
        }

        if (countdown == 0)
        {
            if (countdownText != null) countdownText.enabled=false;
            active=true;
            countdown = -1; // 前回の修正箇所
        }
        
        if(active){
            time+=Time.deltaTime;
            string currentText=time.ToString("F2");
            t_text.text=currentText;
        }

        if (active && playerTransform != null && playerTransform.position.y < fallThreshold)
        {
            GameOver();
            return;
        }

        int count = GameObject.FindGameObjectsWithTag("enemy").Length;
        // Debug.Log(count); 毎フレーム出ると重くなるので一旦コメントアウトをおすすめします
        
        // 敵が0体になったら FinishGame() を呼ぶように変更
        if(count==0 && active){
            FinishGame();
        }
    }

    // 【追加】ゲーム終了時の処理をまとめたメソッド（publicにして外部から呼べるようにする）
    public void FinishGame()
    {
        if (active)
        {
            active = false;
            Debug.Log("clear / finish");
            finishScreen.SetActive(true);
            string timeStr="Time  "+time.ToString("F2");
            timeText.text=timeStr;
        }
    }

    private void GameOver()
    {
        active = false;
        gameOver = true;
        Debug.Log("Game Over: プレイヤーが落下しました");
        if (gameOverScreen != null)
        {
            PlaceGameOverPanelInFrontOfPlayer();
            gameOverScreen.SetActive(true);
        }
    }

    private void PlaceGameOverPanelInFrontOfPlayer()
    {
        Transform camTransform = cameraTransform != null ? cameraTransform : playerTransform;
        if (camTransform == null || gameOverCanvas == null) return;

        // CanvasをWorld Spaceに切り替え（初回のみ）
        if (gameOverCanvas.renderMode != RenderMode.WorldSpace)
        {
            gameOverCanvas.renderMode = RenderMode.WorldSpace;
            gameOverCanvas.worldCamera = Camera.main;
            gameOverCanvas.transform.localScale = Vector3.one * 0.002f;
        }

        // パネルをカメラの正面に配置
        Transform canvasTransform = gameOverCanvas.transform;
        Vector3 forward = camTransform.forward;
        forward.y = 0;
        if (forward == Vector3.zero) forward = Vector3.forward;
        forward.Normalize();

        canvasTransform.position = camTransform.position + forward * gameOverPanelDistance;
        canvasTransform.rotation = Quaternion.LookRotation(forward);
    }

    private void UpdateGameOverPanelPosition()
    {
        if (gameOverCanvas == null || !gameOverScreen.activeSelf) return;
        PlaceGameOverPanelInFrontOfPlayer();
    }

    public void SceneMove()
    {
        SceneManager.LoadScene("title");
    }

    IEnumerator StartCount()
    {
        while (countdown>0)
        {
            Debug.Log(countdown);
            if (countdownText != null) countdownText.text=countdown.ToString();
            yield return new WaitForSeconds(1);
            countdown--;
        }
    }
}