using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AppState : MonoBehaviour, IAppState
{
    public virtual string SceneName
    {
        get;
    }

    /// <summary>
    /// load <SceneName>, and that is set to active scene
    /// </summary>
    public virtual IEnumerator OnEnter()
    {
        // HACK: performance
        var ao = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
        while (!ao.isDone)
        {
            yield return null;
        }
        var scene = SceneManager.GetSceneByName(SceneName);
        SceneManager.SetActiveScene(scene);

        Debug.Log("[AppState] OnEnter END");
    }

    /// <summary>
    /// called every frame
    /// </summary>
    public virtual void Tick()
    {
    }

    /// <summary>
    /// unload <SceneName> with releasing resources
    /// </summary>
    public virtual IEnumerator OnLeave()
    {
        StopAllCoroutines();

        var ao = SceneManager.UnloadSceneAsync(SceneName);
        while (!ao.isDone)
        {
            yield return null;
        }
    }

    /// <summary>
    /// called when fader.FadeOut() animation is done
    /// this is useful when you want to start the scene init process after fadeout
    /// </summary>
    public virtual IEnumerator OnFadeOutEnd()
    {
        yield return null;
    }

    protected IEnumerable<GameObject> GetRootObjects()
    {
        return SceneManager.GetActiveScene().GetRootGameObjects();
    }
}
