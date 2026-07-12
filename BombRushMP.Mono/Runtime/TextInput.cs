using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if PLUGIN
using BombRushMP.PluginCommon;
using Reptile;
#endif

namespace BombRushMP.Mono.Runtime
{
    public class TextInput : MonoBehaviour
    {
        public static TextInput Instance { get; private set; }

        [SerializeField]
        private TextMeshProUGUI _label;
        [SerializeField]
        private TMP_InputField _inputField;
        [SerializeField]
        private TextMeshProUGUI _inputPlaceholder;
        [SerializeField]
        private Button _button1;
        [SerializeField]
        private Button _button2;
        [SerializeField]
        private Button _button3;

        private GameObject _window;
        private TextMeshProUGUI _button1Label;
        private TextMeshProUGUI _button2Label;
        private TextMeshProUGUI _button3Label;

        private Action<string> _button1Callback;
        private Action<string> _button2Callback;
        private Action<string> _button3Callback;

        private Func<string, bool> _validateButton1Callback;
        private Func<string, bool> _validateButton2Callback;
        private Func<string, bool> _validateButton3Callback;

        private Action<string> _enterCallback;
        private Action<string> _cancelCallback;

        private Func<string, bool> _validateEnterCallback;
        private Func<string, bool> _validateCancelCallback;

        private int _maxLength = 64;
        public bool Open = false;

        private void Awake()
        {
            Instance = this;
            _window = GetComponentInChildren<Canvas>(true).gameObject;
            _button1Label = _button1.GetComponentInChildren<TextMeshProUGUI>(true);
            _button2Label = _button2.GetComponentInChildren<TextMeshProUGUI>(true);
            _button3Label = _button3.GetComponentInChildren<TextMeshProUGUI>(true);
            _window.SetActive(false);
            _inputField.textComponent.richText = false;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void LateUpdate()
        {
            if (Open)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                var activeButton1 = true;
                var activeButton2 = true;
                var activeButton3 = true;

                if (_validateButton1Callback != null)
                {
                    activeButton1 = _validateButton1Callback(_inputField.text);
                }

                if (_validateButton2Callback != null)
                {
                    activeButton2 = _validateButton2Callback(_inputField.text);
                }

                if (_validateButton3Callback != null)
                {
                    activeButton3 = _validateButton3Callback(_inputField.text);
                }

                _button1.interactable = activeButton1;
                _button2.interactable = activeButton2;
                _button3.interactable = activeButton3;

#if PLUGIN
                SharedUtils.OverrideInput = true;
                try
                {
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                    {
                        if (_validateEnterCallback != null && _validateEnterCallback(_inputField.text))
                        {
                            Close();
                            _enterCallback(_inputField.text);
                        }
                        else if (_validateEnterCallback == null)
                        {
                            Close();
                            _enterCallback(_inputField.text);
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (_validateCancelCallback != null && _validateCancelCallback(_inputField.text))
                        {
                            Cancel();
                        }
                        else if (_validateCancelCallback == null)
                        {
                            Cancel();
                        }
                    }
                }
                finally
                {
                    SharedUtils.OverrideInput = false;
                }
#endif
            }
        }

        public void Cancel()
        {
            Close();
            if (_cancelCallback != null)
            {
                _cancelCallback(_inputField.text);
            }
        }

        private void Close()
        {
            Open = false;
#if PLUGIN
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            var gameInput = Core.Instance.GameInput;
            gameInput.DisableAllControllerMaps();
            gameInput.EnableControllerMaps(BaseModule.IN_GAME_INPUT_MAPS);
#endif
            _window.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void ShowOkCancel(Action<string> okCallback, Action<string> cancelCallback, Func<string,bool> validateCallback, string label, int maxLength = 64, string placeholder = "", string defaultText = "")
        {
            Show(okCallback, cancelCallback, null, validateCallback, (input) => { return true; }, null, validateCallback, okCallback, (input) => { return true; }, null, true, true, false, maxLength, label, placeholder, defaultText, "OK", "Cancel");
        }

        public void Show(Action<string> button1Callback, Action<string> button2Callback, Action<string> button3Callback, Func<string, bool> validateButton1Callback, Func<string, bool> validateButton2Callback, Func<string, bool> validateButton3Callback, Func<string, bool> validateEnterCallback, Action<string> enterCallback, Func<string, bool> validateCancelCallback, Action<string> cancelCallback, bool showButton1, bool showButton2, bool showButton3, int maxLength = 64, string label = "Text Input", string placeholder = "", string defaultText = "", string button1Label = "Yes", string button2Label = "No", string button3Label = "Cancel")
        {
            Open = true;
#if PLUGIN
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            var gameInput = Core.Instance.GameInput;
            gameInput.DisableAllControllerMaps();
#endif
            _window.SetActive(true);

            _button1Callback = button1Callback;
            _button2Callback = button2Callback;
            _button3Callback = button3Callback;

            _validateButton1Callback = validateButton1Callback;
            _validateButton2Callback = validateButton2Callback;
            _validateButton3Callback = validateButton3Callback;

            _button1.gameObject.SetActive(showButton1);
            _button2.gameObject.SetActive(showButton2);
            _button3.gameObject.SetActive(showButton3);

            _maxLength = maxLength;
            _inputField.characterLimit = maxLength;
            _label.text = label;
            _inputPlaceholder.text = placeholder;
            _inputField.text = defaultText;

            _button1Label.text = button1Label;
            _button2Label.text = button2Label;
            _button3Label.text = button3Label;

            _button1.onClick.RemoveAllListeners();
            _button2.onClick.RemoveAllListeners();
            _button3.onClick.RemoveAllListeners();

            _button1.onClick.AddListener(() =>
            {
                Close();
                if (_button1Callback != null)
                {
                    _button1Callback(_inputField.text);
                }
            });

            _button2.onClick.AddListener(() =>
            {
                Close();
                if (_button2Callback != null)
                {
                    _button2Callback(_inputField.text);
                }
            });

            _button3.onClick.AddListener(() =>
            {
                Close();
                if (_button3Callback != null)
                {
                    _button3Callback(_inputField.text);
                }
            });

            _validateEnterCallback = validateEnterCallback;
            _validateCancelCallback = validateCancelCallback;
            _enterCallback = enterCallback;
            _cancelCallback = cancelCallback;

            _inputField.Select();
        }
    }
}
