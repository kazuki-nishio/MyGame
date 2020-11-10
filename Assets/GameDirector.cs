using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    //ゲーム開始からの経過時間
    private float second = 100f;
    //タイマーをオフにする
    private bool isGoal = false;
    //ベストタイム用の変数
    private float bestTime = 999f;
    //スコアの記録のON/OFF
    bool isScoreRecorded;
    //bestTimeTextを参照
    public UnityEngine.UI.Text bestTimeText;
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
    //スコアを入れる
    private float nowScore;
    //ゴールまでの距離
    private float toGoal;
    //DistanceGageを入れる
    public Slider distanceGage;

    // Start is called before the first frame update
    void Start()
    {
        distanceGage.value = 0;
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
        //RecordedTimeに保存された値を bestTimeTextに表示
        if (PlayerPrefs.HasKey("RecordedTime"))
        {
            float recordedTime = PlayerPrefs.GetFloat("RecordedTime");
            bestTime = recordedTime;
            if (recordedTime > 0)
            {
                bestTimeText.text = "Best:" + recordedTime.ToString("F2") + "sec";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
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
            //RecordedTimeに経過時間を保存
            if (!isScoreRecorded)
            {
                //スコアがよければbestTimeを更新
                if (second < bestTime)
                {
                    PlayerPrefs.SetFloat("RecordedTime", second);
                }
                //経過時間の計測をやめる
                isScoreRecorded = true;
            }
            //マウスのボタンが押されたら最初のシーンに戻る
            if (Input.GetMouseButtonDown(0))
            {
                //SampleSceneを読み込む
                SceneManager.LoadScene("SampleScene");
            }
            return;
        }
        //ゲーム開始からの経過時間を計算
        if (0 < second)
        {
            second -= Time.deltaTime;
        }
        //経過時間を表示
        timerText.GetComponent<Text>().text = "Time" + this.second.ToString("F2") + "sec";
        //スコアを表示
        this.nowScore = player.GetComponent<sardineController>().score;
        scoreText.GetComponent<Text>().text = "Score:" + nowScore.ToString() + "pt";
    }

    //プレーヤーがゴールしたことを判定する
    public void PlayerGoal()
    {
        isGoal = true;
        goalText.GetComponent<Text>().text = "GOAL!!";
    }
}

