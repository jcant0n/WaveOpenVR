using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;

namespace HTCVive
{
    public class DrawVideo : Drawable3D
    {
        /// <summary>
        /// The sprite batch
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// The graphics device
        /// </summary>
        protected GraphicsDevice graphicsDevice;

        /// <summary>
        /// The platform service
        /// </summary>
        protected Platform platform;

        public DrawVideo()
        {
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            // Set services
            this.graphicsDevice = WaveServices.GraphicsDevice;
            this.platform = WaveServices.Platform;
            this.spriteBatch = new SpriteBatch(this.graphicsDevice);
        }

        protected override void Initialize()
        {
            //if (WaveServices.CameraCapture.IsConnected)
            //{
            //    WaveServices.CameraCapture.Start(WaveEngine.Common.Media.CameraCaptureType.Front);
            //}
        }

        protected override void Removed()
        {
            //if (WaveServices.CameraCapture.IsConnected)
            //{
            //    WaveServices.CameraCapture.Stop();
            //}
        }

        public override void Draw(TimeSpan gameTime)
        {
            this.graphicsDevice.RenderTargets.SetRenderTarget(null);
            this.spriteBatch.Draw(StaticResources.DefaultTexture, new Rectangle(0, 0, this.platform.ScreenWidth, this.platform.ScreenHeight), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            this.spriteBatch.Render();
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
