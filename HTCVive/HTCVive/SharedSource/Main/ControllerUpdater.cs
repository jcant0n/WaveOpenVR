﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.OpenVR;

namespace HTCVive
{
    [DataContract]
    public class ControllerUpdater : Behavior
    {
        [RequiredComponent]
        public Transform3D transform;

        [RequiredService]
        public SteamVR_Service steamVR;

        public enum ControllerType
        {
            Right,
            Left,
        }
        private ControllerType type;

        public ControllerUpdater(ControllerType type)
        {
            this.type = type;
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
            //int index = steamVR.ControllerManager.GetDeviceIndex(SteamVR_ControllerManager.DeviceRelation.First, Valve.VR.ETrackedDeviceClass.Controller);
            SteamVR_Controller controller;
            controller = type == ControllerType.Right ? steamVR.ControllerManager.RightHand : steamVR.ControllerManager.LeftHand;

            Owner.IsVisible = controller != null;

            if (controller != null)
            {
                transform.LocalPosition = controller.WorldTransform.Translation;
                transform.LocalOrientation = controller.WorldTransform.Orientation;
            }
        }
    }
}
