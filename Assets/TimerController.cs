using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    //ゲーム開始からの経過時間
    private float second = 0f;
    //タイマーをオフにする
    private bool isGoal=false;
    //ベストタイム用の変数
    private float bestTime;
    //スコアの記録のON/OFF
    bool isScoreRecorded;
    //bestTimeTextを参照
    public UnityEngine.UI.Text bestTimeText;

    // Start is called before the first frame update
    void Start()
    {
        //RecordedTimeに保存された値を bestTimeTextに表示
        float recordedTime = PlayerPrefs.GetFloat("RecordedTime");
        bestTime = recordedTime;
        if(recordedTime > 0)
        {
            bestTimeText.text ="Best:"+ recordedTime.ToString("F2")+"sec";
        }
    }

    // Update is called once per frame
    void Update()
    {
        //ゴール後の処理
        if(isGoal)
        {
            //RecordedTimeに経過時間を保存
            if (!isScoreRecorded)
            {
                //スコアがよければbestTimeを更新
                if(second < bestTime)
                {
                    PlayerPrefs.SetFloat("RecordedTime", second);
                }
                //経過時間の計測をやめる
                isScoreRecorded = true;
            }
            return;
        }
        //ゲーム開始からの経過時間を計算
        second += Time.deltaTime;
        //経過時間を表示
        this.GetComponent<Text>().text = "Time" + this.second.ToString("F2") + "sec";
    }

    //プレーヤーがゴールしたことを判定する
    public void PlayerGoal()
    {
        isGoal = true;
    }
}
