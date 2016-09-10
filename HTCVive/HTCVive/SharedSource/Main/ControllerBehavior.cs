using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Services;
using WaveEngine.OpenVR;

namespace HTCVive
{
    [DataContract]
    public class ControllerBehavior : Behavior
    {
        [RequiredService]
        SteamVR_Service steamVR;

        public ControllerBehavior()
        {
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Update(TimeSpan gameTime)
        {
            var index = steamVR.ControllerManager.GetDeviceIndex(SteamVR_ControllerManager.DeviceRelation.First, Valve.VR.ETrackedDeviceClass.Controller);
            var controller = steamVR.ControllerManager.Input(index);

            if (controller != null && controller.Connected && controller.Valid && controller.HasTracking)
            {
                PrintStatus(controller);
            }
        }

        private void PrintStatus(SteamVR_Controller controller)
        {
            Labels.Add("Controller_Position", controller.WorldTransform.Translation);
            Labels.Add("Controller_Orientation", controller.WorldTransform.Orientation);
        }
    }
}
