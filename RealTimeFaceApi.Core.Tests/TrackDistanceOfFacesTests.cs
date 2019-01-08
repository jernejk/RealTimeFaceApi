using RealTimeFaceApi.Core.Data;
using RealTimeFaceApi.Core.Trackers;
using Shouldly;
using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace RealTimeFaceApi.Core.Tests
{
    public class TrackDistanceOfFacesTests
    {
        [Fact]
        public void ShouldNotHaveMovingFace()
        {
            // Arrange
            var tracking = CreateTracker();
            var face = new Face { Center = new Point(50, 50) };

            // Act
            var result = tracking.HasChanged(new FrameState(new List<Face> { face }), new FrameState(new List<Face> { face }));

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void ShouldHaveMovingFace()
        {
            // Arrange
            var tracking = CreateTracker(14);
            var oldFace = new Face { Center = new Point(50, 50) };
            var newFace = new Face { Center = new Point(60, 60) };

            // Act
            var result = tracking.HasChanged(new FrameState(new List<Face> { oldFace }), new FrameState(new List<Face> { newFace }));

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldNotHaveMovingFaceWhenHigherThreshold()
        {
            // Arrange
            var tracking = CreateTracker(15);
            var oldFace = new Face { Center = new Point(50, 50) };
            var newFace = new Face { Center = new Point(60, 60) };

            // Act
            var result = tracking.HasChanged(new FrameState(new List<Face> { oldFace }), new FrameState(new List<Face> { newFace }));

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void ShouldNotHaveMovingFaces()
        {
            // Arrange
            var tracking = CreateTracker(15);
            var oldFaces = new List<Face>
            {
                new Face { Center = new Point(50, 50) },
                new Face { Center = new Point(200, 200) }
            };
            var newFaces = new List<Face>
            {
                new Face { Center = new Point(55, 44) },
                new Face { Center = new Point(210, 198) }
            };

            // Act
            var result = tracking.HasChanged(new FrameState(oldFaces), new FrameState(newFaces));

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void ShouldHaveMovingFaces()
        {
            // Arrange
            var tracking = CreateTracker(15);
            var oldFaces = new List<Face>
            {
                new Face { Center = new Point(50, 50) },
                new Face { Center = new Point(200, 200) }
            };
            var newFaces = new List<Face>
            {
                new Face { Center = new Point(55, 44) },
                new Face { Center = new Point(210, 185) }
            };

            // Act
            var result = tracking.HasChanged(new FrameState(oldFaces), new FrameState(newFaces));

            // Assert
            result.ShouldBeTrue();
        }

        private static TrackDistanceOfFaces CreateTracker(double threshold = 0)
        {
            return new TrackDistanceOfFaces
            {
                Threshold = threshold
            };
        }
    }
}
