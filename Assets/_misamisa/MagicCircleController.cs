/*
 * MagicCircleController.cs
 * 
 * 魔法陣のコントローラー
 */

using UnityEngine;

public class MagicCircleController : MonoBehaviour
{
    [Header("Magic Circle")]
    public GameObject magicCircleObject;
    public GameObject centerStar;
    public float expandDuration = 1.5f;

    [Header("Controller Settings")]
    public float activationDistance = 0.6f;
    public float grabRadius = 0.15f;

    [Header("OVR Anchors")]
    public Transform leftControllerTransform;
    public Transform rightControllerTransform;

    [Header("SE")]
    public AudioClip summonSE;
    public AudioClip grabSE;
    public AudioClip fumbleSE;
    private AudioSource audioSource;

    [Header("Title")]
    public GameObject titleTextObject;

    private bool isCircleActive = false;
    private bool isExpanding = false;
    private TitleSceneManager sceneManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
audioSource = GetComponent<AudioSource>();
        sceneManager = FindObjectOfType<TitleSceneManager>();

        // ゲーム開始時は非表示
        magicCircleObject.SetActive(false);
        centerStar.SetActive(false);
        titleTextObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
