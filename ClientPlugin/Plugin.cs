using System;
using System.IO;
using ClientPlugin.GUI;
using HarmonyLib;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using Sandbox.ModAPI;
using Shared.Config;
using Shared.Logging;
using Shared.Patches;
using Shared.Plugin;
using VRage.FileSystem;
using VRage.Plugins;
using VRage.Utils;
using VRageMath;
using VRageRender;


namespace FixedEyedAdaptation
{
    // ReSharper disable once UnusedType.Global

    public class Plugin : IPlugin, ICommonPlugin
    {
        public const string Name = "FixedEyeAdaptation";
        public static Plugin Instance { get; private set; }

        public long Tick { get; private set; }

        public IPluginLogger Log => Logger;
        private static readonly IPluginLogger Logger = new PluginLogger(Name);

        public IPluginConfig Config => config?.Data;
        private PersistentConfig<PluginConfig> config;
        private static readonly string ConfigFileName = $"{Name}.cfg";

        private static bool initialized;
        private static bool failed;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public void Init(object gameInstance)
        {

            Instance = this;

            Log.Info("Loading");

            var configPath = Path.Combine(MyFileSystem.UserDataPath, ConfigFileName);
            config = PersistentConfig<PluginConfig>.Load(Log, configPath);

            Common.SetPlugin(this);

            if (!PatchHelpers.HarmonyPatchAll(Log, new Harmony(Name)))
            {
                failed = true;
                return;
            }

            Log.Debug("Successfully loaded");

        }
        public void UpdateEyeAdaptation()
        {
            MyPostprocessSettingsWrapper.AllEnabled = true;

            MyPostprocessSettingsWrapper.Settings.EnableEyeAdaptation = true;
            MyPostprocessSettingsWrapper.Settings.HistogramLogMin = -3;
            MyPostprocessSettingsWrapper.Settings.HistogramLogMax = 4;
            MyPostprocessSettingsWrapper.Settings.HistogramFilterMin = 65;
            MyPostprocessSettingsWrapper.Settings.HistogramFilterMax = 98;
            MyPostprocessSettingsWrapper.Settings.HistogramSkyboxFactor = 0.25f;
            MyPostprocessSettingsWrapper.Settings.MinEyeAdaptationLogBrightness = -6;
            MyPostprocessSettingsWrapper.Settings.MaxEyeAdaptationLogBrightness = 0.95f;
            MyPostprocessSettingsWrapper.Settings.EyeAdaptationPrioritizeScreenCenter = false;

            MyPostprocessSettingsWrapper.Settings.Data.Contrast = 1;
            MyPostprocessSettingsWrapper.Settings.Data.Brightness = 1.1f;
            MyPostprocessSettingsWrapper.Settings.Data.Saturation = 0.755f;

            MyPostprocessSettingsWrapper.Settings.Data.LightColor = new Vector3(1, 0.9, 0.5);
            MyPostprocessSettingsWrapper.Settings.Data.DarkColor = new Vector3(0.2, 0.05, 0);

            MyPostprocessSettingsWrapper.Settings.Data.ConstantLuminance = 0.1f;
            MyPostprocessSettingsWrapper.Settings.Data.LuminanceExposure = 0;
            MyPostprocessSettingsWrapper.Settings.Data.EyeAdaptationTau = 0.4f;

            MyPostprocessSettingsWrapper.Settings.Data.BloomExposure = 3.5f;
            MyPostprocessSettingsWrapper.Settings.Data.BloomLumaThreshold = 9;
            MyPostprocessSettingsWrapper.Settings.Data.BloomEmissiveness = 15;
            MyPostprocessSettingsWrapper.Settings.Data.BloomDepthStrength = 2;
            MyPostprocessSettingsWrapper.Settings.Data.BloomDepthSlope = 0.3f;


            MyPostprocessSettingsWrapper.Settings.Data.EyeAdaptationSpeedUp = 2;
            MyPostprocessSettingsWrapper.Settings.Data.EyeAdaptationSpeedDown = 2;
            MyPostprocessSettingsWrapper.Settings.Data.WhitePoint = 11.2f;


            var pP = MySector.PlanetProperties;
            pP.AtmosphereIntensityMultiplier = 17;
            pP.AtmosphereIntensityAmbientMultiplier = 10;
            pP.CloudsIntensityMultiplier = 17;


            var ssao = MySector.SSAOSettings.Data;
            MySector.SSAOSettings.Enabled = true;
            ssao.MinRadius = 0.115f;
            ssao.MaxRadius = 25;
            ssao.RadiusGrowZScale = 1.007f;
            ssao.Falloff = 3.08f;
            ssao.RadiusBias = 0.25f;
            ssao.Contrast = 2.617f;
            ssao.Normalization = 0.075f;
            ssao.ColorScale = 0.6f;


            MySector.SunProperties.EnvironmentProbe.DrawDistance = 0.00001f;
            MySector.SunProperties.EnvironmentLight.SunColor = new VRageMath.Vector3(100, 0, 0);
            MySector.SunProperties.EnvironmentLight.SunDiscColor = new VRageMath.Vector3(100, 0, 0);
        }

        public void Dispose()
        {
            try
            {
                // TODO: Save state and close resources here, called when the game exists (not guaranteed!)
                // IMPORTANT: Do NOT call harmony.UnpatchAll() here! It may break other plugins.
            }
            catch (Exception ex)
            {
                Log.Critical(ex, "Dispose failed");
            }

            Instance = null;
        }

        public void Update()
        {

            EnsureInitialized();
            try
            {
                if (!failed)
                {
                    CustomUpdate();
                    Tick++;
                }
            }
            catch (Exception ex)
            {
                Log.Critical(ex, "Update failed");
                failed = true;
            }
        }

        private void EnsureInitialized()
        {
            if (initialized || failed)
                return;

            Log.Info("Initializing");
            try
            {
                Initialize();
            }
            catch (Exception ex)
            {
                Log.Critical(ex, "Failed to initialize plugin");
                failed = true;
                return;
            }

            Log.Debug("Successfully initialized");
            initialized = true;
        }

        private void Initialize()
        {
            // TODO: Put your one time initialization code here. It is executed on first update, not on loading the plugin.
        }


        private void CustomUpdate()
        {
            if (Tick % 3600==0 && MyAPIGateway.Session != null && MySector.MainCamera != null) UpdateEyeAdaptation(); //OnLoad would be better
            // TODO: Put your update code here. It is called on every simulation frame!
        }


        // ReSharper disable once UnusedMember.Global
        public void OpenConfigDialog()
        {
            MyGuiSandbox.AddScreen(new MyPluginConfigDialog());
        }
    }

}