using System.Collections;
using UnityEngine.UI;
using UnityEngine;


public class InvincibleGageController : MonoBehaviour
{
    //プレーヤーオブジェクトを入れる
    private GameObject Sardine;
    //InvinciblePointを入れる
    private float invinciblePoint;
    //スライダーコンポーネントを入れる
    private Slider mySlider;
    //無敵の効果時間
    private float duration = 5f;
    //durationの初期値
    private float firstDuration;
    //無敵状態の判定
    private bool isInvincible;
    // Start is called before the first frame update
    void Start()
    {
        this.Sardine = GameObject.Find("Sardine");       
        this.mySlider = GetComponent<Slider>();      
        firstDuration = duration;
    }

    // Update is called once per frame
    void Update()
    {
            this.isInvincible = Sardine.GetComponent<sardineController>().isInvincible;
            //取得したinvinciblePointを表示
            this.invinciblePoint = Sardine.GetComponent<sardineController>().invinciblePoint;
            if (!isInvincible)
            {
                mySlider.value = invinciblePoint / 100f;
            }
            else if (isInvincible)
            {
                duration -= Time.deltaTime;
                //無敵状態の効果時間をゲージに表示
                this.mySlider.value = duration / firstDuration;
                if (duration <= 0)
                {
                    duration = firstDuration;
                }
            }       
    }
}
