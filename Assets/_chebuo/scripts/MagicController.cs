using UnityEngine;

public class MagicController : MonoBehaviour
{
    [SerializeField] GameObject magicManager;
    InputController inputController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(magicManager!=null)inputController=magicManager.GetComponent<InputController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(magicManager!=null)this.transform.position=new Vector3(inputController.rpos.x,inputController.rpos.y+1.35f,inputController.rpos.z);
    }
}
