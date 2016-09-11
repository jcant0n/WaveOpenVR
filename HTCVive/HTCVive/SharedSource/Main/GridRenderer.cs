using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace HTCVive
{
    [DataContract]
    public class GridRenderer : Drawable3D
    {
        /// <summary>
        /// The maxlines
        /// </summary>
        private const int MAXLINES = 6;

        /// <summary>
        /// The lines
        /// </summary>
        private List<Line> lines;

        /// <summary>
        /// The line
        /// </summary>
        private Line line;

        /// <summary>
        /// The layer
        /// </summary>
        private LineBatch3D lineBatch;

        public GridRenderer()
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
            this.lines = new List<Line>();
            this.line = new Line();
            float maxLinesOverTwo = MAXLINES / 2;

            // X Axis
            for (int x = 0; x <= MAXLINES; x++)
            {
                // Start
                this.line.StartPoint.X = -maxLinesOverTwo + x;
                this.line.StartPoint.Y = 0;
                this.line.StartPoint.Z = -maxLinesOverTwo;

                // End
                this.line.EndPoint.X = -maxLinesOverTwo + x;
                this.line.EndPoint.Y = 0;
                this.line.EndPoint.Z = maxLinesOverTwo;

                this.line.Color = (x == maxLinesOverTwo) ? Color.Blue : Color.DarkGray;

                this.lines.Add(this.line);
            }

            // Z Axis
            for (int z = 0; z <= MAXLINES; z++)
            {
                // Start
                this.line.StartPoint.X = -maxLinesOverTwo;
                this.line.StartPoint.Y = 0;
                this.line.StartPoint.Z = -maxLinesOverTwo + z;

                // End
                this.line.EndPoint.X = maxLinesOverTwo;
                this.line.EndPoint.Y = 0;
                this.line.EndPoint.Z = -maxLinesOverTwo + z;

                this.line.Color = (z == maxLinesOverTwo) ? Color.Red : Color.DarkGray;

                this.lines.Add(this.line);
            }
        }

        public override void Draw(TimeSpan gameTime)
        {
            for (int i = 0; i < this.lines.Count; i++)
            {
                Line l = this.lines[i];
                this.lineBatch.DrawLine(ref l);
            }
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
