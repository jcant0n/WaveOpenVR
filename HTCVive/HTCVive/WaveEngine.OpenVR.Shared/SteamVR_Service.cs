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
    using Helpers;
    using Valve.VR;

    public class SteamVR_Service : UpdatableService
    {
        internal static CVRSystem hmd;
        private SteamVR_ControllerManager controllerManager;

        private bool[] connected;
        private TrackedDevicePose_t[] poses;
        private TrackedDevicePose_t[] gamePoses;

        // Events
        public event EventHandler<DeviceEventArgs> OnDeviceChanged;

        #region Properties
        public CVRSystem HMD
        {
            get
            {
                return hmd;
            }
        }

        public CVRCompositor Compositor
        {
            get
            {
                return OpenVR.Compositor;
            }
        }

        public CVROverlay Overlay
        {
            get
            {
                return OpenVR.Overlay;
            }
        }

        public SteamVR_ControllerManager ControllerManager
        {
            get
            {
                return this.controllerManager;
            }
        }

        public bool VRInitializing { get; private set; }

        public bool VRCalibrating { get; private set; }

        public bool VROutOfRange { get; private set; }

        public string TrackingSystemName
        {
            get
            {
                return GetStringProperty(ETrackedDeviceProperty.Prop_TrackingSystemName_String);
            }
        }

        public string ModelNumber
        {
            get
            {
                return GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String);
            }
        }

        public string SerialNumber
        {
            get
            {
                return GetStringProperty(ETrackedDeviceProperty.Prop_SerialNumber_String);
            }
        }

        public float SecondsFromVsyncToPhotons
        {
            get
            {
                return GetFloatProperty(ETrackedDeviceProperty.Prop_SecondsFromVsyncToPhotons_Float);
            }
        }

        public float DisplayFrequency
        {
            get
            {
                return GetFloatProperty(ETrackedDeviceProperty.Prop_DisplayFrequency_Float);
            }
        }
        #endregion

        #region Initialize
        public SteamVR_Service()
        {
            connected = new bool[OpenVR.k_unMaxTrackedDeviceCount];
            poses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            gamePoses = new TrackedDevicePose_t[0];

            this.controllerManager = new SteamVR_ControllerManager(this);
        }

        #endregion

        #region Public Methods

        public override void Update(TimeSpan gameTime)
        {
            var compositor = OpenVR.Compositor;

            if (compositor != null)
            {

                compositor.GetLastPoses(poses, gamePoses);

                for (uint i = 0; i < poses.Length; i++)
                {
                    var connected = poses[i].bDeviceIsConnected;
                    if (connected != this.connected[i])
                    {
                        this.connected[i] = connected;
                        this.OnDeviceChanged?.Invoke(this, new DeviceEventArgs(i, connected));
                    }
                }

                this.controllerManager.Update();

                if (poses.Length > OpenVR.k_unTrackedDeviceIndex_Hmd)
                {
                    var result = poses[OpenVR.k_unTrackedDeviceIndex_Hmd].eTrackingResult;

                    var initializing = result == ETrackingResult.Uninitialized;
                    if (initializing != this.VRInitializing)
                    {
                        this.VRInitializing = initializing;
                    }

                    var calibrating =
                        result == ETrackingResult.Calibrating_InProgress ||
                        result == ETrackingResult.Calibrating_OutOfRange;
                    if (calibrating != this.VRCalibrating)
                    {
                        this.VRCalibrating = calibrating;
                    }

                    var outOfRange =
                        result == ETrackingResult.Running_OutOfRange ||
                        result == ETrackingResult.Calibrating_OutOfRange;
                    if (outOfRange != this.VROutOfRange)
                    {
                        this.VROutOfRange = outOfRange;
                    }
                }
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


        private string GetStringProperty(ETrackedDeviceProperty prop)
        {
            var error = ETrackedPropertyError.TrackedProp_Success;
            var capactiy = hmd.GetStringTrackedDeviceProperty(OpenVR.k_unTrackedDeviceIndex_Hmd, prop, null, 0, ref error);
            if (capactiy > 1)
            {
                var result = new System.Text.StringBuilder((int)capactiy);
                hmd.GetStringTrackedDeviceProperty(OpenVR.k_unTrackedDeviceIndex_Hmd, prop, result, capactiy, ref error);
                return result.ToString();
            }
            return (error != ETrackedPropertyError.TrackedProp_Success) ? error.ToString() : "<unknown>";
        }

        private float GetFloatProperty(ETrackedDeviceProperty prop)
        {
            var error = ETrackedPropertyError.TrackedProp_Success;
            return hmd.GetFloatTrackedDeviceProperty(OpenVR.k_unTrackedDeviceIndex_Hmd, prop, ref error);
        }

        #endregion
    }
}
