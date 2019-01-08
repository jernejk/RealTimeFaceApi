using RealTimeFaceApi.Core.Data;

namespace RealTimeFaceApi.Core.Trackers
{
    public interface IFaceTrackingChanged
    {
        bool HasChanged(FrameState previousState, FrameState newState);
    }
}
