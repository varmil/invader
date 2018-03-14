using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleState : MonoBehaviour, IAppState
{
    [SerializeField]
    private UIController uiController;

    // pressed SPACE key ?
    private bool pressed = false;

    public void OnEnter()
    {
        uiController.ShowTitle();
    }
    
    public void Tick()
    {
        if (!pressed && Input.GetKeyDown(KeyCode.Space))
        {
            pressed = true;

            // go to game scene
            GameProcessManager.Instance.SetState(GetComponent<InGameState>());
        }
    }
    
    public void OnLeave()
    {
        uiController.HideTitle();
    }
}
