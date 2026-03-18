using UnityEngine;
using MegaroChebuWarts.Magic;

public class MagicShoot : MonoBehaviour
{
    [SerializeField]private SpellEffectManager manager;
    InputController inputController;
    void Start()
    {
        inputController=this.GetComponent<InputController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(inputController.rtrigger)Fire(MagicElement.Earth);
    }
    private void Fire(MagicElement element)
    {
        manager.Fire(new SpellRequest
        {
            Element = element,
            Position = new Vector3(inputController.rpos.x,inputController.rpos.y+1.35f,inputController.rpos.z),
            Rotation = inputController.rrot,
        });
        Debug.Log("Fire!!");
    }
}
