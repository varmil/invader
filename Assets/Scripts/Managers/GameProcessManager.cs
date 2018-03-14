using System.Collections;
using System.Linq;
using UnityEngine;

/**
 * ゲームの進行管理
 */
public class GameProcessManager : SingletonMonoBehaviour<GameProcessManager>
{
    [SerializeField]
    private Fade fader = null;

    private IAppState currentState;

//    void Awake()
//    {
//    }
    
    void Start()
    {
        SetState(GetComponent<TitleState>());
    }
    
    private void Update()
    {
        Debug.Log(currentState);
        currentState.Tick();
    }

    public void SetState(IAppState state)
    {
        StartCoroutine(SetStateCoroutine(state));
    }

    private IEnumerator SetStateCoroutine(IAppState state)
    {
        if (currentState != null)
        {
            yield return fader.FadeIn(1.0f);
            currentState.OnLeave();
        }
        
        currentState = state;
        
        if (currentState != null)
        {
            yield return fader.FadeOut(1.0f);
            currentState.OnEnter();
        }
    }
}
