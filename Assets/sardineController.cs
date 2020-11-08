using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class sardineController : MonoBehaviour
{
    //Sardineの移動をさせるためのコンポーネントを入れる
    private Rigidbody myRigidBody;
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //ゲーム開始時のアニメーションのスピード
    private float firstAnimator;
    //前方向の速度
    private float velocityZ = 15f;
    //ゲーム開始時の前方向の速度
    private float firstVelocityZ;
    //横方向の速度
    private float velocityX = 10f;
    //横方向の移動範囲
    private float moveX = 3f;
    //減速させる係数
    private float coefficient = 0.99f;
    //前方向の速度の上昇限界値
    float maxSpeed = 50f;
    //ゲームオーバーの判定
    private bool isEnd = false;
    //コリダーを入れる
    Collider m_collider = null;

    // Start is called before the first frame update
    void Start()
    {
        //sardineのrigidbodyコンポーネントを取得
        this.myRigidBody = GetComponent<Rigidbody>();
        //sardineのAnimationコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();
        //ゲーム開始時の前方向の速度を初期化
        firstVelocityZ = velocityZ;
        //ゲーム開始時のアニメーションのスピードを初期化
        firstAnimator = this.myAnimator.speed;
        //コリダーコンポーネントを取得
        m_collider = GetComponent<Collider>();
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
        //クリア時に減速
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
        //SpeedUpに触れると速度を上昇させる
        if (other.gameObject.tag == "SpeedUpTag" && this.velocityZ < maxSpeed)
        {
            this.myAnimator.speed += 0.2f;
            this.velocityZ += 8f;
        }
        //障害物に触れると速度の上昇値をリセットする
        if (other.gameObject.tag == "ObstacleTag")
        {
            this.myAnimator.speed = firstAnimator;
            this.velocityZ = firstVelocityZ;
            //コリダーを無効化する
            m_collider.enabled = false;
            //オブジェクトを点滅させる
            this.myAnimator.Play("Invincible");
            //衝突から1秒後にコリダーを有効にする
            Invoke("EnableCollider", 1f);
        }
        //ゴールに到着するとゲーム終了
        if (other.gameObject.tag == "GoalTag")
        {
            this.isEnd = true;
            //タイマーを止める
            GameObject.Find("GameDirector").GetComponent<GameDirector>().PlayerGoal();
        }
    }
    //コリダーを有効にする
    private void EnableCollider()
    {
        m_collider.enabled = true;
    }
}
