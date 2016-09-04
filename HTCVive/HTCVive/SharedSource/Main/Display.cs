using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace HTCVive
{
    public class Display : Behavior
    {
        protected override void Update(TimeSpan gameTime)
        {
            Console.WriteLine("\r" + WaveServices.Clock.FrameCount);
            //Console.WriteLine(WaveServices.Clock.UpdateCount);
            //Console.WriteLine("\r\r");
        }
    }
}
