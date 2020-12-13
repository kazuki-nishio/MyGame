using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class sardineController : MonoBehaviour
{
    private Rigidbody myRigidBody;//Sardineの移動をさせるためのコンポーネントを入れる   
    private Animator myAnimator;//アニメーションするためのコンポーネントを入れる   
    private float firstAnimator;//ゲーム開始時のアニメーションのスピード    
    private float accelAnimatorSpeed;//加速中のアニメーションのスピード   
    public float velocityZ = 15f;//通常時の前方向の速度    
    public float invincibleVelocityZ = 90f;//無敵状態の時の前方向の速度    
    private float firstVelocityZ;//ゲーム開始時の前方向の速度    
    private float velocityX = 10f;//横方向の速度   
    private float moveX = 3f;//横方向の移動範囲   
    private float accelVelocityZ = 50f;//加速した時の速度   
    public float decel = 0.99f;//減速させる係数   
    private bool isEnd = false;//ゲームオーバーの判定   
    [System.NonSerialized] public bool isInvincible = false;//無敵状態の判定    
    [System.NonSerialized] public float invinciblePoint;//無敵状態へ移行するためのポイント    
    [System.NonSerialized] public float score = 0;//スコアを入れる 
    private float goldShrimpScore = 50f;//通常時の金のエビのスコア   
    private float redShrimpScore = 10f; //通常時の赤いエビのスコア  
    private float finalShrimpScore = 500f;
    private float invincibleGoldShrimpScore;//無敵状態の金のエビのスコア   
    private float invincibleRedShrimpScore;//無敵状態の赤のエビのスコア    
    [System.NonSerialized] public int combo = 0;//コンボ    
    public int comboBonusPoint = 100;//コンボボーナスポイント   
    private GameObject comboBonusText;//ボーナスポイントのテキスト    
    private int accumulateCombo = 0;//コンボの累積値    
    Collider m_collider = null;//コリダーを入れる    
    private bool spaceSwitch = true;//スペースキー入力の有効・無効を判定するための変数    
    private GameObject bigExplosion;//BigExplosionオブジェクトを入れる
    public AudioClip getShrimp;//アイテム取得時の効果音
    public AudioClip getGarbage;//障害物接触時の効果音
    public AudioClip explode;//ゴール時の音
    public AudioClip invincibleMusic;//無敵移行時の音

    // Start is called before the first frame update
    void Start()
    {
        this.bigExplosion = GameObject.Find("BigExplosion"); //BigExplosionオブジェクトを取得       
        this.myRigidBody = GetComponent<Rigidbody>(); //sardineのrigidbodyコンポーネントを取得       
        this.myAnimator = GetComponent<Animator>();//sardineのAnimationコンポーネントを取得        
        firstVelocityZ = velocityZ;//ゲーム開始時の前方向の速度を初期化        
        firstAnimator = this.myAnimator.speed;//ゲーム開始時のアニメーションのスピードを初期化        
        m_collider = GetComponent<Collider>();//コリダーコンポーネントを取得       
        this.comboBonusText = GameObject.Find("BonusPointText"); //コンボボーナステキストを取得       
        invincibleGoldShrimpScore = goldShrimpScore * 1.1f;//無敵状態の金のエビのスコア       
        invincibleRedShrimpScore = redShrimpScore * 1.1f;//無敵状態の赤のエビのスコア       
        this.accelAnimatorSpeed = firstAnimator + 0.6f;//加速中のアニメーションのスピード
    }

    // Update is called once per frame
    void Update()
    {
        //ゲームシーン読み込みから3秒までの処理
        if (Time.timeSinceLevelLoad <= 3f)
        {
            this.myRigidBody.velocity = new Vector3(0, 0, 0); //プレイヤーの速度ベクトルを0に
        }
        //3秒以降の処理
        else
        {
            float InputVelocityX = 0;//x軸方向の速度を初期化
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
            if (isEnd)
            {
                this.velocityX *= decel;
                this.velocityZ *= decel;
                this.invincibleVelocityZ *= decel;
                this.myAnimator.speed *= decel;
            }
            else if (!isEnd)
            {
                //無敵状態での処理
                if (isInvincible)
                {
                    this.velocityZ = invincibleVelocityZ;//無敵状態のとき速度                    
                    this.transform.localScale = new Vector3(20, 20, 20);//無敵時に巨大化
                }
                //非無敵状態のときの速度
                else if (!isInvincible)
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
            GetComponent<ParticleSystem>().Play();//パーティクルを再生                      
            GetComponent<AudioSource>().PlayOneShot(getShrimp, 0.5f);//アイテム取得時の効果音を再生
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
            //金のエビに触れた場合の処理
            if (other.gameObject.tag == "InvincibleTag")
            {

                score += goldShrimpScore; //得点を取得              
                invinciblePoint += 30f; //無敵ポイントを取得           
            }
            //赤いエビに触れた場合の処理
            else if (other.gameObject.tag == "NormalItemTag")
            {
                score += redShrimpScore;
                invinciblePoint += 10f;
            }
            //巨大エビに触れた際の処理
            else if (other.gameObject.tag == "FinalItemTag")
            {
                score += finalShrimpScore;
                isInvincible = true;
                GetComponent<AudioSource>().PlayOneShot(invincibleMusic, 0.5f);
            }
            //障害物に触れたときの処理
            else if (other.gameObject.tag == "ObstacleTag")
            {
                GetComponent<AudioSource>().PlayOneShot(getGarbage, 0.5f);
                combo = 0;           //コンボ数をリセット               
                accumulateCombo = 0; //コンボの累積値リセット
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
            if (100f <= invinciblePoint)
            {
                GetComponent<AudioSource>().PlayOneShot(invincibleMusic, 0.8f);
                isInvincible = true;
                this.myAnimator.speed += 0.6f;
                invinciblePoint = 0f;
                Invoke("UnInvincible", 8f);//8秒後に解除
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
            //BigExplosionを再生
            bigExplosion.transform.position = this.transform.position;
            bigExplosion.GetComponent<ParticleSystem>().Play();
            //効果音を再生
            GetComponent<AudioSource>().PlayOneShot(explode, 0.7f);
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
