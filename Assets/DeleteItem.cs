using System.Collections;
using UnityEngine;

public class DeleteItem : MonoBehaviour
{
    //メインカメラを入れる
    private GameObject mainCam;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
       if(this.transform.position.z<mainCam.transform.position.z)
        {
            Destroy(gameObject);
        }
    }
}
