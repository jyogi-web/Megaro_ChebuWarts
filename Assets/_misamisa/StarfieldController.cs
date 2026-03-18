using UnityEngine;

public class StarfieldController : MonoBehaviour
{
    [Header("Starfield Settings")]
    public GameObject starPrefab;
    public int starCount = 200;
    public float spawnRadius = 50f;
    public float rotationSpeed = 0.5f;

    [Header("Touch Effect")]
    public GameObject starBurstPrefab;
    public AudioClip starTouchSE;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SpawnStars();
    }

    // Update is called once per frame
    void Update()
    {
        // 星空全体をゆっくり回転
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void SpawnStars()
    {
        for (int i = 0; i < starCount; i++)
        {
            // 球面上のランダムな位置に星を配置
            Vector3 pos = Random.onUnitSphere * spawnRadius;
            GameObject star = Instantiate(starPrefab, pos, Quaternion.identity, transform);

            // 星のサイズをランダムに
            float size = Random.Range(0.05f, 0.2f);
            star.transform.localScale = Vector3.one * size;

            // 各星にStarTouchableをアタッチ
            star.AddComponent<StarTouchable>().Init(this);
        }
    }

    // 星が触られたときに呼ばれる
    public void OnStarTouched(Vector3 position)
    {
        if (starBurstPrefab)
            Instantiate(starBurstPrefab, position, Quaternion.identity);

        if (starTouchSE && audioSource)
            audioSource.PlayOneShot(starTouchSE);
    }
}
// 各星にアタッチするクラス
public class StarTouchable : MonoBehaviour
{
    private StarfieldController controller;

    public void Init(StarfieldController ctrl)
    {
        controller = ctrl;
    }

    public void Touch()
    {
        controller.OnStarTouched(transform.position);
        Destroy(gameObject, 0.1f);
    }
}