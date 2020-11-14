using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class sardineController : MonoBehaviour
{
    //Sardineの移動をさせるためのコンポーネントを入れる
    private Rigidbody myRigidBody;
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //ゲーム開始時のアニメーションのスピード
    private float firstAnimator;
    //通常時の前方向の速度
    public float velocityZ = 15f;
    //無敵状態の時の前方向の速度
    public float invincibleVelocityZ = 50f;
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
    //無敵状態の判定
    private bool isInvincible = false;
    //スコアを入れる
    public float score = 0;
    //通常時の金のエビのスコア
    public float goldShrimpScore = 50f;
    //通常時の赤いエビのスコア
    public float redShrimpScore = 10f;
    //無敵状態の金のエビのスコア
    private float invincibleGoldShrimpScore;
    //無敵状態の赤のエビのスコア
    private float invincibleRedShrimpScore;
    //コンボ
    public int combo = 0;
    //コンボボーナスポイント
    public int comboBonusPoint = 100;
    //ボーナスポイントのテキスト
    private GameObject comboBonusText;
    //コンボの累積値
    private int accumulateCombo = 0;
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
        //コンボボーナステキストを取得
        this.comboBonusText = GameObject.Find("BonusPointText");
        //無敵状態の金のエビのスコア
        invincibleGoldShrimpScore = goldShrimpScore * 1.1f;
        //無敵状態の赤のエビのスコア
        invincibleRedShrimpScore = redShrimpScore * 1.1f;
    }

    // Update is called once per frame
    void Update()
    {
        //ゲームシーン読み込みから3秒までの処理
        if(Time.timeSinceLevelLoad <= 3f)
        {
            //プレイヤーの速度ベクトルを0に
            this.myRigidBody.velocity = new Vector3(0, 0, 0);
        }
        //3秒以降の処理
        else
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
            }
            //無敵状態のときは速度を上昇
            if (isInvincible)
            {
                this.myRigidBody.velocity = new Vector3(InputVelocityX, 0, this.invincibleVelocityZ);
            }
            else
            {
                //Sardineの速度を設定
                this.myRigidBody.velocity = new Vector3(InputVelocityX, 0, this.velocityZ);
            }
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        //障害物以外に触れるとコンボ数を増加
        if (other.gameObject.tag != "ObstacleTag")
        {
            combo++;
            accumulateCombo++;
            //10コンボごとにボーナス
            if(accumulateCombo==10)
            {
                score += comboBonusPoint;
                accumulateCombo = 0;
                comboBonusText.GetComponent<Text>().text = "Bonus +" + comboBonusPoint + "pt!!";
                Invoke("ComboBonusTextEnable", 1f);
            }
        }
        //非無敵状態での処理
        if (!isInvincible)
        {
            //金のエビに触れると無敵状態へ移行
            if (other.gameObject.tag == "InvincibleTag")
            {
                isInvincible = true;
                this.myAnimator.speed += 0.6f;
                //得点を取得
                score += goldShrimpScore;
                //5秒後に解除
                Invoke("UnInvincible", 5f);
            }
            //赤いエビに触れると得点を獲得し一定時間速度を上昇させる
            if (other.gameObject.tag == "NormalItemTag")
            {
                score += redShrimpScore;
                if (velocityZ < maxSpeed)
                {
                    this.myAnimator.speed += 0.3f;
                    this.velocityZ += 10f;
                    Invoke("ResetSpeed", 0.8f);
                }
            }
            //障害物に触れたときの処理
            if (other.gameObject.tag == "ObstacleTag")
            {
                //コンボ数をリセット
                combo = 0;
                //コンボの累積値リセット
                accumulateCombo = 0;
                //オブジェクトを点滅させる
                this.myAnimator.Play("Invincible");
            }
        }
        //無敵時の処理
        else if (isInvincible)
        {
            //獲得得点を上昇
            if (other.gameObject.tag == "InvincibleTag")
            {
                score += invincibleGoldShrimpScore;
            }
            //獲得得点を上昇
            if (other.gameObject.tag == "NormalItemTag")
            {
                score += invincibleRedShrimpScore;
            }
            //無敵状態で障害物に触れると障害物を破壊
            if (other.gameObject.tag == "ObstacleTag")
            {
                Destroy(other.gameObject);
            }
        }
        //ゴールに到着するとゲーム終了
        if (other.gameObject.tag == "GoalTag")
        {
            isEnd = true;
            //isGoalをtrueにする
            GameObject.Find("GameDirector").GetComponent<GameDirector>().PlayerGoal();
        }
    }
    //無敵状態を解除
    private void UnInvincible()
    {
        isInvincible = false;
        this.myAnimator.speed = firstAnimator;
    }
    //赤いエビによって上昇した速度をリセット
    private void ResetSpeed()
    {
        this.myAnimator.speed = firstAnimator;
        this.velocityZ = firstVelocityZ;
    }
    //コンボボーナステキストを非表示に
    private void ComboBonusTextEnable()
    {
        comboBonusText.GetComponent<Text>().text = "";
    }
}
