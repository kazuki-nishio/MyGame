using System.Collections;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    //Sardineオブジェクトを入れる
    private GameObject Sardine;
    //Fieldオブジェクトを入れる
    private GameObject Field;
    //SardineとFieldGeneratorの位置の差
    private float difference;

    // Start is called before the first frame update
    void Start()
    {
        //Sardineオブジェクトを取得
        this.Sardine = GameObject.Find("Sardine");
        //FieldGeneratorとSardineの距離の差を求める
        this.difference = this.transform.position.z - Sardine.transform.position.z;
        //Fieldオブジェクトを取得
        this.Field = GameObject.Find("Field");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(0, this.transform.position.y, Sardine.transform.position.z + difference);
    }

    private void OnTriggerEnter(Collider other)
    {
        //チェックポイントと衝突した際に海を生成する
        if (other.gameObject.tag == "FieldCheckPointTag")
        {
            Instantiate(Field);
            Field.transform.position = new Vector3(0, this.transform.position.y, this.transform.position.z);
        }
    }
}
