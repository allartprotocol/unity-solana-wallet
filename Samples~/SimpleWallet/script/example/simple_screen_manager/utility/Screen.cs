using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen : MonoBehaviour, IScreen
{    
    public SimpleScreenManager manager { get; set; }

    public virtual void HideScreen()
    {
        gameObject.SetActive(false);
    }

    public virtual void ShowScreen(object data = null)
    {
        gameObject.SetActive(true);
    }
}
