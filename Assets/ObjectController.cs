using System.Collections;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    /// <summary>背景となるオブジェクト</summary>
    public GameObject m_background = null;
    /// <summary>背景となるオブジェクトの奥行</summary>
    public float m_depth = 100f;
    /// <summary>背景となるオブジェクトを奥にいくつ並べるか</summary>
    public int m_count = 4;
    /// <summary>プレイヤーのオブジェクト</summary>
    GameObject m_player = null;
    /// <summary>背景のオブジェクト（複製して複数になる）を入れておく配列</summary>
    GameObject[] m_backgroundObjects;
    /// <summary>m_backgroundObjects 配列に指定する添え字</summary>
    int index = 0;
    /// <summary>最後に背景を移動させた時のプレイヤーの位置のZ座標</summary>
    float m_lastTimeMovedBackgroundPositionZ;
    public float itemGenerateRange = 80f;    //プレイヤーの前方何mまでアイテムを生成するか   
    public float interval = 20f;//アイテムの生成間隔   
    public GameObject trashBagPrefab;//trashBagPrefabを入れる    
    public GameObject redShrimpPrefab;//redShrimpPrefabを入れる   
    public GameObject goldShrimpPrefab; //goldShrimpPrefabを入れる   
    public GameObject finalShrimpPrefab;//finalShrimpPrefabを入れる
    public GameObject goal; //Goalオブジェクトを入れる    
    private float posRangeX = 3.4f;//アイテムの生成範囲    
    float lastTimeGenerateItemPositionZ;//最後にアイテムを生成した時のプレイヤーの位置のZ座標    
    public GameObject fishinBoatPrefab;//FishinBoatPrefab    
    public GameObject GoalBoatPrefab;//GoalBoatPrefab    
    private bool isBoat = false;//FishinBoatを1つだけ生成するための変数
    private bool isShrimp = false;//FinalShrimpを1つだけ生成するための変数

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        //lastTimeMovedBackgroundPositionZをプレイヤーの初期位置で初期化
        float m_lastTimeMovedBackgroundPositionZ = m_player.transform.position.z;
        m_backgroundObjects = new GameObject[m_count];
        m_backgroundObjects[0] = m_background;
        // m_count の数だけ背景を奥に並べる
        for (int i = 1; i < m_count; i++)
        {
            GameObject go = Instantiate(m_background);
            go.transform.position = m_background.transform.position + Vector3.forward * m_depth * i;
            m_backgroundObjects[i] = go;
        }
        //lastTimeGenerateItemPositionZをプレイヤーの初期位置で初期化
        lastTimeGenerateItemPositionZ = m_player.transform.position.z;
        //障害物をプレーヤーの前方80m先まで生成
        for (float i = m_player.transform.position.z + 20f; i <= m_player.transform.position.z + itemGenerateRange; i += interval)
        {
            int num = Random.Range(1, 7);
            //障害物（ゴミ袋）を生成
            if (num <= 3)
            {
                GameObject cube = Instantiate(trashBagPrefab);
                cube.transform.position = new Vector3(-3, cube.transform.position.y, i);
            }
            //アイテム（shrimp）を生成
            for (int j = -1; j <= 1; j++)
            {
                //各レーンのアイテムを確率で生成
                int shrimpGenerationRate = Random.Range(1, 5);
                //50%の確立でSpeedUpを生成
                if (shrimpGenerationRate <= 2)
                {
                    //生成されるアイテムのZ軸方向のオフセット
                    int offSetZ = Random.Range(-5, 6);
                    GameObject sppedUp = Instantiate(redShrimpPrefab);
                    sppedUp.transform.position = new Vector3(posRangeX * j, 1, i + offSetZ);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーが背景の奥行の分だけ前進したら、一番後ろの背景を最も前方に移動させる
        if (m_lastTimeMovedBackgroundPositionZ + m_depth < m_player.transform.position.z)
        {
            m_backgroundObjects[index].transform.position += Vector3.forward * m_depth * m_count;
            m_lastTimeMovedBackgroundPositionZ = m_player.transform.position.z;
            index = (index + 1) % m_count;
        }
        //プレイヤーがアイテムの生成間隔分だけ前進したら、アイテムを生成する(ゴール以降には生成しない)
        if (lastTimeGenerateItemPositionZ + interval < m_player.transform.position.z && m_player.transform.position.z + itemGenerateRange < goal.transform.position.z)
        {
            int num = Random.Range(1, 8);//障害物の生成確立            
            int shrimpGenerationRate;//各レーンのアイテム(shrimp）の生成確率           
            int goldShrimpGenerationRate; //金のエビの生成確立           
            int srimpOffSetZ;//生成されるアイテム(shrimp)のZ軸方向のオフセット           
            int trashBagOffSetX;//障害物のx軸方向のオフセット
            //道程の4割地点でfishinBoatを生成
            if (goal.transform.position.z * 0.39f <= m_player.transform.position.z + itemGenerateRange && m_player.transform.position.z + itemGenerateRange <= goal.transform.position.z * 0.41f && !isBoat)
            {
                GameObject boat = Instantiate(fishinBoatPrefab);
                boat.transform.position = new Vector3(-5f, 0, m_player.transform.position.z + 100f);
                isBoat = true;
            }
            //ゴール地点にfishinBoatを生成
            if (goal.transform.position.z * 0.79f <= m_player.transform.position.z + itemGenerateRange && m_player.transform.position.z + itemGenerateRange <= goal.transform.position.z * 0.81f && isBoat)
            {
                GameObject boat = Instantiate(GoalBoatPrefab);
                boat.transform.position = new Vector3(boat.transform.position.x, boat.transform.position.y, goal.transform.position.z);
                isBoat = false;
            }
            //コースの80%までのアイテム生成
            if (m_player.transform.position.z + itemGenerateRange <= goal.transform.position.z * 0.8)
            {
                //障害物を生成
                if (num <= 3)
                {
                    trashBagOffSetX = Random.Range(-4, 4);
                    GameObject trashBag = Instantiate(trashBagPrefab);
                    trashBag.transform.position = new Vector3(trashBagOffSetX, trashBag.transform.position.y, m_player.transform.position.z + itemGenerateRange);
                }
                //アイテムを生成
                for (int j = -1; j <= 1; j++)
                {
                    shrimpGenerationRate = Random.Range(1, 5);
                    srimpOffSetZ = Random.Range(-5, 6);
                    //確立で各レーンのアイテムを生成
                    if (shrimpGenerationRate <= 3)
                    {
                        //金のエビの生成
                        goldShrimpGenerationRate = Random.Range(1, 6);
                        if (goldShrimpGenerationRate <= 2)
                        {
                            GameObject goldSrimp = Instantiate(goldShrimpPrefab);
                            goldSrimp.transform.position = new Vector3(posRangeX * j, 1, m_player.transform.position.z + itemGenerateRange + srimpOffSetZ);
                        }
                        //赤いエビ生成
                        else if (2 < goldShrimpGenerationRate)
                        {
                            GameObject redShrimp = Instantiate(redShrimpPrefab);
                            redShrimp.transform.position = new Vector3(posRangeX * j, 1, m_player.transform.position.z + itemGenerateRange + srimpOffSetZ);
                        }
                    }
                }
            }
            //コースの80%～ゴールまでのアイテム生成
            else if (goal.transform.position.z * 0.8 < m_player.transform.position.z + itemGenerateRange && !isShrimp)
            {
                GameObject finalShrimp = Instantiate(finalShrimpPrefab);
                finalShrimp.transform.position = new Vector3(0, 3f, goal.transform.position.z * 0.9f);
                isShrimp = true;
            }
            lastTimeGenerateItemPositionZ = m_player.transform.position.z;//最後にアイテムを生成した地点を更新
        }
    }
}
