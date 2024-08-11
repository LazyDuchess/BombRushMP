using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BombRushMP.Plugin
{
    public class ChatUI : MonoBehaviour
    {
        public static ChatUI Instance { get; private set; }
        private KeyCode _chatKey = KeyCode.Tab;
        private GameObject _sendButton;
        private TMP_InputField _inputField;
        private ScrollRect _scrollRect;
        private Image _scrollRectImage;
        private Image[] _scrollBarImages;
        private GameObject _chatWindow;
        private enum States
        {
            None,
            Unfocused,
            Focused
        }
        private States _state = States.None;

        private void Awake()
        {
            Instance = this;
            _chatWindow = transform.Find("Canvas").Find("Chat Window").gameObject;
            _sendButton = _chatWindow.transform.Find("Send Button").gameObject;
            _inputField = _chatWindow.transform.Find("Input Field").GetComponent<TMP_InputField>();
            _scrollRect = _chatWindow.transform.Find("Scroll View").GetComponent<ScrollRect>();
            _scrollBarImages = _scrollRect.transform.Find("Scrollbar Vertical").GetComponentsInChildren<Image>(true);
            _scrollRectImage = _scrollRect.GetComponent<Image>();
            SetState(States.Unfocused);
        }

        private void Update()
        {
            if (ShouldDisplay())
                _chatWindow.SetActive(true);
            else
            {
                SetState(States.Unfocused);
                _chatWindow.SetActive(false);
                return;
            }
            switch (_state)
            {
                case States.Unfocused:
                    UnfocusedUpdate();
                    break;
                case States.Focused:
                    FocusedUpdate();
                    break;
            }
        }

        private void UnfocusedUpdate()
        {
            if (Input.GetKeyDown(_chatKey))
                SetState(States.Focused);
        }

        private void FocusedUpdate()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(_chatWindow.RectTransform(), Input.mousePosition))
                {
                    SetState(States.Unfocused);
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(_chatKey))
                SetState(States.Unfocused);
        }

        private void SetState(States newState)
        {
            if (_state == newState) return;
            _state = newState;
            switch (_state)
            {
                case States.Unfocused:
                    EnterUnfocusedState();
                    break;

                case States.Focused:
                    EnterFocusedState();
                    break;
            }
        }

        private void EnterUnfocusedState()
        {
            _scrollRectImage.enabled = false;
            _inputField.gameObject.SetActive(false);
            _sendButton.SetActive(false);
            foreach (var image in _scrollBarImages)
                image.enabled = false;
            EventSystem.current.SetSelectedGameObject(null);
        }

        private void EnterFocusedState()
        {
            _scrollRectImage.enabled = true;
            _inputField.gameObject.SetActive(true);
            _sendButton.SetActive(true);
            foreach (var image in _scrollBarImages)
                image.enabled = true;
            _inputField.Select();
        }

        private bool ShouldDisplay()
        {
            var uiManager = Core.Instance.UIManager;
            if (!uiManager.gameplay.gameplayScreen.gameObject.activeInHierarchy) return false;
            return true;
        }


        public static void Create()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            var mpAssets = MPAssets.Instance;
            var prefab = mpAssets.Bundle.LoadAsset<GameObject>("Chat UI");
            var chatUI = Instantiate(prefab);
            chatUI.transform.SetParent(Core.Instance.UIManager.transform, false);
            chatUI.AddComponent<ChatUI>();
        }
    }
}
