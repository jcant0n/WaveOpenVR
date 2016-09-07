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
        SteamVRService steamVR;

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

            steamVR = WaveServices.GetService<SteamVRService>();
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (steamVR.RightHand != null && steamVR.RightHand.Connected)
            {
                Labels.Add("Right_Status", steamVR.RightHand.Connected);
                Labels.Add("Right_Position", steamVR.RightHand.WorldTransform.Translation);
                Labels.Add("Right_Orientation", steamVR.RightHand.WorldTransform.Orientation);
            }

            if (steamVR.LeftHand != null && steamVR.LeftHand.Connected)
            {
                Labels.Add("Left_Status", steamVR.LeftHand.Connected);
                Labels.Add("Left_Position", steamVR.LeftHand.WorldTransform.Translation);
                Labels.Add("Left_Orientation", steamVR.LeftHand.WorldTransform.Orientation);
            }
        }
    }
}
