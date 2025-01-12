using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class PlayerListUI : MonoBehaviour
    {
        public static PlayerListUI Instance { get; private set; }
        public bool Displaying
        {
            get
            {
                return _window.activeSelf;
            }

            set
            {
                _window.SetActive(value);
            }
        }
        private GameObject _playerReferenceObject;
        private GameObject _window;
        
        private void Awake()
        {
            Instance = this;
            var canvas = transform.Find("Canvas");
            _window = canvas.Find("Player List Window").gameObject;
            _playerReferenceObject = _window.transform.Find("Scroll View").Find("Viewport").Find("Content").Find("Player").gameObject;
            _playerReferenceObject.SetActive(false);
            Displaying = false;
        }
    }
}
