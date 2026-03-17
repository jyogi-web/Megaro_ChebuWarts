using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    [Header("")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;
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
