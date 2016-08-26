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
#endregion

namespace WaveEngine.OpenVR.Shared
{
    public class SteamVRService : Service
    {
        private Valve.VR.CVRSystem hmd;

        #region Initialize
        public SteamVRService()
        {
        }

        #endregion

        #region Public Methods
        public Valve.VR.CVRSystem HMD { get; set; }
        #endregion

        #region Private Methods
        protected override void Initialize()
        {
            base.Initialize();

            var error = Valve.VR.EVRInitError.None;

            hmd = Valve.VR.OpenVR.Init(ref error);
            if (error != Valve.VR.EVRInitError.None)
            {
                ReportError(error);
                Valve.VR.OpenVR.Shutdown();
            }
        }

        protected override void Terminate()
        {
            base.Terminate();
            Valve.VR.OpenVR.Shutdown();
        }

        private void ReportError(Valve.VR.EVRInitError error)
        {
            switch (error)
            {
                case Valve.VR.EVRInitError.None:
                    break;
                case Valve.VR.EVRInitError.VendorSpecific_UnableToConnectToOculusRuntime:
                    Debug.WriteLine("SteamVR Initialization Failed!  Make sure device is on, Oculus runtime is installed, and OVRService_*.exe is running.");
                    break;
                case Valve.VR.EVRInitError.Init_VRClientDLLNotFound:
                    Debug.WriteLine("SteamVR drivers not found!  They can be installed via Steam under Library > Tools.  Visit http://steampowered.com to install Steam.");
                    break;
                case Valve.VR.EVRInitError.Driver_RuntimeOutOfDate:
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
