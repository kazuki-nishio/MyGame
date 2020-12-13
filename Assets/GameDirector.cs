using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameDirector : MonoBehaviour
{
    //チュートリアルシーンで使用する変数
    [SerializeField] GameObject[] tutorialPanel = null;//チュートリアル用のパネルを入れる配列
    private int count = 0;//配列の添え字

    // ゲームシーンで使用する変数
    private float second = 100f;   //ゲーム開始からの経過時間
    private bool isGoal = false;  //タイマーをオフにする    
    private int startCount = 3;//ゲームスタート時のカウント   
    bool isScoreRecorded; //スコアの記録のON/OFF    
    private float score;//スコアを入れる      
    private GameObject goalText; //ゴールした際に表示するテキスト    
    private GameObject timerText;//タイマーテキスト    
    private GameObject player;//プレイヤーのオブジェクトを入れる
    private GameObject goal; //ゴールオブジェクトを入れる  
    private GameObject distanceText; //ゴールまでの距離を表示するテキスト   
    private GameObject scoreText;//スコアを表示するテキスト   
    private GameObject comboText;//コンボ数を表示するテキスト 
    private GameObject countDownText;  //ゲームスタート時のカウントダウンを表示するテキスト  
    private float toGoal;//ゴールまでの距離   
    public Slider distanceGage = null;//DistanceGageを入れる
    private float alpha = 0f;//パネルのアルファ値   
    private float alphaIncSpeed = 0.01f; //アルファ値の増加速度
    //パネルUIを入れる
    private GameObject panel;
    private float red;
    private float green;
    private float blue;

    // リザルトシーンで使用する変数
    [SerializeField] private GameObject resultScoreText = null;    //スコアテキストを入れる   
    [SerializeField] private GameObject timeScoreText = null;//タイムスコアテキストを入れる   
    [SerializeField] private GameObject totalScoreText = null;//トータルスコアテキストを入れる   
    [SerializeField] private GameObject newRecordText = null;//ニューレコードテキストを入れる   
    [SerializeField] private GameObject BestScoreText = null;//ベストスコアを表示するテキストを入れる   
    private float finishTime;//ゴール時の時間を入れる   
    private float finalScore;//ゴール時のスコアを入れる  
    private float totalScore; //トータルスコアを入れる 
    private GameObject[] particleObject;  //パーティクルシステムを入れる  
    private float bestScore = 0f; //ベストスコアを入れる
    public AudioClip buttonClickSound;//ボタン押下時の効果音
    public AudioClip ResultTextMusic;

    // Start is called before the first frame update
    void Start()
    {
        //デリゲートを登録
        SceneManager.sceneLoaded += ResultGameSceneLoded;
        //ゲームシーンでの処理
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            InvokeRepeating("CountDown", 0f, 1f);//スタートと同時にカウントダウン          
            this.player = GameObject.Find("Sardine"); //プレイヤーオブジェクトを取得           
            this.goal = GameObject.Find("Goal");//ゴールオブジェクトを取得          
            this.distanceText = GameObject.Find("DistanceText"); //DistanceTextを取得           
            this.goalText = GameObject.Find("GoalText");//GoalTextを取得           
            this.timerText = GameObject.Find("TimerText");//Timertextを取得          
            this.scoreText = GameObject.Find("ScoreText"); //Scoretextを取得          
            this.comboText = GameObject.Find("ComboText"); //Combotextを取得           
            this.countDownText = GameObject.Find("CountDownText");//CountDowntextを取得
            //パネルを取得
            this.panel = GameObject.Find("Panel");
            this.red = panel.GetComponent<Image>().color.r;
            this.green = panel.GetComponent<Image>().color.g;
            this.blue = panel.GetComponent<Image>().color.b;
        }
        ///リザルトシーンでの処理
        //トータルスコアがベストスコアを超えた場合ベストスコアを更新
        if (SceneManager.GetActiveScene().name == "ResultScene")
        {
            this.totalScore = this.finalScore + (this.finishTime * 10f);//トータルスコア取得
            bestScore = PlayerPrefs.GetFloat("RecordedScore");//ベストスコアを取得
            BestScoreText.GetComponent<Text>().text = "Best:" + bestScore.ToString("F0") + "pt";
            if (bestScore < totalScore)
            {
                PlayerPrefs.SetFloat("RecordedScore", totalScore);
                this.particleObject = GameObject.FindGameObjectsWithTag("ParticleTag");
            }
            //各種得点のUIを順番に表示
            StartCoroutine(DelayMethod(1f, () =>
            {
                resultScoreText.GetComponent<Text>().text = "Score:" + this.finalScore.ToString() + " pt";
                GetComponent<AudioSource>().PlayOneShot(ResultTextMusic, 0.5f);//効果音を再生
                StartCoroutine(DelayMethod(1f, () =>
                {
                    StartCoroutine("IndicateScore");
                }));
            }));
        }
    }
    //ゴール時の得点とタイムをリザルトシーンへ受け渡す
    private void ResultGameSceneLoded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "ResultScene")
        {
            var gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
            gameDirector.finishTime = this.second;
            gameDirector.finalScore = this.score;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //チュートリアルシーンでの処理
        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            tutorialPanel[count].SetActive(true);
            if (Input.GetKeyDown(KeyCode.RightArrow) && count < tutorialPanel.Length - 1)
            {
                count++;
                tutorialPanel[count - 1].SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && 0 < count)
            {
                count--;
                tutorialPanel[count + 1].SetActive(false);
            }
        }
        //ゲームシーンでの処理
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            //ゲームシーン開始3秒以降の処理
            if (3 < Time.timeSinceLevelLoad)
            {
                this.score = player.GetComponent<sardineController>().score; //スコアを取得               
                this.toGoal = goal.transform.position.z - player.transform.position.z; //プレーヤーとゴールまでの位置を計算                
                distanceGage.value = player.transform.position.z / goal.transform.position.z;//プレーヤーの進行度をゲージに表示
                //ゴールまでの距離をDistanceTextに表示
                if (0 <= toGoal)
                {
                    distanceText.GetComponent<Text>().text = "ゴールまで：" + toGoal.ToString("F0") + "m";
                }
                //ゴール後の処理
                if (isGoal)
                {
                    //ゴール後に画面をフェード
                    StartCoroutine(DelayMethod(4f, () =>
                    {
                        alpha += alphaIncSpeed;
                        panel.GetComponent<Image>().color = new Color(red, green, blue, alpha);
                        if (0.99f <= alpha)
                        {
                            SceneManager.LoadScene("ResultScene");
                        }
                    }));
                }
                //ゲーム開始からの経過時間を計算
                if (0 < second && player.transform.position.z < goal.transform.position.z)
                {
                    second -= Time.deltaTime;
                }
                timerText.GetComponent<Text>().text = "Time" + this.second.ToString("F2") + "sec"; //経過時間を表示               
                scoreText.GetComponent<Text>().text = "Score:" + score.ToString() + "pt";//スコアを表示               
                comboText.GetComponent<Text>().text = player.GetComponent<sardineController>().combo.ToString() + "combo"; //コンボ数を表示
            }
        }
    }

    //プレーヤーがゴールしたことを判定する
    public void PlayerGoal()
    {
        isGoal = true;
        goalText.GetComponent<Text>().text = "GOAL!!";
    }
    //ゲーム開始と同時にカウントダウン
    private void CountDown()
    {
        if (0 < startCount)
        {
            countDownText.GetComponent<Text>().text = startCount.ToString();
            startCount--;
        }
        else if (startCount == 0)
        {
            countDownText.GetComponent<Text>().text = "Start!!";
            Invoke("CountDownTextEnable", 1f);
        }
    }
    //CountDownを無効化
    private void CountDownTextEnable()
    {
        CancelInvoke();
        Destroy(countDownText);
    }
    /// <summary>
    /// ボタンを押した際のイベント
    public void PushButton(GameObject button)
    {
        GetComponent<AudioSource>().PlayOneShot(buttonClickSound, 0.3f);//効果音を鳴らす
        //ゲームシーンを読み込む
        if (button.gameObject.tag == "GameSceneTag")
        {
            SceneManager.LoadScene("GameScene");
        }
        //チュートリアルシーンを読み込む
        else if (button.gameObject.tag == "TutorialSceneTag")
        {
            SceneManager.LoadScene("TutorialScene");
        }
        //タイトルシーンを読み込む
        else if (button.gameObject.tag == "TitleSceneTag")
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

    //メソッドの処理にDelayをかける
    IEnumerator DelayMethod(float delayTime, System.Action action)
    {
        yield return new WaitForSeconds(delayTime);
        action();
    }
    //各種得点を表示するコルーチン
    IEnumerator IndicateScore()
    {
        timeScoreText.GetComponent<Text>().text = "Time:" + finishTime.ToString("F1") + "×10 = " + (this.finishTime * 10f).ToString("F0") + " pt";
        GetComponent<AudioSource>().PlayOneShot(ResultTextMusic, 0.5f);//効果音を再生
        yield return new WaitForSeconds(1f);
        totalScoreText.GetComponent<Text>().text = "Total:" + totalScore.ToString("F0") + " pt";
        GetComponent<AudioSource>().PlayOneShot(ResultTextMusic, 0.5f);//効果音を再生
        //ハイスコアならニューレコードテキストを表示し、パーティクルを再生
        if (bestScore < totalScore)
        {
            StartCoroutine(DelayMethod(1.5f, () =>
            {
                resultScoreText.SetActive(false);
                timeScoreText.SetActive(false);
                newRecordText.SetActive(true);
                newRecordText.GetComponent<Text>().text = "New Record!!";
                GetComponent<AudioSource>().Play();//効果音を再生
                for (int i = 0; i < particleObject.Length; i++)
                {
                    GameObject particle = particleObject[i];
                    particle.GetComponent<ParticleSystem>().Play();
                }
            }));
        }
    }
}

