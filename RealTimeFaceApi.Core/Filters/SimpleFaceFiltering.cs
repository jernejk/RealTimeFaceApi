using RealTimeFaceApi.Core.Data;
using System.Collections.Generic;

namespace RealTimeFaceApi.Core.Filters
{
    public class SimpleFaceFiltering
    {
        private readonly IEnumerable<IFaceFilter> _faceFiltering;

        public SimpleFaceFiltering(IEnumerable<IFaceFilter> faceTrackings)
        {
            _faceFiltering = faceTrackings;
        }

        public FrameState OldState { get; private set; }

        public FrameState FilterFaces(FrameState newState)
        {
            var finalState = newState;
            foreach (var filter in _faceFiltering)
            {
                finalState = filter.Filter(OldState, finalState);
            }

            OldState = finalState;

            return finalState;
        }
    }
}
