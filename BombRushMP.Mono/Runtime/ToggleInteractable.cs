using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if PLUGIN
using CommonAPI;
using Reptile;
#endif
using UnityEngine;

namespace BombRushMP.Mono
{
#if PLUGIN
    public class ToggleInteractable : CustomInteractable
#else
    public class ToggleInteractable : MonoBehaviour
#endif
    {
        public AudioSource ToggleOnSFX = null;
        public AudioSource ToggleOffSFX = null;
        public GameObject ObjectToToggleOn = null;
        public GameObject ObjectToToggleOff = null;
        public bool StartsOn = false;
        private bool _isOn = false;

#if PLUGIN
        private void Awake()
        {
            PlacePlayerAtSnapPosition = false;
            _isOn = StartsOn;
            UpdateState();
        }

        public override void Interact(Player player)
        {
            _isOn = !_isOn;
            if (_isOn)
            {
                if (ToggleOnSFX != null)
                    ToggleOnSFX.Play();
            }
            else
            {
                if (ToggleOffSFX != null)
                    ToggleOffSFX.Play();
            }
            UpdateState();
        }

        private void UpdateState()
        {
            if (_isOn)
            {
                ObjectToToggleOn.gameObject.SetActive(true);
                ObjectToToggleOff.gameObject.SetActive(false);
            }
            else
            {
                ObjectToToggleOn.gameObject.SetActive(false);
                ObjectToToggleOff.gameObject.SetActive(true);
            }
        }
#endif
    }
}
