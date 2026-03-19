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

    [SerializeField]GameObject finishScreen;

    void Start()
    {
        StartCoroutine(StartCount());
        finishScreen.SetActive(false);
    }

    void Update()
    {
        if (countdown == 0)
        {
            countdownText.enabled=false;
            active=true;
            countdown = -1; // 前回の修正箇所
        }
        
        if(active){
            time+=Time.deltaTime;
            string currentText=time.ToString("F2");
            t_text.text=currentText;
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

    public void SceneMove()
    {
        SceneManager.LoadScene("title");
    }

    IEnumerator StartCount()
    {
        while (countdown>0)
        {
            // Debug.Log(countdown);
            countdownText.text=countdown.ToString();
            yield return new WaitForSeconds(1);
            countdown--;
        }
    }
}