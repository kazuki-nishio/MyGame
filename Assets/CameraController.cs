using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    private GameObject Sardine;//Sardineオブジェクトを入れる    
    private GameObject Goal;//Goalオブジェクトを入れる    
    private float difference;//Sardineとカメラの位置の差

    // Start is called before the first frame update
    void Start()
    {        
        this.Sardine = GameObject.Find("Sardine");//Sardineオブジェクトを取得        
        this.Goal = GameObject.Find("Goal");//Goalオブジェクトを取得      
        this.difference = Sardine.transform.position.z - this.transform.position.z; //カメラとSardineの距離の差を求める
    }

    // Update is called once per frame
    void Update()
    {
        if(Sardine.transform.position.z < Goal.transform.position.z)
        {
            //カメラの位置をSardineの後ろに固定
            this.transform.position = new Vector3(Sardine.transform.position.x, this.transform.position.y, Sardine.transform.position.z - difference);
        }
       else
        {
            this.transform.position = new Vector3(0, this.transform.position.y, Goal.transform.position.z - difference);
        }
    }
}
