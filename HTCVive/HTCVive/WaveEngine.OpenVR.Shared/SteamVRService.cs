#region File Description
//-----------------------------------------------------------------------------
// SteamVRService
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common;
using System.Diagnostics;
using Valve.VR;
#endregion

namespace WaveEngine.OpenVR.Shared
{
    public class SteamVRService : UpdatableService
    {
        public Controller[] Controllers;
        internal static CVRSystem hmd;

        #region Properties
        public CVRSystem HMD
        {
            get
            {
                return hmd;
            }
        }

        public Controller LeftHand
        {
            get
            {
                if (hmd != null)
                {
                    uint index = 0;
                    index = hmd.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);

                    return Controllers[index];
                }

                return null;
            }
        }

        public Controller RightHand
        {
            get
            {
                if (hmd != null)
                {
                    uint index = 0;
                    index = hmd.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);

                    return Controllers[index];
                }

                return null;
            }
        }
        #endregion

        #region Initialize
        public SteamVRService()
        {
        }

        #endregion

        #region Public Methods

        public override void Update(TimeSpan gameTime)
        {
            for (int i = 0; i < Valve.VR.OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                this.Controllers[i].Update();
            }
        }
        #endregion

        #region Private Methods
        protected override void Initialize()
        {
            base.Initialize();

            var error = EVRInitError.None;

            hmd = Valve.VR.OpenVR.Init(ref error);
            if (error != EVRInitError.None)
            {
                ReportError(error);
                Valve.VR.OpenVR.Shutdown();
            }

            // Initialize devices
            if (Controllers == null)
            {
                Controllers = new Controller[Valve.VR.OpenVR.k_unMaxTrackedDeviceCount];

                for (uint i = 0; i < Controllers.Length; i++)
                {
                    Controllers[i] = new Controller(i);
                }
            }
        }

        protected override void Terminate()
        {
            base.Terminate();
            Valve.VR.OpenVR.Shutdown();
        }

        private void ReportError(EVRInitError error)
        {
            switch (error)
            {
                case EVRInitError.None:
                    break;
                case EVRInitError.VendorSpecific_UnableToConnectToOculusRuntime:
                    Debug.WriteLine("SteamVR Initialization Failed!  Make sure device is on, Oculus runtime is installed, and OVRService_*.exe is running.");
                    break;
                case EVRInitError.Init_VRClientDLLNotFound:
                    Debug.WriteLine("SteamVR drivers not found!  They can be installed via Steam under Library > Tools.  Visit http://steampowered.com to install Steam.");
                    break;
                case EVRInitError.Driver_RuntimeOutOfDate:
                    Debug.WriteLine("SteamVR Initialization Failed!  Make sure device's runtime is up to date.");
                    break;
                default:
                    Debug.WriteLine(Valve.VR.OpenVR.GetStringForHmdError(error));
                    break;
            }
        }
        #endregion
    }
}
