using UnityEngine;

public class MagicController : MonoBehaviour
{
    InputController inputController;
    
    void OnParticleCollision(GameObject obj)
    {
        if (obj.CompareTag("enemy"))
        {
            
        }
    }
}
