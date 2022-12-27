using System;
using System.IO;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using Sandbox.ModAPI;
using VRage.FileSystem;
using VRage.Plugins;
using VRage.Utils;
using VRageMath;
using VRageRender;


namespace FixedEyedAdaptation
{
    public class Plugin : IPlugin
    {
        private static bool firstload = true;


        public void Init(object gameInstance)
        {

        }


        public void Dispose()
        {

        }

        public void Update()
        {
            if (firstload && MyAPIGateway.Session != null && MySector.MainCamera != null)
            {
                MyPostprocessSettingsWrapper.Settings.EnableEyeAdaptation = true;

                MyPostprocessSettingsWrapper.Settings.HistogramLogMin = -3;
                MyPostprocessSettingsWrapper.Settings.HistogramLogMax = 4;
                MyPostprocessSettingsWrapper.Settings.HistogramFilterMin = 65;
                MyPostprocessSettingsWrapper.Settings.HistogramFilterMax = 98;

                MyPostprocessSettingsWrapper.Settings.HistogramSkyboxFactor = 0.25f;
                MyPostprocessSettingsWrapper.Settings.MinEyeAdaptationLogBrightness = -2;
                MyPostprocessSettingsWrapper.Settings.MaxEyeAdaptationLogBrightness = 0.95f;


                MyPostprocessSettingsWrapper.Settings.EyeAdaptationPrioritizeScreenCenter = false;
                MyPostprocessSettingsWrapper.Settings.Data.EyeAdaptationTau = 0.4f;

                firstload = false;
            }
        }
    }

}