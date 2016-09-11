using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Valve.VR;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.OpenVR;

namespace HTCVive
{
    [DataContract]
    public class ControllerDrawable : Drawable3D
    {
        [RequiredComponent]
        private Transform3D transform;

        [RequiredService]
        public SteamVR_Service steamVR;

        private LineBatch3D lineBatch;

        public ControllerDrawable()
        {
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.lineBatch = this.RenderManager.FindLayer(DefaultLayers.Opaque).LineBatch3D;
        }

        public override void Draw(TimeSpan gameTime)
        {
            int index = steamVR.ControllerManager.GetDeviceIndex(SteamVR_ControllerManager.DeviceRelation.First, Valve.VR.ETrackedDeviceClass.Controller);
            var controller = steamVR.ControllerManager.Input(index);

            if(controller == null)
            {
                return;
            }

            if (controller.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger))
            {
                var position = transform.Position;

                var forward = transform.WorldTransform.Forward;
                var up = transform.WorldTransform.Up;
                var left = transform.WorldTransform.Left;

                float div = 3f;
                forward = Vector3.Divide(forward, div);
                up = Vector3.Divide(up, div);
                left = Vector3.Divide(left, div);

                this.lineBatch.DrawLine(position, position + forward, Color.Blue);
                this.lineBatch.DrawLine(position, position + left, Color.Red);
                this.lineBatch.DrawLine(position, position + up, Color.Yellow);
            }
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
