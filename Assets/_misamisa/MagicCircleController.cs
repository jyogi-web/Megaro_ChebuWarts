/*
 * MagicCircleController.cs
 * 
 * 魔法陣のコントローラー
 */

using UnityEngine;
using System.Collections;

public class MagicCircleController : MonoBehaviour
{
    [Header("Magic Circle")]
    public GameObject magicCircleObject;
    public GameObject centerStar;
    public float expandDuration = 1.5f;

    [Header("Controller Settings")]
    public float activationDistance = 1.0f;
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

    private Vector3 leftInitialPosition;
    private Vector3 rightInitialPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sceneManager = FindObjectOfType<TitleSceneManager>();

        // ゲーム開始時は非表示
        magicCircleObject.SetActive(false);
        centerStar.SetActive(false);
        titleTextObject.SetActive(false);

        // 初期位置を記録
        if (leftControllerTransform != null)
            leftInitialPosition = leftControllerTransform.position;
        if (rightControllerTransform != null)
            rightInitialPosition = rightControllerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCircleActive && !isExpanding)
            CheckControllerSpread();

        if (isCircleActive)
            CheckGrab(); // コントローラーが近づいたかチェック
    }

    void CheckControllerSpread()
    {
        float leftDist = 0f;
        float rightDist = 0f;

        if (leftControllerTransform != null)
            leftDist = Vector3.Distance(
                leftControllerTransform.position,
                leftInitialPosition
            );

        if (rightControllerTransform != null)
            rightDist = Vector3.Distance(
                rightControllerTransform.position,
                rightInitialPosition
            );

        float maxDist = Mathf.Max(leftDist, rightDist);
        Debug.Log("最大距離：" + maxDist);

        if (maxDist >= activationDistance)
        {
            StartCoroutine(SummonCircle());
            Debug.Log("魔法陣召喚開始");
        }
        else
        {
            isCircleActive = false;
            Debug.Log("まだ距離が足りない");
        }
    }

    IEnumerator SummonCircle()
    {
        isExpanding = true;

        if (summonSE && audioSource)
            audioSource.PlayOneShot(summonSE);

        // 魔法陣を表示してスケールをゼロから広げる
        magicCircleObject.SetActive(true);
        magicCircleObject.transform.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < expandDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / expandDuration;
            magicCircleObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, EaseOut(t));
            yield return null; // 1フレーム待つ
        }

        // アニメーション完了後にタイトルと星を表示
        titleTextObject.SetActive(true);
        centerStar.SetActive(true);
        isCircleActive = true;
        
    }

    void CheckGrab()
    {
        if (leftControllerTransform == null || rightControllerTransform == null) return;

        // コントローラーが星に近いか確認
        bool leftNear = Vector3.Distance(
            leftControllerTransform.position,
            centerStar.transform.position
        ) < grabRadius;

        bool rightNear = Vector3.Distance(
            rightControllerTransform.position,
            centerStar.transform.position
        ) < grabRadius;

        // グリップボタンを押した瞬間に判定
        bool leftGrip = OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger);
        bool rightGrip = OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);

        if ((leftNear && leftGrip) || (rightNear && rightGrip))
        {
            if (grabSE && audioSource)
                audioSource.PlayOneShot(grabSE);

            Debug.Log("ゲームスタート");
            sceneManager.OnGameStart();
        }
    }

    // イーズアウト：最初速く、最後ゆっくり止まる
    float EaseOut(float t) => 1f - Mathf.Pow(1f - t, 3f);

}
