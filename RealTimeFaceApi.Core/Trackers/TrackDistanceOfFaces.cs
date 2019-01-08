using RealTimeFaceApi.Core.Data;
using RealTimeFaceApi.Core.Utils;
using System;
using System.Linq;

namespace RealTimeFaceApi.Core.Trackers
{
    public class TrackDistanceOfFaces : IFaceTrackingChanged
    {
        public double Threshold { get; set; }

        public bool HasChanged(FrameState previousState, FrameState newState)
        {
            foreach (var face in newState.Faces)
            {
                var center = face.Center;

                // Get shortest distance to the current face.
                var faceWithShortestDistance = previousState
                    .Faces
                    .Select(r => MathHelper.GetDistancePow2(center, r.Center))
                    .Min(r => r);

                // We only need to sqrt only the minimum distance.
                faceWithShortestDistance = Math.Sqrt(faceWithShortestDistance);
                if (faceWithShortestDistance > Threshold)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
