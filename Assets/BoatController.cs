using System.Collections;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    //障害物のプリファブ
    public GameObject obstaclePrefab;
    //fishingBoatのRigidBody
    private Rigidbody boatRigid;
    //fishingBoatの前方向の速度
    private float fishingBoatVelocityZ;
    //fishingBoatの横方向の速度
    private float fishingBoatVelocityX;
    //fishingBoat生成時のボートの座標
    private float boatGeneratingPoint;
    //Sin関数へ入れる角度
    private double radian = 0d;
    //プレーヤーオブジェクト
    private GameObject sardine;
    //プレーヤーのRigidBody
    private Rigidbody sardineRigid;

    // Start is called before the first fraboatGeneratingPointme update
    void Start()
    {
        //プレーヤのオブジェクトを取得
        this.sardine = GameObject.Find("Sardine");
        //プレーヤーのRigidBodyを取得
        sardineRigid = sardine.GetComponent<Rigidbody>();
        if (gameObject.tag!="GoalBoatTag")
        {
            //InstantiateObstacleを1秒ごとに呼び出す
            InvokeRepeating("InstantiateObstacle", 0f, 1f);
            //fishingBoatのRigidBodyを取得
            this.boatRigid = GetComponent<Rigidbody>();          
            //ボート生成時の座標をboatGeneratingPointへ設定
            boatGeneratingPoint = this.transform.position.z;
        }    
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.tag=="GoalBoatTag")
        {
            this.transform.position = new Vector3(sardine.transform.position.x, this.transform.position.y, this.transform.position.z);
        }
        else
        {
            //前方向の速度を設定
            if (70f < this.transform.position.z - sardine.transform.position.z)
            {
                this.fishingBoatVelocityZ = sardineRigid.velocity.z * 0.6f;
            }
            else if (this.transform.position.z - sardine.transform.position.z <= 70f)
            {
                this.fishingBoatVelocityZ = sardineRigid.velocity.z;
            }
            //Sin関数の角度を増加
            this.radian += 1.2d;
            //横方向の移動速度を設定
            this.fishingBoatVelocityX = (float)System.Math.Sin(radian * (System.Math.PI / 180)) * 15f;
            //一定距離を進んだら加速してオブジェクトを消去
            if (boatGeneratingPoint + 500f < this.transform.position.z)
            {
                CancelInvoke("InstantiateObstacle");
                fishingBoatVelocityX = 0;
                this.fishingBoatVelocityZ = sardineRigid.velocity.z * 2.5f;
                Invoke("boatDelete", 1.5f);
            }
            //fishingBoatの速度を設定
            this.boatRigid.velocity = new Vector3(fishingBoatVelocityX, 0, fishingBoatVelocityZ);
        }     
    }

    //障害物を生成する
    private void InstantiateObstacle()
    {
        if (this.transform.position.z - sardine.transform.position.z <= 70f)
        {
            GameObject trashBag = Instantiate(obstaclePrefab);
            trashBag.transform.position = this.transform.position;
        }
    }
    //ボートを消去
    private void boatDelete()
    {
        Destroy(gameObject);
    }
}
