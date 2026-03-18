using UnityEngine;
using UnityEngine.InputSystem;

public class InputController2 : MonoBehaviour{
    [SerializeField]InputActionProperty rposAction;
    [SerializeField]InputActionProperty lposAction;
    [SerializeField]InputActionProperty rtriggerAction;
    [SerializeField]InputActionProperty ltriggerAction;

    public Vector3 rpos;
    public Vector3 lpos;
    public bool rtrigger;
    public bool ltrigger;
    void Start(){
        rposAction.action.Enable();
        lposAction.action.Enable();
        rtriggerAction.action.Enable();
        ltriggerAction.action.Enable();
    }
    void Update(){
        rpos=rposAction.action.ReadValue<Vector3>();
        lpos=lposAction.action.ReadValue<Vector3>();
        if(rtriggerAction.action.ReadValue<float>()>0.5f)rtrigger=true;
        else rtrigger=false;
        if(ltriggerAction.action.ReadValue<float>()>0.5f)ltrigger=true;
        else ltrigger=false;
    }
}