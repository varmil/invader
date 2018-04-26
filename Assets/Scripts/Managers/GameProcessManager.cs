using System.Collections;
using System.Linq;
using UnityEngine;

/**
 * ゲームの進行管理
 */
public class GameProcessManager : MonoBehaviour
{
    [SerializeField]
    private Fade fader = null;

    private AppState currentState;

    void Start()
    {
        SetState(GetComponent<TitleState>());
    }

    void Update()
    {
        currentState.Tick();
    }

    private void SetState(AppState state)
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

        // set next state
        currentState = state;

        // Dependency Inversion
        currentState.NextStateSet = (nextState) => this.SetState(nextState);

        if (currentState != null)
        {
            yield return StartCoroutine(currentState.OnEnter());
            yield return fader.FadeOut(1.0f);
            yield return StartCoroutine(currentState.OnFadeOutEnd());
        }
    }
}
