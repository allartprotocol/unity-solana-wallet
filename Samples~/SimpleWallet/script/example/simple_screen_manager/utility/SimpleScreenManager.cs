using System.Collections.Generic;
using UnityEngine;

namespace AllArt.Solana.Example
{
    public class SimpleScreenManager : MonoBehaviour
    {
        public SimpleScreen[] screens;
        private Dictionary<string, SimpleScreen> screensDict = new Dictionary<string, SimpleScreen>();

        private void Start()
        {
            PopulateDictionary();
        }

        private void PopulateDictionary()
        {
            if (screens != null && screens.Length > 0)
            {
                foreach (SimpleScreen screen in screens)
                {
                    SetupScreen(screen);
                }
                screens[0].gameObject.SetActive(true);
            }
        }

        private void SetupScreen(SimpleScreen screen)
        {
            screen.gameObject.SetActive(false);
            screensDict.Add(screen.gameObject.name, screen);
            screen.manager = this;
        }

        public void ShowScreen(SimpleScreen curScreen, SimpleScreen screen)
        {
            curScreen.HideScreen();
            screen.ShowScreen();
        }

        public void ShowScreen(SimpleScreen curScreen, int index)
        {
            curScreen.HideScreen();
            screens[index].ShowScreen();
        }

        public void ShowScreen(SimpleScreen curScreen, string name, object data = null)
        {
            curScreen.HideScreen();
            screensDict[name].ShowScreen(data);
        }
    }

}
