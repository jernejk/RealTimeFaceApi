using RealTimeFaceApi.Core.Data;
using System.Collections.Generic;

namespace RealTimeFaceApi.Core.Filters
{
    public class TooSmallFacesFilter : IFaceFilter
    {
        public TooSmallFacesFilter(double minimumWidth, double minimumHeight)
        {
            MinimumWidth = minimumWidth;
            MinimumHeight = minimumHeight;
        }

        public double MinimumWidth { get; }
        public double MinimumHeight { get; }

        public FrameState Filter(FrameState previousState, FrameState currentState)
        {
            var faces = new List<Face>(currentState.Faces);
            foreach (var face in currentState.Faces)
            {
                if (face.Size.Width < MinimumWidth || face.Size.Height < MinimumHeight)
                {
                    faces.Remove(face);
                }
            }

            // Avoid additional allocations if it hasn't changed.
            return
                faces.Count == currentState.Faces.Count ?
                currentState :
                new FrameState(faces);
        }
    }
}
