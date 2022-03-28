using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Collections;
using SFB;
using System.Collections.Generic;
using System;

namespace AllArt.Solana.Example
{
    [RequireComponent(typeof(TxtLoader))]
    public class LoginScreen : SimpleScreen
    {
        public TMP_InputField _passwordInputField;
        public TextMeshProUGUI _passwordText;
        public Button _createNewWalletBtn;
        public Button _loginToWalletBtn;
        public Button _loginBtn;
        public Button _tryAgainBtn;
        public Button _backBtn;
        public TextMeshProUGUI _messageTxt;

        public List<GameObject> _panels = new List<GameObject>();
        public SimpleScreenManager parentManager;

        private string _password;
        private string _mnemonics;
        private string _path;
        private SimpleWallet _simpleWallet;
        private Cypher _cypher;
        private string _pubKey;
        private string[] _paths;
        private string _loadedKey;

        private void OnEnable()
        {
            _passwordInputField.text = String.Empty;
        }

        private void Start()
        {
            _cypher = new Cypher();
            _simpleWallet = SimpleWallet.instance;
            //_mnemonics = "gym basket dizzy chest pact rubber canvas staff around shadow brain purchase hello parent digital degree window version still rather measure brass lock arrest";
            _passwordText.text = "";

            SwitchButtons("Login");

            if(_loginToWalletBtn != null)
            {
                _loginToWalletBtn.onClick.AddListener(() =>
                {
                    SwitchPanels(1);
                });
            }
 
            if(_backBtn != null)
            {
                _backBtn.onClick.AddListener(() =>
                {
                    SwitchPanels(0);
                });
            }

            if(_createNewWalletBtn != null)
            {
                _createNewWalletBtn.onClick.AddListener(() =>
                {
                    SimpleWallet.instance.DeleteWalletAndClearKey();
                    manager.ShowScreen(this, "generate_screen");
                    SwitchPanels(0);
                });
            }

            _passwordInputField.onSubmit.AddListener(delegate { LoginChecker(); });

            _loginBtn.onClick.AddListener(LoginChecker);
            _tryAgainBtn.onClick.AddListener(() => { SwitchButtons("Login"); });  

            if(_messageTxt != null)
                _messageTxt.gameObject.SetActive(false);
        }

        private void LoginChecker()
        {
            if (_simpleWallet.LoginCheckMnemonicAndPassword(_passwordInputField.text))
            {
                SimpleWallet.instance.GenerateWalletWithMenmonic(_simpleWallet.LoadPlayerPrefs(_simpleWallet.MnemonicsKey));
                MainThreadDispatcher.Instance().Enqueue(() => { _simpleWallet.StartWebSocketConnection(); }); 
                manager.ShowScreen(this, "wallet_screen");
                this.gameObject.SetActive(false);
            }
            else
            {
                SwitchButtons("TryAgain");
            }
        }

        private void SwitchButtons(string btnName)
        {
            _loginBtn.gameObject.SetActive(false);
            _tryAgainBtn.gameObject.SetActive(false);

            switch (btnName)
            {
                case "Login":
                    _loginBtn.gameObject.SetActive(true);
                    _passwordInputField.gameObject.SetActive(true);
                    return;
                case "TryAgain":
                    _tryAgainBtn.gameObject.SetActive(true);
                    _passwordInputField.text = string.Empty;
                    _passwordInputField.gameObject.SetActive(false);
                    return;
            }
        }

        private void SwitchPanels(int order)
        {
            _passwordInputField.text = String.Empty;

            foreach (GameObject panel in _panels)
            {
                if (panel.transform.GetSiblingIndex() == order)
                    panel.SetActive(true);
                else
                    panel.SetActive(false);
            }
        }

        //
        // WebGL
        //
        [DllImport("__Internal")]
        private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

        // Called from browser
        public void OnFileUpload(string url)
        {
            StartCoroutine(OutputRoutine(url));
        }
        private IEnumerator OutputRoutine(string url)
        {
            var loader = new WWW(url);
            yield return loader;
            _loadedKey = loader.text;

            //LoginWithPrivateKeyCallback();
        }
    }
}

