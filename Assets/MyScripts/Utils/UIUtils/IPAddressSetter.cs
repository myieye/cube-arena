using System;
using System.IO;
using HoloToolkit.UI.Keyboard;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using Settings_CA = CubeArena.Assets.MyScripts.Utils.Settings.Settings;

namespace CubeArena.Assets.MyScripts.Utils.UIUtils {
    public class IPAddressSetter : MonoBehaviour {

        public static IPAddressSetter Instance { get; private set; }

        private string ipPath;
        private bool savedCursorActiveness;

        private void Awake () {
            Instance = this;
            ipPath = Path.Combine (Application.persistentDataPath, "server-ip.txt");

            if (File.Exists (ipPath)) {
                var ipAddress = File.ReadAllText (ipPath).Trim ();
                if (!string.IsNullOrEmpty (ipAddress)) {
                    Settings_CA.Instance.ServerIp = ipAddress;
                }
            }
        }

        public void OpenIPAddressSetter () {
            PresentIPKeyboard ();
        }

        private void PresentIPKeyboard () {
            var ipAddress = Settings_CA.Instance.ServerIp;
            savedCursorActiveness = !SimpleSinglePointerSelector.Instance.Cursor.gameObject.activeSelf;
            SimpleSinglePointerSelector.Instance.Cursor.gameObject.SetActive (true);

            Keyboard.Instance.Close ();
            Keyboard.Instance.PresentKeyboard (ipAddress, Keyboard.LayoutType.Alpha);

            Keyboard.Instance.RepositionKeyboard (transform, null, 0.045f);

            Keyboard.Instance.SubmitOnEnter = true;
            Keyboard.Instance.OnTextSubmitted += OnTextSubmitted;
        }

        private void OnTextSubmitted (object sender, EventArgs e) {
            SimpleSinglePointerSelector.Instance.Cursor.gameObject.SetActive (savedCursorActiveness);

            var newIpAddress = Keyboard.Instance.InputField.text;
            Debug.Log ("OnTextSubmitted: " + newIpAddress);
            Settings_CA.Instance.ServerIp = newIpAddress;
            SaveIpAddressToFile ();
        }

        private void SaveIpAddressToFile () {
            using (TextWriter writer = File.CreateText (ipPath)) {
                writer.Write (Settings_CA.Instance.ServerIp);
            }
        }
    }
}