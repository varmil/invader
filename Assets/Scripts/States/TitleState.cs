using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleState : AppState, IAppState
{
    public override string SceneName
    {
        get { return "TitleScene"; }
    }

    private TitleUIController uiController;

    // pressed SPACE key ?
    private bool pressed = false;

    public override IEnumerator OnEnter()
    {
        yield return StartCoroutine(base.OnEnter());

        var rootObjects = GetRootObjects();
        uiController = rootObjects.First(e => e.name == "UICanvas").GetComponent<TitleUIController>();

        Initialize();
    }

    public override void Tick()
    {
        base.Tick();

        if (!pressed && Input.GetKeyDown(KeyCode.Space))
        {
            pressed = true;

            // go to game scene
            GameProcessManager.Instance.SetState(GetComponent<InGameState>());
        }
    }

    public override IEnumerator OnLeave()
    {
        yield return StartCoroutine(base.OnLeave());
    }

    private void Initialize()
    {
        pressed = false;
    }
}
