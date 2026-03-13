using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectMagic : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;
    GameObject selectedButton;
    public int magicNumber;
    string magicButtonName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetSelectMagic(){
        selectedButton=eventSystem.currentSelectedGameObject;
        string[] parts=selectedButton.name.Split('_');
        magicNumber=int.Parse(parts[1]);
        Debug.Log(magicNumber);
    }
}
