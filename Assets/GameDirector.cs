using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    //ゲーム開始からの経過時間
    private float second = 0f;
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
    //ゴールまでの距離
    private float toGoal;


    // Start is called before the first frame update
    void Start()
    {
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
        //RecordedTimeに保存された値を bestTimeTextに表示
        if(PlayerPrefs.HasKey("RecordedTime"))
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
        if (0 <= toGoal)
        {
            //プレーヤーとゴールまでの位置を計算
            this.toGoal = goal.transform.position.z - player.transform.position.z;
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
        second += Time.deltaTime;
        //経過時間を表示
        timerText.GetComponent<Text>().text = "Time" + this.second.ToString("F2") + "sec";    
    }

    //プレーヤーがゴールしたことを判定する
    public void PlayerGoal()
    {
        isGoal = true;
        goalText.GetComponent<Text>().text = "GOAL!!";
    }
}

