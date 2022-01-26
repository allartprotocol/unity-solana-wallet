using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace AllArt.Solana.Example
{
    public class TxtLoader : MonoBehaviour
    {
        private string _loadedTxt;
        private string _savedTxt;
        private string _path;

        public Action<string> TxtLoadedAction;
        /// <summary>
        /// First param is path
        /// Second param is Text to save
        /// Third param is title of the txt file
        /// </summary>>
        public Action<string, string, string> TxtSavedAction;
        public Action<string, byte[], string> ByteArraySavedAction;
        public void LoadTxt()
        {
            try
            {
                _path = string.Empty;

#if UNITY_WEBGL && !UNITY_EDITOR
                UploadFile(gameObject.name, "OnFileUpload", ".txt", false);
#elif UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_STANDALONE
                string[] paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "txt", false);
                if (paths.Length == 0) return;
                _path = paths[0];
                _loadedTxt = File.ReadAllText(_path);
#elif UNITY_ANDROID || UNITY_IPHONE
                string txt;
                txt = NativeFilePicker.ConvertExtensionToFileType("txt");
                NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
		            {
			            if (path == null)
				            Debug.Log("Operation cancelled");
			            else
			            {
                            _loadedTxt = File.ReadAllText(path);
                        }
		            }, new string[] { txt });
		        Debug.Log("Permission result: " + permission);
#endif
#if UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IPHONE
                TxtLoadedAction?.Invoke(_loadedTxt);
#endif
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        public void SaveTxt(string fileTitle, string txtToSave, bool instantSave)
        {
            _path = string.Empty;
#if UNITY_WEBGL && !UNITY_EDITOR
                if(instantSave)
                {
                    var bytes = Encoding.UTF8.GetBytes(txtToSave);
                    DownloadFile(gameObject.name, "OnFileDownload", fileTitle + ".txt", bytes, bytes.Length);
                }
                else
                    TxtSavedAction?.Invoke(_path, txtToSave, fileTitle);
                
#elif UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_STANDALONE

            string[] paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "txt", false);
            if (paths.Length == 0) return;
            _path = paths[0];

#elif UNITY_ANDROID || UNITY_IPHONE
                string fileType = NativeFilePicker.ConvertExtensionToFileType("txt");
                NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
                {
                    if (path == null)
                        Debug.Log("Operation cancelled");
                    else
                    {
                        _path = path;
                    }
                }, new string[] { fileType });
#endif
            if (instantSave)
                File.WriteAllText(_path, txtToSave);
            else
                TxtSavedAction?.Invoke(_path, txtToSave, fileTitle);
        }

        public void SaveByteArray(string fileTitle, byte[] array, bool instantSave)
        {
            _path = string.Empty;
#if UNITY_WEBGL && !UNITY_EDITOR
                if(instantSave)
                {
                    var bytes = array;
                    DownloadFile(gameObject.name, "OnFileDownload", fileTitle + ".txt", bytes, bytes.Length);
                }
                else
                    ByteArraySavedAction?.Invoke(_path, array, fileTitle);
                
#elif UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_STANDALONE

            string[] paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "txt", false);
            if (paths.Length == 0) return;
            _path = paths[0];

#elif UNITY_ANDROID || UNITY_IPHONE
                string fileType = NativeFilePicker.ConvertExtensionToFileType("txt");
                NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
                {
                    if (path == null)
                        Debug.Log("Operation cancelled");
                    else
                    {
                        _path = path;
                    }
                }, new string[] { fileType });
#endif
            if (instantSave)
                File.WriteAllBytes(_path, array);
            else
                ByteArraySavedAction?.Invoke(_path, array, fileTitle);
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

            string loadedTxt = loader.text;

            MainThreadDispatcher.Instance().Enqueue(() => { TxtLoadedAction?.Invoke(loadedTxt); });           
        }

        [DllImport("__Internal")]
        public static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

        // Called from browser
        public void OnFileDownload()
        {

        }
    }
}