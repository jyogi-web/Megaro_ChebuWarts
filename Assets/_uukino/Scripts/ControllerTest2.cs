using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerTest2 : MonoBehaviour
{
    [SerializeField]InputActionProperty XRIRight;
    [SerializeField]InputActionProperty XRIRightTrigger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        XRIRight.action.Enable();
        XRIRightTrigger.action.Enable();
        Debug.Log(XRIRight);
    }

    // Update is called once per frame
    void Update()
    {
        var pos =XRIRight.action.ReadValue<Vector3>();
        var isPressed=XRIRightTrigger.action.ReadValue<float>();
    }
}