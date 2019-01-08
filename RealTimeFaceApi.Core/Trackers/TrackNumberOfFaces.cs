using RealTimeFaceApi.Core.Data;

namespace RealTimeFaceApi.Core.Trackers
{
    public class TrackNumberOfFaces : IFaceTrackingChanged
    {
        public bool HasChanged(FrameState previousState, FrameState newState)
        {
            var prevStateNumber = previousState?.Faces?.Count ?? 0;
            var newStateNumber = newState?.Faces?.Count ?? 0;

            return prevStateNumber != newStateNumber;
        }
    }
}
