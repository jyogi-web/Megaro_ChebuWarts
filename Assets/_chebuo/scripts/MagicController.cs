using UnityEngine;

public class MagicController : MonoBehaviour
{
    InputController inputController;
    EnemyController enemyController;
    
    void OnParticleCollision(GameObject obj)
    {
        if (obj.CompareTag("enemy"))
        {
            EnemyController hitEnemy = obj.GetComponent<EnemyController>();
            if (hitEnemy != null)
            {
                hitEnemy.HP -= 1;
                Debug.Log("hit");
            }
        }
    }
}
