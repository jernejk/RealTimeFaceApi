using System.Collections.Generic;
using System.Linq;

namespace RealTimeFaceApi.Core.Data
{
    public class FrameState
    {
        public FrameState(IEnumerable<Face> faces)
        {
            Faces = faces.ToList().AsReadOnly();
        }

        public IReadOnlyList<Face> Faces { get; }
    }
}
