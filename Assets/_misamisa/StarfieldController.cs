using UnityEngine;

public class StarfieldController : MonoBehaviour
{
    [Header("Starfield Settings")]
    public GameObject starPrefab;
    public int starCount = 200;
    public float spawnRadius = 50f;
    public float rotationSpeed = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnStars();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        }
    }
}
