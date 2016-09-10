#region File Description
//-----------------------------------------------------------------------------
// SteamVR_Service
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common;
using System.Diagnostics;
#endregion

namespace WaveEngine.OpenVR
{
    using Valve.VR;

    public class SteamVR_Service : UpdatableService
    {
        internal static CVRSystem hmd;
        private SteamVR_ControllerManager controllerManager;

        private TrackedDevicePose_t[] poses;
        private TrackedDevicePose_t[] gamePoses;

        #region Properties
        public CVRSystem HMD
        {
            get
            {
                return hmd;
            }
        }

        public SteamVR_ControllerManager ControllerManager
        {
            get
            {
                return controllerManager;
            }
        }

        #endregion

        #region Initialize
        public SteamVR_Service()
        {
            poses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            gamePoses = new TrackedDevicePose_t[0];

            this.controllerManager = new SteamVR_ControllerManager();
        }

        #endregion

        #region Public Methods

        public override void Update(TimeSpan gameTime)
        {
            var compositor = OpenVR.Compositor;

            if (compositor != null)
            {
                compositor.GetLastPoses(poses, gamePoses);
                controllerManager.Update(poses);
            }
        }

        #endregion

        #region Private Methods
        protected override void Initialize()
        {
            base.Initialize();

            var error = EVRInitError.None;

            bool initialized = true;
            hmd = OpenVR.Init(ref error);
            if (error != EVRInitError.None)
            {
                ReportError(error);
                initialized = false;
            }

            // Verify common interfaces are valid.
            OpenVR.GetGenericInterface(OpenVR.IVRCompositor_Version, ref error);
            if (error != EVRInitError.None)
            {
                ReportError(error);
                initialized = false;
            }

            OpenVR.GetGenericInterface(OpenVR.IVROverlay_Version, ref error);
            if (error != EVRInitError.None)
            {
                ReportError(error);
                initialized = false;
            }

            if (!initialized)
            {
                OpenVR.Shutdown();
                return;
            }

            controllerManager.Initialize();
        }

        protected override void Terminate()
        {
            base.Terminate();
            OpenVR.Shutdown();
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
                    Debug.WriteLine(OpenVR.GetStringForHmdError(error));
                    break;
            }
        }
        #endregion
    }
}
