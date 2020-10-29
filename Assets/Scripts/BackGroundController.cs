using UnityEngine;

/// <summary>
/// プレイヤーの進行状況に応じて背景となるオブジェクトを奥へ奥へと移動させる。
/// </summary>
public class BackGroundController : MonoBehaviour
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
    

    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
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
    }

    void Update()
    {
        // プレイヤーが背景の奥行の分だけ前進したら、一番後ろの背景を最も前方に移動させる
        if (m_lastTimeMovedBackgroundPositionZ + m_depth < m_player.transform.position.z)
        {
            m_backgroundObjects[index].transform.position += Vector3.forward * m_depth * m_count;
            m_lastTimeMovedBackgroundPositionZ = m_player.transform.position.z;
            index = (index + 1) % m_count;
        }
    }
}
