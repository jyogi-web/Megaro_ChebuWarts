/*
 * MagicCircleController.cs
 * 
 * 魔法陣のコントローラー
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MagicCircleController : MonoBehaviour
{
    [Header("Magic Circle")]
    public GameObject magicCircleObject;
    public GameObject centerStar;
    public float expandDuration = 0.5f;

    [Header("Controller Settings")]
    public float activationDistance = 1.0f;
    public float grabRadius = 0.5f;

    [Header("OVR Anchors")]
    public Transform leftControllerTransform;
    public Transform rightControllerTransform;

    [Header("Input Actions")]
    [SerializeField] InputActionProperty leftGripAction;
    [SerializeField] InputActionProperty rightGripAction;

    [Header("SE")]
    public AudioClip summonSE;
    public AudioClip grabSE;
    public AudioClip fumbleSE;
    private AudioSource audioSource;

    [Header("Title")]
    public GameObject titleTextObject;

    [Header("Guidance")]
public ParticleSystem guidanceParticle;
public Transform playerTransform; // XR Origin(XR Rig)をドラッグ
private List<ParticleSystem> trailParticles = new List<ParticleSystem>();

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
        leftGripAction.action.Enable();
        rightGripAction.action.Enable();

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
        {
            CheckGrab(); 
            UpdateGuidanceParticles();
        }
            
    }

    void UpdateGuidanceParticles()
    {
        if (trailParticles.Count == 0) return;
        if (playerTransform == null) return;

        int count = trailParticles.Count;
        Vector3 from = playerTransform.position + Vector3.up;
        Vector3 to = centerStar.transform.position;

        if (float.IsNaN(from.x) || float.IsNaN(to.x)) return;

        for (int i = 0; i < count; i++)
        {
            if (trailParticles[i] == null) continue;

            float t = count > 1 ? (float)i / (count - 1) : 0f;
            Vector3 target = Vector3.Lerp(from, to, t);

            // なめらかに追従
            trailParticles[i].transform.position = Vector3.Lerp(
                trailParticles[i].transform.position,
                target,
                Time.deltaTime * 3f
            );
            
        }
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

        StartCoroutine(SpawnGuidanceParticles());

    }

    IEnumerator SpawnGuidanceParticles()
    {
        if (guidanceParticle == null || playerTransform == null) yield break;

        int particleCount = 8; // 道案内の粒子の数

        for (int i = 0; i < particleCount; i++)
        {
            // プレイヤーからCenterStarまでを等間隔に配置
            float t = (float)i / (particleCount - 1);
            Vector3 pos = Vector3.Lerp(
                playerTransform.position + Vector3.up,
                centerStar.transform.position,
                t
        )   ;

            // 粒子を生成
            ParticleSystem p = Instantiate(guidanceParticle, pos, Quaternion.identity);
            p.gameObject.SetActive(true);
            trailParticles.Add(p);

            yield return new WaitForSeconds(0.1f); // 少しずつ出現
        }
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

        // グリップ値で判定（0〜1のアナログ値）
        bool leftGrip = leftGripAction.action.ReadValue<float>() > 0.5f;
        bool rightGrip = rightGripAction.action.ReadValue<float>() > 0.5f;
#if UNITY_EDITOR
        if (Keyboard.current != null && Keyboard.current.gKey.isPressed) leftGrip = rightGrip = true;
#endif

        Debug.Log($"左近い: {leftNear}, 右近い: {rightNear}, 左グリップ: {leftGrip}, 右グリップ: {rightGrip}");

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
