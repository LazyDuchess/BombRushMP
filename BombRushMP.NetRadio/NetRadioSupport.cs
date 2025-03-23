using BepInEx;
using BepInEx.Bootstrap;
using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.NetRadio
{
    public static class NetRadioSupport
    {
        public static bool Installed { get; private set; } = false;
        public static bool Paused
        {
            get
            {
                return _paused;
            }
            set
            {
                if (_paused == value) return;
                if (value)
                {
                    NetRadioPluginInstance.enabled = false;
                    var globalRadio = NetRadioGlobalRadioField.GetValue(null);
                    NetRadioManagerRadioVolumeProperty.SetValue(globalRadio, 0f);
                }
                else
                {
                    NetRadioPluginInstance.enabled = true;
                }
                _paused = value;
            }
        }
        private static bool _paused = false;
        private static BaseUnityPlugin NetRadioPluginInstance = null;
        private static Type NetRadioType = null;
        private static Type NetRadioManagerType = null;
        private static FieldInfo NetRadioGlobalRadioField = null;
        private static PropertyInfo NetRadioManagerRadioVolumeProperty = null;

        public static void Initialize()
        {
            Installed = true;
            NetRadioPluginInstance = Chainloader.PluginInfos["goatgirl.NetRadio"].Instance;
            NetRadioType = ReflectionUtility.GetTypeByName("NetRadio.NetRadio");
            NetRadioManagerType = ReflectionUtility.GetTypeByName("NetRadio.NetRadioManager");
            NetRadioGlobalRadioField = NetRadioType.GetField("GlobalRadio");
            NetRadioManagerRadioVolumeProperty = NetRadioManagerType.GetProperty("radioVolume");
        }
    }
}
