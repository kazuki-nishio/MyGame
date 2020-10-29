using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class sardineController : MonoBehaviour
{
    //Sardineの移動をさせるためのコンポーネントを入れる
    private Rigidbody myRigidBody;
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //前方向の速度
    private float velocityZ = 16f;
    //横方向の速度
    private float velocityX = 10f;
    //横方向の移動範囲
    private float moveX = 4f;
    //減速させる係数
    private float coefficient = 0.99f;
    //ゲームオーバーの判定
    private bool isEnd = false;
    //FieldCheckPointPefabをいれる
    public GameObject FieldCheckPointPefab;
    //フィールドの生成範囲
    private float fieldStart = 70f;
    private float fieldEnd = 160f;

    // Start is called before the first frame update
    void Start()
    {
        //sardineのrigidbodyコンポーネントを取得
        this.myRigidBody = GetComponent<Rigidbody>();
        //sardineのAnimationコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //x軸方向の速度を初期化
        float InputVelocityX = 0;
        //左矢印が押されたら左方向の速度を代入
        if (Input.GetKey(KeyCode.LeftArrow) && -this.moveX < this.transform.position.x)
        {
            InputVelocityX = -this.moveX;
        }
        //右矢印が押されたら右方向の速度を代入
        if (Input.GetKey(KeyCode.RightArrow) && this.transform.position.x < this.moveX)
        {
            InputVelocityX = this.moveX;
        }
        //ゲームオーバー時に減速
        if (this.isEnd)
        {
            this.velocityX *= coefficient;
            this.velocityZ *= coefficient;
            this.myAnimator.speed *= coefficient;
            //マウスのボタンが押されたら最初のシーンに戻る
            if (Input.GetMouseButtonDown(0))
            {
                //SampleSceneを読み込む
                SceneManager.LoadScene("SampleScene");
            }
        }
        //Sardineの速度を設定
        this.myRigidBody.velocity = new Vector3(InputVelocityX, 0, this.velocityZ);
    }
    private void OnTriggerEnter(Collider other)
    {
        this.isEnd = true;
        //FieldCheckPointの場合は素通り
        if(other.gameObject.tag== "FieldCheckPointTag")
        {
            this.isEnd = false;
        }
    }
}
