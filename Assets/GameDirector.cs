using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameDirector : MonoBehaviour
{
    /// <summary>
    /// ゲームシーンで使用する変数
    /// </summary>
    //ゲーム開始からの経過時間
    private float second = 100f;
    //タイマーをオフにする
    private bool isGoal = false;
    //ゲームスタート時のカウント
    private int startCount = 3;
    //スコアの記録のON/OFF
    bool isScoreRecorded;
    //スコアを入れる
    private float score;
    //bestTimeTextを参照
    public UnityEngine.UI.Text bestTimeText = null;
    //ゴールした際に表示するテキスト
    private GameObject goalText;
    //タイマーテキスト
    private GameObject timerText;
    //プレイヤーのオブジェクトを入れる
    private GameObject player;
    //ゴールオブジェクトを入れる
    private GameObject goal;
    //ゴールまでの距離を表示するテキスト
    private GameObject distanceText;
    //スコアを表示するテキスト
    private GameObject scoreText;
    //コンボ数を表示するテキスト
    private GameObject comboText;
    //ゲームスタート時のカウントダウンを表示するテキスト
    private GameObject countDownText;
    //ゴールまでの距離
    private float toGoal;
    //DistanceGageを入れる
    public Slider distanceGage = null;
    //パネルUIを入れる
    private GameObject panel;
    private float red;
    private float green;
    private float blue;
    //パネルのアルファ値
    private float alpha = 0f;
    //アルファ値の増加速度
    private float alphaIncSpeed = 0.01f;

    /// リザルトシーンで使用する変数
    //スコアテキストを入れる
    [SerializeField] private GameObject resultScoreText = null;
    //タイムスコアテキストを入れる
    [SerializeField] private GameObject timeScoreText = null;
    //トータルスコアテキストを入れる
    [SerializeField] private GameObject totalScoreText = null;
    //ニューレコードテキストを入れる
    [SerializeField] private GameObject newRecordText = null;
    //ベストスコアを表示するテキストを入れる
    [SerializeField] private GameObject BestScoreText = null;
    //ゴール時の時間を入れる
    private float finishTime;
    //ゴール時のスコアを入れる
    private float finalScore;
    //トータルスコアを入れる
    private float totalScore;
    //パーティクルシステムを入れる
    private GameObject[] particleObject;
    //ベストスコアを入れる
    private float bestScore = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //デリゲートを登録
        SceneManager.sceneLoaded += ResultGameSceneLoded;
        //ゲームシーンでの処理
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            //スタートと同時にカウントダウン
            InvokeRepeating("CountDown", 0f, 1f);
            //プレイヤーオブジェクトを取得
            this.player = GameObject.Find("Sardine");
            //ゴールオブジェクトを取得
            this.goal = GameObject.Find("Goal");
            //DistanceTextを取得
            this.distanceText = GameObject.Find("DistanceText");
            //GoalTextを取得
            this.goalText = GameObject.Find("GoalText");
            //Timertextを取得
            this.timerText = GameObject.Find("TimerText");
            //Scoretextを取得
            this.scoreText = GameObject.Find("ScoreText");
            //Combotextを取得
            this.comboText = GameObject.Find("ComboText");
            //CountDowntextを取得
            this.countDownText = GameObject.Find("CountDownText");
            //パネルを取得
            this.panel = GameObject.Find("Panel");
            this.red = panel.GetComponent<Image>().color.r;
            this.green = panel.GetComponent<Image>().color.g;
            this.blue = panel.GetComponent<Image>().color.b;
        }
        if (SceneManager.GetActiveScene().name == "ResultScene")
        {
            this.totalScore = this.finalScore + (this.finishTime * 10f);
            bestScore = PlayerPrefs.GetFloat("RecordedScore");
            BestScoreText.GetComponent<Text>().text = "Best：" + bestScore.ToString("F0") + "pt";
            if (bestScore < totalScore)
            {
                PlayerPrefs.SetFloat("RecordedScore", totalScore);
                this.particleObject = GameObject.FindGameObjectsWithTag("ParticleTag");
            }
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
        //ゲームシーンでの処理
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            //ゲームシーン開始3秒以降の処理
            if (3 < Time.timeSinceLevelLoad)
            {
                //スコアを取得
                this.score = player.GetComponent<sardineController>().score;
                //プレーヤーとゴールまでの位置を計算
                this.toGoal = goal.transform.position.z - player.transform.position.z;
                //プレーヤーの進行度をゲージに表示
                distanceGage.value = player.transform.position.z / goal.transform.position.z;
                if (0 <= toGoal)
                {
                    //ゴールまでの距離をDistanceTextに表示
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
                if (0 < second)
                {
                    second -= Time.deltaTime;
                }
                //経過時間を表示
                timerText.GetComponent<Text>().text = "Time" + this.second.ToString("F2") + "sec";
                //スコアを表示
                scoreText.GetComponent<Text>().text = "Score:" + score.ToString() + "pt";
                //コンボ数を表示
                comboText.GetComponent<Text>().text = player.GetComponent<sardineController>().combo.ToString() + "combo";
            }
        }
        //各種得点のUIを順番に表示
        if (SceneManager.GetActiveScene().name == "ResultScene")
        {
            StartCoroutine(DelayMethod(1f, () =>
            {
                resultScoreText.GetComponent<Text>().text = "Score：" + this.finalScore.ToString() + " pt";
                StartCoroutine(DelayMethod(1f, () =>
                 {
                     StartCoroutine("IndicateScore");
                 }));
            }));
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
    /// BackToTitlePush：タイトルシーンを読み込む
    /// RestartPush：ゲームシーンを読み込む
    public void BackToTitlePush()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void RestartPush()
    {
        SceneManager.LoadScene("SampleScene");
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
        timeScoreText.GetComponent<Text>().text = "Time：" + finishTime.ToString("F1") + "×10 = " + (this.finishTime * 10f).ToString("F0") + " pt";
        yield return new WaitForSeconds(1f);
        totalScoreText.GetComponent<Text>().text = "Total：" + totalScore.ToString("F0") + " pt";
        //ハイスコアならニューレコードテキストを表示し、パーティクルを再生
        if (bestScore < totalScore)
        {
            StartCoroutine(DelayMethod(1.5f, () =>
            {
                resultScoreText.SetActive(false);
                timeScoreText.SetActive(false);
                newRecordText.SetActive(true);
                newRecordText.GetComponent<Text>().text = "New Record!!";
                for (int i = 0; i < particleObject.Length; i++)
                {
                    GameObject particle = particleObject[i];
                    particle.GetComponent<ParticleSystem>().Play();
                }
            }));
        }
    }
}

