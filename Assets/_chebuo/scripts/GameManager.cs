using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float time = 0f;
    public float cullentTime;
    public bool active;
    [SerializeField] Text t_text;
    [SerializeField] Text e_counter;
    void Start()
    {
        active = true;
    }
    void Update()
    {
        if(active){
            time+=Time.deltaTime;
            cullentTime=(int)time;
            t_text.text=cullentTime.ToString();
        }
        int count = GameObject.FindGameObjectsWithTag("enemy").Length;
        e_counter.text="敵の数: " + count.ToString();
        if(count==0){
            active = false;
            Debug.Log("clear");
        }
    }
}
