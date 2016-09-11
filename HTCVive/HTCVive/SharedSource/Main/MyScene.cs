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

            Entity virtualCamera = EntityManager.Find("Controller");
            virtualCamera
                .AddComponent(new ControllerUpdater())
                .AddComponent(new ControllerDrawable());
        }

        protected override void Start()
        {
            base.Start();

            var steamVR = WaveServices.GetService<SteamVR_Service>();

            if (steamVR != null)
            {
                Labels.Add("TrackingSystemName", steamVR.TrackingSystemName);
                Labels.Add("ModelNumber", steamVR.ModelNumber);
                Labels.Add("SerialNumber", steamVR.SerialNumber);
                Labels.Add("SecondsFromVsyncToPhotons", steamVR.SecondsFromVsyncToPhotons);
                Labels.Add("DisplayFrequency", steamVR.DisplayFrequency);
            }
        }
    }
}
