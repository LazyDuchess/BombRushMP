﻿using Reptile;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BombRushMP.Common;
using BombRushMP.Common.Packets;

namespace BombRushMP.Plugin
{
    public class ChatUI : MonoBehaviour
    {
        public static ChatUI Instance { get; private set; }
        private const int MaxMessages = 500;
        private KeyCode _chatKey = KeyCode.Tab;
        private Button _sendButton;
        private TMP_InputField _inputField;
        private ScrollRect _scrollRect;
        private Image _scrollRectImage;
        private Image[] _scrollBarImages;
        private GameObject _chatWindow;
        private TextMeshProUGUI _referenceText;
        private List<TextMeshProUGUI> _messages = new();

        private const float TimeForMessagesToHide = 30f;
        private bool _messagesHidden = false;
        private float _messageHideTimer = 0f;

        public enum States
        {
            None,
            Unfocused,
            Focused
        }
        public States State { get; private set; } = States.None;

        private void Awake()
        {
            Instance = this;
            _chatWindow = transform.Find("Canvas").Find("Chat Window").gameObject;
            _sendButton = _chatWindow.transform.Find("Send Button").GetComponent<Button>();
            _inputField = _chatWindow.transform.Find("Input Field").GetComponent<TMP_InputField>();
            _scrollRect = _chatWindow.transform.Find("Scroll View").GetComponent<ScrollRect>();
            _scrollBarImages = _scrollRect.transform.Find("Scrollbar Vertical").GetComponentsInChildren<Image>(true);
            _scrollRectImage = _scrollRect.GetComponent<Image>();
            _referenceText = _scrollRect.content.Find("Text").GetComponent<TextMeshProUGUI>();
            _referenceText.gameObject.SetActive(false);
            _sendButton.onClick.AddListener(TrySendChatMessage);
            var clientController = ClientController.Instance;
            clientController.PacketReceived += OnPacketReceived;
            SetState(States.Unfocused);
        }

        private void OnPacketReceived(Packets packetId, Packet packet)
        {
            var clientController = ClientController.Instance;
            var lobbyManager = clientController.ClientLobbyManager;
            if (packetId == Packets.ServerChat)
            {
                HandleServerMessage((ServerChat)packet);
            }
        }

        private void HandleServerMessage(ServerChat serverMessage)
        {
            var mpSettings = MPSettings.Instance;
            var text = serverMessage.Message;
            if (serverMessage.MessageType == ChatMessageTypes.PlayerJoinedOrLeft && mpSettings.LeaveJoinMessages == false)
                return;
            if (serverMessage.MessageType == ChatMessageTypes.Chat)
                text = string.Format(ClientConstants.ChatMessage, TMPFilter.CloseAllTags(serverMessage.Author), serverMessage.Message);
            AddMessage(text);
        }

        public void AddMessage(string text)
        {
            var scrollToBottom = true;
            if (State == States.Focused && _scrollRect.normalizedPosition.y > 0.1f)
                scrollToBottom = false;
            ShowMessages();
            if (_messages.Count >= MaxMessages)
            {
                var oldestMessage = _messages[0];
                Destroy(oldestMessage.gameObject);
                _messages.RemoveAt(0);
            }
            var newText = Instantiate(_referenceText.gameObject);
            newText.SetActive(true);
            newText.transform.SetParent(_scrollRect.content, false);
            var label = newText.GetComponent<TextMeshProUGUI>();
            label.text = text;
            _messages.Add(label);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.RectTransform());
            if (scrollToBottom)
                _scrollRect.normalizedPosition = new Vector2(0, 0);
        }

        private bool CanOpenChat()
        {
            var gameInput = Core.Instance.gameInput;
            var enabledMaps = gameInput.GetAllCurrentEnabledControllerMapCategoryIDs(0);
            return enabledMaps.controllerMapCategoryIDs.Contains(0) && enabledMaps.controllerMapCategoryIDs.Contains(6) && !Core.Instance.IsCorePaused;
        }

        public void TrySendChatMessage()
        {
            var message = _inputField.text;
            _inputField.text = "";
            SetState(States.Unfocused);
            if (!TMPFilter.IsValidChatMessage(message)) return;
            var clientController = ClientController.Instance;
            clientController.SendPacket(new ClientChat(message), MessageSendMode.Reliable);
        }

        private void LateUpdate()
        {
            if (ShouldDisplay())
                _chatWindow.SetActive(true);
            else
            {
                SetState(States.Unfocused);
                _chatWindow.SetActive(false);
                return;
            }

            switch (State)
            {
                case States.Unfocused:
                    UnfocusedUpdate();
                    break;
                case States.Focused:
                    FocusedUpdate();
                    break;
            }
        }

        private void HideMessages()
        {
            _messageHideTimer = TimeForMessagesToHide;
            _messagesHidden = true;
            _scrollRect.gameObject.SetActive(false);
        }

        private void ShowMessages()
        {
            _messageHideTimer = 0f;
            _messagesHidden = false;
            _scrollRect.gameObject.SetActive(true);
        }

        private void UnfocusedUpdate()
        {
            _scrollRect.normalizedPosition = new Vector2(0, 0);
            _messageHideTimer += Time.deltaTime;
            if (_messageHideTimer >= TimeForMessagesToHide)
                HideMessages();

            if (Input.GetKeyDown(_chatKey) && CanOpenChat())
                SetState(States.Focused);
        }

        private void FocusedUpdate()
        {
            InputUtils.Override = true;
            try
            {
                _messageHideTimer = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
                {
                    if (!RectTransformUtility.RectangleContainsScreenPoint(_chatWindow.RectTransform(), Input.mousePosition))
                    {
                        SetState(States.Unfocused);
                    }
                }
                if (Input.GetKeyDown(KeyCode.End))
                    _scrollRect.normalizedPosition = new Vector2(0, 0);
                if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) && EventSystem.current.currentSelectedGameObject == _inputField.gameObject)
                    TrySendChatMessage();
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(_chatKey))
                    SetState(States.Unfocused);
            }
            finally
            {
                InputUtils.Override = false;
            }
        }

        private void SetState(States newState)
        {
            if (State == newState) return;
            State = newState;
            switch (State)
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
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            var gameInput = Core.Instance.GameInput;
            gameInput.DisableAllControllerMaps();
            gameInput.EnableControllerMaps(BaseModule.IN_GAME_INPUT_MAPS);
            _scrollRectImage.enabled = false;
            _inputField.gameObject.SetActive(false);
            _sendButton.gameObject.SetActive(false);
            foreach (var image in _scrollBarImages)
                image.enabled = false;
            EventSystem.current.SetSelectedGameObject(null);
        }

        private void EnterFocusedState()
        {
            var gameInput = Core.Instance.GameInput;
            gameInput.DisableAllControllerMaps();
            ShowMessages();
            _scrollRectImage.enabled = true;
            _inputField.gameObject.SetActive(true);
            _sendButton.gameObject.SetActive(true);
            foreach (var image in _scrollBarImages)
                image.enabled = true;
            _inputField.Select();
        }

        private bool ShouldDisplay()
        {
            var uiManager = Core.Instance.UIManager;
            if (uiManager == null || uiManager.gameplay == null || uiManager.gameplay.gameplayScreen == null) return false;
            if (!uiManager.gameplay.gameplayScreen.gameObject.activeInHierarchy)
            {
                if (SpectatorController.Instance == null)
                    return false;
            }
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
