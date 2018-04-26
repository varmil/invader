using System.Collections;
using System.Linq;
using UnityEngine;

/**
 * ゲームの進行管理
 */
public class GameProcessManager : SingletonMonoBehaviour<GameProcessManager>
{
    public GlobalStore GlobalStore
    {
        get;
        private set;
    }

    [SerializeField]
    private Fade fader = null;

    private AppState currentState;

    void Awake()
    {
        this.GlobalStore = new GlobalStore();
    }

    void Start()
    {
        SetState(GetComponent<TitleState>());
    }

    private void Update()
    {
        currentState.Tick();
    }

    public void SetState(AppState state)
    {
        StartCoroutine(SetStateCoroutine(state));
    }

    private IEnumerator SetStateCoroutine(AppState state)
    {
        if (currentState != null)
        {
            yield return fader.FadeIn(1.0f);
            yield return StartCoroutine(currentState.OnLeave());
        }

        currentState = state;

        if (currentState != null)
        {
            yield return StartCoroutine(currentState.OnEnter());
            yield return fader.FadeOut(1.0f);
            yield return StartCoroutine(currentState.OnFadeOutEnd());
        }
    }
}
