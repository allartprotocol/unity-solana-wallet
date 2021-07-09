using System.Collections.Generic;
using UnityEngine;

public class SimpleScreenManager : MonoBehaviour
{
    public Screen[] screens;
    private Dictionary<string, Screen> screensDict = new Dictionary<string, Screen>();

    private void Start()
    {
        PopulateDictionary();
    }

    private void PopulateDictionary()
    {
        if (screens != null && screens.Length > 0)
        {
            foreach (Screen screen in screens)
            {
                SetupScreen(screen);
            }
            screens[0].gameObject.SetActive(true);
        }
    }

    private void SetupScreen(Screen screen)
    {
        screen.gameObject.SetActive(false);
        screensDict.Add(screen.gameObject.name, screen);
        screen.manager = this;
    }

    public void ShowScreen(Screen curScreen, Screen screen)
    {
        curScreen.HideScreen();
        screen.ShowScreen();
    }

    public void ShowScreen(Screen curScreen, int index)
    {
        curScreen.HideScreen();
        screens[index].ShowScreen();
    }

    public void ShowScreen(Screen curScreen, string name, object data = null)
    {
        curScreen.HideScreen();
        screensDict[name].ShowScreen(data);
    }
}
