using RealTimeFaceApi.Core.Data;
using RealTimeFaceApi.Core.Trackers;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace RealTimeFaceApi.Core.Tests
{
    public class TrackNumberOfFacesTests
    {
        [Fact]
        public void ShouldChange()
        {
            // Arrange
            var tracking = new TrackNumberOfFaces();

            // Act
            var result = tracking.HasChanged(new FrameState(new List<Face>()), new FrameState(new List<Face> { new Face() }));

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldNotChange()
        {
            // Arrange
            var tracking = new TrackNumberOfFaces();
            var face = new Face();

            // Act
            var result = tracking.HasChanged(new FrameState(new List<Face> { face }), new FrameState(new List<Face> { face }));

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void ShouldChangeEvenWhenNull()
        {
            // Arrange
            var tracking = new TrackNumberOfFaces();
            var face = new Face();

            // Act
            var result = tracking.HasChanged(null, new FrameState(new List<Face> { face }));

            // Assert
            result.ShouldBeTrue();
        }
    }
}
