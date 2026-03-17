using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    [Header("BGM")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;

    [Header("References")]
    public MagicCircleController magicCircle;
    public ScrollController scroll;
    public EasterEggController easterEgg;

    [Header("Scene Names")]
    public string gameSceneName = "GameScene";
    public string explainSceneName = "ExplainScene";

    public void OnGameStart()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnOpenScroll()
    {
        scroll.OpenScroll();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(bgmSource && bgmClip)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
