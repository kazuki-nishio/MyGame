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
    //プレイヤーの前方何mまでアイテムを生成するか
    public float itemGenerateRange = 80f;
    //アイテムの生成間隔
    public float interval = 20f;
    //CubePrefabを入れる
    public GameObject cubePrefab_Low;
    public GameObject cubePrefab_Middle;
    public GameObject cubePrefab_High;
    //SpeedUpPrefabを入れる
    public GameObject speedUpPrefab;
    //Goalオブジェクトを入れる
    public GameObject goal;
    /// <summary>プレイヤーのオブジェクト</summary>
    GameObject m_player = null;
    /// <summary>背景のオブジェクト（複製して複数になる）を入れておく配列</summary>
    GameObject[] m_backgroundObjects;
    /// <summary>m_backgroundObjects 配列に指定する添え字</summary>
    int index = 0;
    /// <summary>最後に背景を移動させた時のプレイヤーの位置のZ座標</summary>
    float m_lastTimeMovedBackgroundPositionZ;
    //アイテムの生成範囲
    private float posRangeX = 3.4f;
    //最後にアイテムを生成した時のプレイヤーの位置のZ座標
    float lastTimeGenerateItemPositionZ;

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
            //移動速度の異なるキューブをランダムに生成
            if (num <=3)
            {
                GameObject cube = Instantiate(cubePrefab_Low);
                cube.transform.position = new Vector3(-3, cube.transform.position.y, i);
            }
            if (4 <= num && num <= 5)
            {
                GameObject cube = Instantiate(cubePrefab_Middle);
                cube.transform.position = new Vector3(-3, cube.transform.position.y, i);
            }
            //SpeedUpを生成
            for (int j = -1; j <= 1; j++)
            {
                //各レーンのSpeedUpを確率で生成
                int speedUpGenerationRate = Random.Range(1, 5);
                //50%の確立でSpeedUpを生成
                if (speedUpGenerationRate <= 2)
                {
                    //生成されるアイテムのZ軸方向のオフセット
                    int offSetZ = Random.Range(-5, 6);
                    GameObject sppedUp = Instantiate(speedUpPrefab);
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
            //それぞれの障害物の生成確立
            int num = Random.Range(1, 8);
            //各レーンのSpeedUpの生成確率
            int speedUpGenerationRate;
            //生成されるSpeedUpのZ軸方向のオフセット
            int offSetZ;
            //コースの60%までは高速キューブを生成しない
            if (m_player.transform.position.z + itemGenerateRange < goal.transform.position.z * 0.6)
            {
                //移動速度の異なるキューブをランダムに生成
                if (num <= 3)
                {
                    GameObject cube = Instantiate(cubePrefab_Low);
                    cube.transform.position = new Vector3(-3, cube.transform.position.y, m_player.transform.position.z + itemGenerateRange);
                }
                if (4 <= num && num <= 5)
                {
                    GameObject cube = Instantiate(cubePrefab_Middle);
                    cube.transform.position = new Vector3(-3, cube.transform.position.y, m_player.transform.position.z + itemGenerateRange);
                }
                //SpeedUpを生成
                for (int j = -1; j <= 1; j++)
                {
                    speedUpGenerationRate = Random.Range(1, 5);
                    offSetZ = Random.Range(-5, 6);
                    //50%の確立でSpeedUpを生成
                    if (speedUpGenerationRate <= 2)
                    {
                        GameObject sppedUp = Instantiate(speedUpPrefab);
                        sppedUp.transform.position = new Vector3(posRangeX * j, 1, m_player.transform.position.z + itemGenerateRange + offSetZ);
                    }
                }
            }
            //コースの60%～80%まで高速の障害物を含め生成
            if (goal.transform.position.z * 0.6 <= m_player.transform.position.z + itemGenerateRange && m_player.transform.position.z + itemGenerateRange < goal.transform.position.z * 0.8)
            {
                if (num <= 2)
                {
                    GameObject cube = Instantiate(cubePrefab_Low);
                    cube.transform.position = new Vector3(-3, cube.transform.position.y, m_player.transform.position.z + itemGenerateRange);
                }
                if (3 <= num && num <= 5)
                {
                    GameObject cube = Instantiate(cubePrefab_Middle);
                    cube.transform.position = new Vector3(-3, cube.transform.position.y, m_player.transform.position.z + itemGenerateRange);
                }
                if (6 <= num && num <= 7)
                {
                    GameObject cube = Instantiate(cubePrefab_High);
                    cube.transform.position = new Vector3(-3, cube.transform.position.y, m_player.transform.position.z + itemGenerateRange);
                }
                //SpeedUpを生成
                for (int j = -1; j <= 1; j++)
                {
                    speedUpGenerationRate = Random.Range(1, 5);
                    offSetZ = Random.Range(-5, 6);
                    //50%の確立でSpeedUpを生成
                    if (speedUpGenerationRate <= 2)
                    {
                        GameObject sppedUp = Instantiate(speedUpPrefab);
                        sppedUp.transform.position = new Vector3(posRangeX * j, 1, m_player.transform.position.z + itemGenerateRange + offSetZ);
                    }
                }
            }
            //コースの80%～ゴールまではSpeedUpのみ生成
            if (goal.transform.position.z * 0.8 <= m_player.transform.position.z + itemGenerateRange)
            {
                for (int j = -1; j <= 1; j++)
                {
                        GameObject sppedUp = Instantiate(speedUpPrefab);
                        sppedUp.transform.position = new Vector3(posRangeX * j, 1, m_player.transform.position.z + itemGenerateRange);
                }
            }
            lastTimeGenerateItemPositionZ = m_player.transform.position.z;
        }
    }
}
