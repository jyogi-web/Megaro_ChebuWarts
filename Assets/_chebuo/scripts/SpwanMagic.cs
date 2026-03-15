using System.Collections.Generic;
using UnityEngine;

public class SpwanMagic : MonoBehaviour
{
    [SerializeField] GameObject[] magics;
    [SerializeField] GameObject worldControllerPos;
    [SerializeField] LineRenderer circle;
    List<Vector3> points=new List<Vector3>();
    Vector3 lastPos;
    bool hasLastPos=false;
    Vector3 center;
    public int magicNumber;
    InputController inputController;

    void Start()
    {
        inputController=this.GetComponent<InputController>();
    }

    void Update()
    {
        if(inputController.rtrigger){
            Vector3 pos=new Vector3(inputController.rpos.x,inputController.rpos.y+1.35f,inputController.rpos.z);
            if(!hasLastPos||Vector3.Distance(lastPos,pos)>0.02f){
                points.Add(pos);
                lastPos=pos;
                hasLastPos=true;
            }
        }
        circle.positionCount=points.Count;
        circle.SetPositions(points.ToArray());
        center=Vector3.zero;
        foreach(var p in points){
            center+=p;
        }
        if(points.Count>0)center/=points.Count;
        Debug.Log(center);
        if (inputController.rtrigger)Debug.Log(center);
        if(!inputController.rtrigger&&points.Count>0&&hasLastPos){
            GameObject magic=Instantiate(magics[magicNumber],center,Quaternion.identity);
            Destroy(magic,5f);
            hasLastPos=false;
            Invoke("DeleteMagicCircle",5f);
        }     
    }
    void DeleteMagicCircle(){
        points.Clear();
        circle.positionCount = 0;
    }
}
 