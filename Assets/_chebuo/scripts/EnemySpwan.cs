using System.Collections;
using UnityEngine;

public class EnemySpwan : MonoBehaviour
{
    [SerializeField] GameObject enemy; 
    float interval=3;
    
    void Start()
    {
        StartCoroutine(EnemyInstantiater());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator EnemyInstantiater()
    {
        while (true)
        {
            float angle=Random.Range(0,360);
            float rad=angle*Mathf.Deg2Rad;
            float rx=Mathf.Cos(rad);
            float ry=Mathf.Sin(rad);
            GameObject obj=Instantiate(enemy,new Vector3(rx*8,4,ry*8),Quaternion.identity);
            yield return  new WaitForSeconds(interval);
        }
    }
}
