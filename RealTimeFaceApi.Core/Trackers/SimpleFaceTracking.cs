using RealTimeFaceApi.Core.Data;
using System.Collections.Generic;
using System.Linq;

namespace RealTimeFaceApi.Core.Trackers
{
    public class SimpleFaceTracking
    {
        private readonly IEnumerable<IFaceTrackingChanged> _faceTrackings;

        public SimpleFaceTracking(IEnumerable<IFaceTrackingChanged> faceTrackings)
        {
            _faceTrackings = faceTrackings;
        }

        public FrameState OldState { get; private set; }

        public bool ShouldUpdateRecognition(FrameState newState)
        {
            if (_faceTrackings.Any(tracker => tracker.HasChanged(OldState, newState)))
            {
                OldState = newState;
                return true;
            }

            return false;
        }
    }
}
