using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

/**
 * UIの大元たる親コントローラ
 */
public class TitleUIController : MonoBehaviour
{
    private GameObject titleView;

    void Awake()
    {
        titleView = transform.Find("Title").gameObject;
    }

    public void Show()
    {
        titleView.SetActive(true);
    }

    public void Hide()
    {
        titleView.SetActive(false);
    }
}
