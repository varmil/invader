using UnityEngine;

/// <summary>
/// MonoBehaviourを継承したシングルトン
/// </summary>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static volatile T instance;

    /// <summary>
    /// 同期オブジェクト
    /// </summary>
    private static object syncObj = new object();

    public static T Instance
    {
        get
        {
            // アプリ終了時に，再度インスタンスの呼び出しがある場合に，オブジェクトを生成することを防ぐ
            if (applicationIsQuitting)
            {
                return null;
            }

            // インスタンスがない場合に探す
            if (instance == null)
            {
                instance = FindObjectOfType<T>() as T;

                if (FindObjectsOfType<T>().Length > 1)
                {
                    throw new System.Exception(typeof(T).ToString() + " exist multiple instances.");
                }

                if (instance == null)
                {
                    // 同時にインスタンス生成を呼ばないためにlockする
                    lock (syncObj)
                    {
                        // シングルトンオブジェクトだと分かりやすいように名前を設定
                        instance = new GameObject("Managers/" + typeof(T).ToString() + " (singleton)").AddComponent<T>();
                    }
                }

            }
            return instance;
        }

        // インスタンスをnull化するときに使うのでprivateに
        private set
        {
            instance = value;
        }
    }

    /// <summary>
    /// アプリが終了しているかどうか
    /// </summary>
    static bool applicationIsQuitting = false;

    void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    void OnDestroy()
    {
        Instance = null;
    }

    // コンストラクタをprotectedにすることでインスタンスを生成出来なくする
    protected SingletonMonoBehaviour() { }
}