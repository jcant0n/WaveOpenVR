#region File Description
//-----------------------------------------------------------------------------
// SteamVR_DeviceManager
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
    using Helpers;
    using System.Collections.Generic;
    using Valve.VR;

    public class SteamVR_ControllerManager
    {
        public SteamVR_Controller[] Controllers;

        private bool[] connected; // Only Controllers
        private uint leftIndex;
        private uint rightIndex;

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

        public IEnumerable<SteamVR_Controller> ControllerList
        {
            get
            {
                for (int i = 0; i < connected.Length; i++)
                {
                    if (connected[i])
                    {
                        yield return Controllers[i];
                    }
                }
            }
        }
        #endregion

        #region Initialize
        public SteamVR_ControllerManager(SteamVR_Service service)
        {
            leftIndex = OpenVR.k_unTrackedDeviceIndexInvalid;
            rightIndex = OpenVR.k_unTrackedDeviceIndexInvalid;

            service.OnDeviceChanged += OnDeviceChanged;
        }

        #endregion

        #region Public Methods

        public SteamVR_Controller Input(int index)
        {
            if ((int)index > -1 && index < OpenVR.k_unMaxTrackedDeviceCount)
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

        internal void Update()
        {
            // Update all controllers
            for (int i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                this.Controllers[i].Update();
            }
        }

        #endregion

        #region Private Methods

        private void OnDeviceChanged(object sender, DeviceEventArgs e)
        {
            bool changed = this.connected[e.Index];
            this.connected[e.Index] = false;

            var hmd = SteamVR_Service.hmd;

            if (e.Connected)
            {
                if (hmd != null && hmd.GetTrackedDeviceClass(e.Index) == ETrackedDeviceClass.Controller)
                {
                    this.connected[e.Index] = true;
                    changed = !changed; 
                }
            }

            if (changed)
            {
                leftIndex = hmd.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
                rightIndex = hmd.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);

                // If neither role has been assigned yet, try hooking up at least the right controller.
                if (leftIndex == OpenVR.k_unTrackedDeviceIndexInvalid && rightIndex == OpenVR.k_unTrackedDeviceIndexInvalid)
                {
                    for (int i = 0; i < this.connected.Length; i++)
                    {
                        if (this.connected[i])
                        {
                            rightIndex = (uint)i;
                            break;
                        }
                    }
                }
            }
        }

        internal void Initialize()
        {
            if (Controllers == null)
            {
                connected = new bool[OpenVR.k_unMaxTrackedDeviceCount];
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
