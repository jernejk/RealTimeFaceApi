using RealTimeFaceApi.Core.Data;

namespace RealTimeFaceApi.Core.Filters
{
    public interface IFaceFilter
    {
        FrameState Filter(FrameState previousState, FrameState newState);
    }
}
