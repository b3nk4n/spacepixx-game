using System;
using Microsoft.Devices;

namespace SpacepiXX
{
    public static class VibrationManager
    {
        private static SettingsManager settings = SettingsManager.GetInstance();

        public static void Vibrate(float seconds)
        {
            if (settings.GetVabrationValue())
                VibrateController.Default.Start(TimeSpan.FromSeconds(seconds));
        }
    }
}
