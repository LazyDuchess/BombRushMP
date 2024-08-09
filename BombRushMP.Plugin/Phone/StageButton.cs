using CommonAPI.Phone;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin.Phone
{
    public class StageButton : MonoBehaviour
    {
        private SimplePhoneButton _simplePhoneButton;
        public string StageName = "";
        public Stage Stage;
        private void Awake()
        {
            _simplePhoneButton = GetComponent<SimplePhoneButton>();
        }

        private void Update()
        {
            var clientController = ClientController.Instance;
            if (clientController != null)
                _simplePhoneButton.Label.text = $"({clientController.GetPlayerCountForStage(Stage)}) {StageName}";
        }
    }
}
