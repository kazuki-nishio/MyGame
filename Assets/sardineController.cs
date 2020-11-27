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
    //加速中のアニメーションのスピード
    private float accelAnimatorSpeed;
    //通常時の前方向の速度
    public float velocityZ = 15f;
    //無敵状態の時の前方向の速度
    public float invincibleVelocityZ = 60f;
    //ゲーム開始時の前方向の速度
    private float firstVelocityZ;
    //横方向の速度
    private float velocityX = 10f;
    //横方向の移動範囲
    private float moveX = 3f;
    //加速した時の速度
    private float accelVelocityZ = 50f;
    //減速させる係数
    public float decel = 0.99f;
    //ゲームオーバーの判定
    private bool isEnd = false;
    //無敵状態の判定
   [System.NonSerialized] public bool isInvincible = false;
    //無敵状態へ移行するためのポイント
    [System.NonSerialized] public float invinciblePoint;
    //スコアを入れる
   [System.NonSerialized] public float score = 0;
    //通常時の金のエビのスコア
    public float goldShrimpScore = 50f;
    //通常時の赤いエビのスコア
    public float redShrimpScore = 10f;
    //無敵状態の金のエビのスコア
    private float invincibleGoldShrimpScore;
    //無敵状態の赤のエビのスコア
    private float invincibleRedShrimpScore;
    //コンボ
    [System.NonSerialized] public int combo = 0;
    //コンボボーナスポイント
    public int comboBonusPoint = 100;
    //ボーナスポイントのテキスト
    private GameObject comboBonusText;
    //コンボの累積値
    private int accumulateCombo = 0;
    //コリダーを入れる
    Collider m_collider = null;
    //スペースキー入力の有効・無効を判定するための変数
    private bool spaceSwitch = true;

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
        //加速中のアニメーションのスピード
        this.accelAnimatorSpeed = firstAnimator + 0.6f;
    }

    // Update is called once per frame
    void Update()
    {
        //ゲームシーン読み込みから3秒までの処理
        if (Time.timeSinceLevelLoad <= 3f)
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
                this.velocityX *= decel;
                this.velocityZ *= decel;
                this.invincibleVelocityZ *= decel;
                this.myAnimator.speed *= decel;
            }
            //無敵状態のとき速度
            if (isInvincible)
            {
                this.velocityZ = invincibleVelocityZ;
                //無敵時に巨大化
                this.transform.localScale = new Vector3(20, 20, 20);
            }
            //非無敵状態のときの速度
            else if(!isInvincible)
            {
                this.transform.localScale = new Vector3(10, 10, 10);
                //スペースキーが押されるとmaxSpeedまで加速
                if (Input.GetButton("Jump") && !isEnd && spaceSwitch)
                {
                    this.velocityZ = accelVelocityZ;
                    this.myAnimator.speed = accelAnimatorSpeed;
                }
                //離すと速度を初期状態に戻す
                else if (Input.GetButtonUp("Jump") && !isEnd)
                {
                    this.velocityZ = firstVelocityZ;
                    this.myAnimator.speed = firstAnimator;
                }
                else
                {
                    this.velocityZ = firstVelocityZ;
                    this.myAnimator.speed = firstAnimator;
                }
            }
            //Sardineの速度を設定
            this.myRigidBody.velocity = new Vector3(InputVelocityX, 0, this.velocityZ);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //障害物以外に触れたときの処理
        if (other.gameObject.tag != "ObstacleTag" && other.gameObject.tag != "GoalTag")
        {
            //パーティクルを再生
            GetComponent<ParticleSystem>().Play();
            //コンボ数を増加
            combo++;
            accumulateCombo++;
            //アイテムを削除
            Destroy(other.gameObject);
            //10コンボごとにボーナス
            if (accumulateCombo == 10)
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
                //得点を取得
                score += goldShrimpScore;
                //無敵ポイントを取得
                invinciblePoint += 30f;               
            }
            //赤いエビに触れると得点を獲得し一定時間速度を上昇させる
            if (other.gameObject.tag == "NormalItemTag")
            {
                score += redShrimpScore;
                invinciblePoint += 10f;
            }
            //障害物に触れたときの処理
            if (other.gameObject.tag == "ObstacleTag")
            {
                //コンボ数をリセット
                combo = 0;
                //コンボの累積値リセット
                accumulateCombo = 0;
                //プレーヤーの速度を初期速度へ
                this.velocityZ = firstVelocityZ;
                this.myAnimator.speed = firstAnimator;
                //スペースキーの入力を1秒間無効化
                spaceSwitch = false;
                Invoke("SpaceActive", 1f);
                //オブジェクトを点滅させる
                this.myAnimator.Play("Invincible");
            }
            //一定以上の無敵ポイントを取得で無敵状態へ移行
            if(100f <= invinciblePoint)
            {
                isInvincible = true;
                this.myAnimator.speed += 0.6f;
                invinciblePoint = 0f;
                //5秒後に解除
                Invoke("UnInvincible", 5f);
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
    //コンボボーナステキストを非表示に
    private void ComboBonusTextEnable()
    {
        comboBonusText.GetComponent<Text>().text = "";
    }
    //スペースキーの入力を有効化
    private void SpaceActive()
    {
        spaceSwitch = true;
    }
}
