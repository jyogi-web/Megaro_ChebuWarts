using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour{
    [SerializeField]InputActionProperty rposAction;
    [SerializeField]InputActionProperty lposAction;
    [SerializeField]InputActionProperty rrotAction;
    [SerializeField]InputActionProperty lrotAction;
    [SerializeField]InputActionProperty rtriggerAction;
    [SerializeField]InputActionProperty ltriggerAction;

    public Vector3 rpos;
    public Vector3 lpos;
    public Quaternion rrot;
    public Quaternion lrot;
    public bool rtrigger;
    public bool ltrigger;
    void Start(){
        rposAction.action.Enable();
        lposAction.action.Enable();
        rrotAction.action.Enable();
        lrotAction.action.Enable();
        rtriggerAction.action.Enable();
        ltriggerAction.action.Enable();
    }
    void Update(){
        rpos=rposAction.action.ReadValue<Vector3>();
        lpos=lposAction.action.ReadValue<Vector3>();
        rrot=rrotAction.action.ReadValue<Quaternion>();
        lrot=lrotAction.action.ReadValue<Quaternion>();
        if(rtriggerAction.action.ReadValue<float>()>0.5f)rtrigger=true;
        else rtrigger=false;
        if(ltriggerAction.action.ReadValue<float>()>0.5f)ltrigger=true;
        else ltrigger=false;
    }
}