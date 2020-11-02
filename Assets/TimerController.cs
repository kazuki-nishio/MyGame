using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    //ゲーム開始からの経過時間
    private float second = 0f;
    //タイマーをオフにする
    private bool isGoal=false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //ゴール後は処理を行わないようにする
        if(isGoal)
        {
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
