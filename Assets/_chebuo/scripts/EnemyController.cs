using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject target;
    GameManager gameManager;
    public int HP;
    public float speed;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb=this.GetComponent<Rigidbody>();
        rb.useGravity=false;
    }

    // Update is called once per frame
    void Update()
    {
        if(HP<=0){
            rb.useGravity=true;
            Destroy(gameObject,3f);
        }
    }
}