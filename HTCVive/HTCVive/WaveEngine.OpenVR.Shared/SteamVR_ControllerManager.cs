#region File Description
//-----------------------------------------------------------------------------
// SteamVR_ControllerManager
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
#endregion

using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;

namespace WaveEngine.OpenVR
{
    using Valve.VR;

    public class SteamVR_ControllerManager
    {
        public SteamVR_Controller[] Controllers;
        public bool[] Connected;

        private uint leftIndex;
        private uint rightIndex;

        public event EventHandler<uint> OnControllerConnected;
        public event EventHandler<uint> OnControllerDisconnected;

        public enum DeviceRelation
        {
            First,
            Leftmost,
            Rightmost,
            FarthestLeft,
            FarthestRight,
        }

        #region Properties

        public SteamVR_Controller LeftHand
        {
            get
            {
                if (leftIndex != OpenVR.k_unTrackedDeviceIndexInvalid)
                {
                    return Controllers[leftIndex];
                }

                return null;
            }
        }

        public SteamVR_Controller RightHand
        {
            get
            {
                if (rightIndex != OpenVR.k_unTrackedDeviceIndexInvalid)
                {
                    return Controllers[rightIndex];
                }

                return null;
            }
        }
        #endregion

        #region Initialize
        public SteamVR_ControllerManager()
        {
            leftIndex = OpenVR.k_unTrackedDeviceIndexInvalid;
            rightIndex = OpenVR.k_unTrackedDeviceIndexInvalid;
        }

        #endregion

        #region Public Methods

        public SteamVR_Controller Input(int index)
        {
            if (index > -1 && index < OpenVR.k_unMaxTrackedDeviceCount)
            {
                return Controllers[index];
            }

            return null;
        }

        public int GetDeviceIndex(DeviceRelation relation,
                                  ETrackedDeviceClass deviceClass = ETrackedDeviceClass.Controller,
                                  int relativeTo = (int)OpenVR.k_unTrackedDeviceIndex_Hmd) // use -1 for absolute tracking space
        {
            var result = -1;

            var invXform = ((uint)relativeTo < OpenVR.k_unMaxTrackedDeviceCount)
                ? Matrix.Invert(Input(relativeTo).WorldTransform) : Matrix.Identity;

            var system = OpenVR.System;
            if (system == null)
                return result;

            var best = -float.MaxValue;
            for (int i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                if (i == relativeTo || system.GetTrackedDeviceClass((uint)i) != deviceClass)
                    continue;

                var device = Input(i);
                if (!device.Connected)
                    continue;

                if (relation == DeviceRelation.First)
                    return i;

                float score;

                var pos = Vector3.Transform(device.WorldTransform.Translation, invXform);

                if (relation == DeviceRelation.FarthestRight)
                {
                    score = pos.X;
                }
                else if (relation == DeviceRelation.FarthestLeft)
                {
                    score = -pos.X;
                }
                else
                {
                    var dir = new Vector3(pos.X, 0.0f, pos.Z);
                    dir.Normalize();
                    var dot = Vector3.Dot(dir, Vector3.Forward);
                    var cross = Vector3.Cross(dir, Vector3.Forward);
                    if (relation == DeviceRelation.Leftmost)
                    {
                        score = (cross.Y > 0.0f) ? 2.0f - dot : dot;
                    }
                    else
                    {
                        score = (cross.Y < 0.0f) ? 2.0f - dot : dot;
                    }
                }

                if (score > best)
                {
                    result = i;
                    best = score;
                }
            }

            return result;
        }

        internal void Update(TrackedDevicePose_t[] poses)
        {
            for (int i = 0; i < poses.Length; i++)
            {
                var connected = poses[i].bDeviceIsConnected;
                if (connected != this.Connected[i])
                {
                    this.OnDeviceChanged(this, (uint)i, connected);
                }
            }

            // Update all controllers
            for (int i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                this.Controllers[i].Update();
            }
        }

        #endregion

        #region Private Methods
        private void OnDeviceChanged(object sender, uint index, bool connected)
        {
            bool changed = this.Connected[index];
            this.Connected[index] = false;

            var hmd = SteamVR_Service.hmd;

            if (connected)
            {
                if (hmd != null && hmd.GetTrackedDeviceClass(index) == ETrackedDeviceClass.Controller)
                {
                    this.Connected[index] = true;
                    changed = !changed;
                }
            }

            if (changed)
            {
                leftIndex = hmd.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
                rightIndex = hmd.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);

                
                if (connected)
                {
                    OnControllerConnected?.Invoke(null, index);
                }
                else
                {
                    OnControllerDisconnected?.Invoke(null, index);
                }
            }
        }

        internal void Initialize()
        {
            if (Controllers == null)
            {
                Connected = new bool[OpenVR.k_unMaxTrackedDeviceCount];
                Controllers = new SteamVR_Controller[OpenVR.k_unMaxTrackedDeviceCount];

                for (uint i = 0; i < Controllers.Length; i++)
                {
                    Controllers[i] = new SteamVR_Controller(i);
                }
            }
        }

        #endregion
    }
}
