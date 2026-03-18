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

    private Transform playerTransform;
    private bool gameOver = false;

    void Start()
    {
        StartCoroutine(StartCount());
        finishScreen.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);

        var xrOrigin = GameObject.Find("XR Origin (XR Rig)");
        if (xrOrigin != null) playerTransform = xrOrigin.transform;
        Debug.Log(playerTransform);
    }
    void Update()
    {
        if (gameOver) return;

        if (countdown == 0)
        {
            if (countdownText != null) countdownText.enabled=false;
            active=true;
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
        Debug.Log(count);
        if(count==0&&active){
            active = false;
            Debug.Log("clear");
            finishScreen.SetActive(true);
            string timeStr="タイム:"+time.ToString("F2");
            timeText.text=timeStr;
        }
    }

    private void GameOver()
    {
        active = false;
        gameOver = true;
        Debug.Log("Game Over: プレイヤーが落下しました");
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);
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
