using System;
using System.IO;
using HoloToolkit.UI.Keyboard;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using Settings_CA = CubeArena.Assets.MyScripts.Utils.Settings.Settings;

namespace CubeArena.Assets.MyScripts.Utils.UIUtils {
    public class IPAddressSetter : MonoBehaviour {

        [SerializeField]
        private GameObject cursor;

        public static IPAddressSetter Instance { get; private set; }

        private string ipPath;
        private bool savedCursorActiveness;

        private void Awake () {
            Instance = this;
            ipPath = Path.Combine (Application.persistentDataPath, "saved-server-ip.txt");
            var settingsIpPath = Path.Combine (Application.persistentDataPath, "settings-server-ip.txt");

            if (!File.Exists (settingsIpPath)) {
                using (var writer = File.CreateText (settingsIpPath)) {
                    writer.Write (Settings_CA.Instance.ServerIp);
                }
            }

            var prevSettingsIp = File.ReadAllText (settingsIpPath).Trim ();
            if (prevSettingsIp == Settings_CA.Instance.ServerIp) {
                if (File.Exists (ipPath)) {
                    var ipAddress = File.ReadAllText (ipPath).Trim ();
                    if (!string.IsNullOrEmpty (ipAddress)) {
                        Settings_CA.Instance.ServerIp = ipAddress;
                    }
                }
            } else {
                using (var writer = File.CreateText (settingsIpPath)) {
                    writer.Write (Settings_CA.Instance.ServerIp);
                }
            }
        }

        public void OpenIPAddressSetter () {
            PresentIPKeyboard ();
        }

        private void PresentIPKeyboard () {
            var ipAddress = Settings_CA.Instance.ServerIp;
            savedCursorActiveness = cursor.activeSelf;
            cursor.SetActive (true);

            Keyboard.Instance.gameObject.SetActive (true);

            Keyboard.Instance.Close ();
            Keyboard.Instance.PresentKeyboard (ipAddress, Keyboard.LayoutType.Alpha);

            Keyboard.Instance.RepositionKeyboard (transform, null, 0.045f);

            Keyboard.Instance.SubmitOnEnter = true;
            Keyboard.Instance.OnTextSubmitted += OnTextSubmitted;
        }

        private void OnTextSubmitted (object sender, EventArgs e) {
            var newIpAddress = Keyboard.Instance.InputField.text;
            Debug.Log ("OnTextSubmitted: " + newIpAddress);

            Settings_CA.Instance.ServerIp = newIpAddress;
            SaveIpAddressToFile ();

            cursor.SetActive (savedCursorActiveness);
            Keyboard.Instance.gameObject.SetActive (false);
        }

        private void SaveIpAddressToFile () {
            using (TextWriter writer = File.CreateText (ipPath)) {
                writer.Write (Settings_CA.Instance.ServerIp);
            }
        }
    }
}