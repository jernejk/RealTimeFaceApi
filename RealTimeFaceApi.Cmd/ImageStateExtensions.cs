using OpenCvSharp;
using RealTimeFaceApi.Core.Data;
using System.Linq;

namespace RealTimeFaceApi.Cmd
{
    public static class ImageStateExtensions
    {
        public static FrameState ToImageState(this Rect[] faces)
        {
            var newFaces = faces.Select(face => new Face
            {
                Center = new System.Drawing.Point
                {
                    X = (int)(face.X + face.Width * 0.5),
                    Y = (int)(face.Y + face.Height * 0.5)
                },
                Size = new System.Drawing.Size
                {
                    Width = (int)(face.Width * 0.5) + 10,
                    Height = (int)(face.Height * 0.5) + 10
                }
            });

            return new FrameState(newFaces);
        }
    }
}
