using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectMagic2 : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;
    GameObject selectedButton;
    public int magicNumber;
    string magicButtonName;
    
    public void GetSelectMagic(){
        selectedButton=eventSystem.currentSelectedGameObject;
        string[] parts=selectedButton.name.Split('_');
        magicNumber=int.Parse(parts[1]);
        Debug.Log(magicNumber);
    }
}