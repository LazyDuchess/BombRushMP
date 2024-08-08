using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Phone
{
    public static class PhoneUtility
    {
        public static void BackToHomescreen(Reptile.Phone.Phone phone)
        {
            while (phone.m_PreviousApps.Count > 0)
            {
                phone.DisableApp(phone.m_CurrentApp.name);
                phone.EnableApp(phone.m_PreviousApps.Pop().name);
            }
            phone.audioManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Back, 0f);
        }
    }
}
