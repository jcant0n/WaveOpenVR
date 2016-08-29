#region File Description
//-----------------------------------------------------------------------------
// Device
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
#endregion

using System;
using Valve.VR;
using WaveEngine.Common.Math;

namespace WaveEngine.OpenVR.Shared
{
    public class Device
    {
        public float hairTriggerDelta = 0.1f; // amount trigger must be pulled or released to change state
        float hairTriggerLimit;
        bool hairTriggerState, hairTriggerPrevState;

        VRControllerState_t currentState, prevState;
        TrackedDevicePose_t pose;
        int previousFrameCount = -1;

        #region Properties
        public uint Index { get; private set; }

        public bool Valid { get; private set; }

        public bool Connected
        {
            get
            {
                this.Update();
                return pose.bDeviceIsConnected;
            }
        }

        public bool HasTracking
        {
            get
            {
                this.Update();
                return pose.bPoseIsValid;
            }
        }

        public bool OutOfRange
        {
            get
            {
                this.Update();
                return pose.eTrackingResult == ETrackingResult.Running_OutOfRange || pose.eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
            }
        }

        public bool Calibrating
        {
            get
            {
                this.Update();
                return pose.eTrackingResult == ETrackingResult.Calibrating_InProgress || pose.eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
            }
        }

        public bool Uninitialized
        {
            get
            {
                this.Update();
                return pose.eTrackingResult == ETrackingResult.Uninitialized;
            }
        }

        // Transform

        public Vector3 Velocity
        {
            get
            {
                this.Update();
                return new Vector3(pose.vVelocity.v0, pose.vVelocity.v1, pose.vVelocity.v2);
            }
        }

        public Vector3 AngularVelocity
        {
            get
            {
                this.Update();
                return new Vector3(-pose.vAngularVelocity.v0, -pose.vAngularVelocity.v1, pose.vAngularVelocity.v2);
            }
        }

        public VRControllerState_t CurrentState
        {
            get
            { 
                this.Update();
                return this.currentState;
            }
        }

        public VRControllerState_t PreviousState
        {
            get
            {
                this.Update();
                return this.prevState;
            }
        }

        public TrackedDevicePose_t Pose
        {
            get
            {
                this.Update();
                return this.pose;
            }
        }
        #endregion

        #region Initialize
        public Device(uint i)
        {
            this.Index = i;
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            // TimeCount
            if (SteamVRService.hmd != null)
            {
                Valid = SteamVRService.hmd.GetControllerStateWithPose(ETrackingUniverseOrigin.TrackingUniverseStanding, Index, ref this.currentState, ref this.pose);
                UpdateHairTrigger();
            }
        }

        public bool GetPress(ulong buttonMask)
        {
            this.Update();
            return (this.currentState.ulButtonPressed & buttonMask) != 0;
        }

        public bool GetPressDown(ulong buttonMask)
        {
            this.Update();
            return (this.currentState.ulButtonPressed & buttonMask) != 0 && (this.prevState.ulButtonPressed & buttonMask) == 0;
        }

        public bool GetPressUp(ulong buttonMask)
        {
            this.Update();
            return (this.currentState.ulButtonPressed & buttonMask) == 0 && (this.prevState.ulButtonPressed & buttonMask) != 0;
        }

        public bool GetPress(EVRButtonId buttonId)
        {
            return GetPress(1ul << (int)buttonId);
        }

        public bool GetPressDown(EVRButtonId buttonId)
        {
            return GetPressDown(1ul << (int)buttonId);
        }

        public bool GetPressUp(EVRButtonId buttonId)
        {
            return GetPressUp(1ul << (int)buttonId);
        }

        public bool GetTouch(ulong buttonMask)
        {
            this.Update();
            return (currentState.ulButtonTouched & buttonMask) != 0;
        }

        public bool GetTouchDown(ulong buttonMask)
        {
            this.Update();
            return (this.currentState.ulButtonTouched & buttonMask) != 0 && (this.prevState.ulButtonTouched & buttonMask) == 0;
        }

        public bool GetTouchUp(ulong buttonMask)
        {
            this.Update();
            return (this.currentState.ulButtonTouched & buttonMask) == 0 && (this.prevState.ulButtonTouched & buttonMask) != 0;
        }

        public bool GetTouch(EVRButtonId buttonId)
        {
            return GetTouch(1ul << (int)buttonId);
        }

        public bool GetTouchDown(EVRButtonId buttonId)
        {
            return GetTouchDown(1ul << (int)buttonId);
        }

        public bool GetTouchUp(EVRButtonId buttonId)
        {
            return GetTouchUp(1ul << (int)buttonId);
        }

        public Vector2 GetAxis(EVRButtonId buttonId = EVRButtonId.k_EButton_SteamVR_Touchpad)
        {
            this.Update();
            var axisId = (uint)buttonId - (uint)EVRButtonId.k_EButton_Axis0;

            switch (axisId)
            {
                case 0: return new Vector2(this.currentState.rAxis0.x, this.currentState.rAxis0.y);
                case 1: return new Vector2(this.currentState.rAxis1.x, this.currentState.rAxis1.y);
                case 2: return new Vector2(this.currentState.rAxis2.x, this.currentState.rAxis2.y);
                case 3: return new Vector2(this.currentState.rAxis3.x, this.currentState.rAxis3.y);
                case 4: return new Vector2(this.currentState.rAxis4.x, this.currentState.rAxis4.y);
            }
            return Vector2.Zero;
        }

        public void TriggerHapticPulse(ushort durationMicroSec = 500, EVRButtonId buttonId = EVRButtonId.k_EButton_SteamVR_Touchpad)
        {
            if (SteamVRService.hmd != null)
            {
                var axisId = (uint)buttonId - (uint)EVRButtonId.k_EButton_Axis0;
                SteamVRService.hmd.TriggerHapticPulse(Index, axisId, (char)durationMicroSec);
            }
        }

        public bool GetHairTrigger()
        {
            this.Update();
            return hairTriggerState;
        }

        public bool GetHairTriggerDown()
        {
            this.Update();
            return hairTriggerState && !hairTriggerPrevState;
        }

        public bool GetHairTriggerUp()
        {
            this.Update();
            return !hairTriggerState && hairTriggerPrevState;
        }
        #endregion

        #region Private Methods
        private void UpdateHairTrigger()
        {
            hairTriggerPrevState = hairTriggerState;
            var value = this.currentState.rAxis1.x; // trigger

            if (hairTriggerState)
            {
                if (value < hairTriggerLimit - hairTriggerDelta || value <= 0.0f)
                {
                    hairTriggerState = false;
                }
            }
            else
            {
                if (value > hairTriggerLimit + hairTriggerDelta || value >= 1.0f)
                {
                    hairTriggerState = true;
                }
            }

            hairTriggerLimit = hairTriggerState ? Math.Max(hairTriggerLimit, value) : Math.Min(hairTriggerLimit, value);
        }

        #endregion
    }
}
