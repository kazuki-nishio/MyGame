using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Sardineオブジェクトを入れる
    private GameObject Sardine;
    //Goalオブジェクトを入れる
    private GameObject Goal;
    //Sardineとカメラの位置の差
    private float difference;

    // Start is called before the first frame update
    void Start()
    {
        //Sardineオブジェクトを取得
        this.Sardine = GameObject.Find("Sardine");
        //Goalオブジェクトを取得
        this.Goal = GameObject.Find("Goal");
        //カメラとSardineの距離の差を求める
        this.difference = Sardine.transform.position.z - this.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if(Sardine.transform.position.z < Goal.transform.position.z)
        {
            //カメラの位置をSardineの後ろに固定
            this.transform.position = new Vector3(0, this.transform.position.y, Sardine.transform.position.z - difference);
        }
       else
        {
            this.transform.position = new Vector3(0, this.transform.position.y, Goal.transform.position.z - difference);
        }
    }
}
