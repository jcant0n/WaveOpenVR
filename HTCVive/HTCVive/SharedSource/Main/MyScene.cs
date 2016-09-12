#region Using Statements
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.OpenVR;
#endregion

namespace HTCVive
{
    public class MyScene : Scene
    {
        protected override void CreateScene()
        {
            WaveServices.ScreenContextManager.SetDiagnosticsActive(true);

            this.Load(WaveContent.Scenes.MyScene);

            //WaveServices.CameraCapture.Start(WaveEngine.Common.Media.CameraCaptureType.Front);
            //var texture = WaveServices.CameraCapture.PreviewTexture;

            //Entity sprite = new Entity()
            //                    .AddComponent(new Transform2D()
            //                    {
            //                        XScale = (float)this.VirtualScreenManager.VirtualWidth / (float)texture.Width,
            //                        YScale = (float)this.VirtualScreenManager.VirtualHeight / (float)texture.Height,
            //                    })
            //                    .AddComponent(new Sprite(texture))
            //                    .AddComponent(new SpriteRenderer(DefaultLayers.Opaque));
            //EntityManager.Add(sprite);

            Entity right = EntityManager.Find("RightController");
            right.AddComponent(new ControllerUpdater(ControllerUpdater.ControllerType.Right))
                .AddComponent(new ControllerDrawable());

            Entity left = EntityManager.Find("LeftController");
            left.AddComponent(new ControllerUpdater(ControllerUpdater.ControllerType.Left))
                .AddComponent(new ControllerDrawable());
        }
    }
}
