using OpenCvSharp;
using RealTimeFaceApi.Core.Data;
using RealTimeFaceApi.Core.Filters;
using RealTimeFaceApi.Core.Trackers;
using System;

namespace RealTimeFaceApi.Cmd
{
    class Program
    {
        private static readonly Scalar _staticColor = new Scalar(0, 0, 255);

        static void Main(string[] args)
        {
            VideoCapture capture = InitializeCapture();
            if (capture == null)
            {
                Console.ReadKey();
                return;
            }

            CascadeClassifier haarCascade = InitializeFaceClassifier();
            int timePerFrame = (int)Math.Round(1000 / capture.Fps);

            var filtering = new SimpleFaceFiltering(new IFaceFilter[]
            {
                new TooSmallFacesFilter(20, 20)
            });

            var trackingChanges = new SimpleFaceTracking(new IFaceTrackingChanged[]
            {
                new TrackNumberOfFaces(),
                new TrackDistanceOfFaces { Threshold = 2000 }
            });

            using (Window window = new Window("capture"))
            using (Mat image = new Mat())
            {
                while (true)
                {
                    capture.Read(image);
                    if (image.Empty())
                        continue;

                    // Detect faces
                    var faces = DetectFaces(haarCascade, image);

                    // Filter faces
                    var state = faces.ToImageState();
                    state = filtering.FilterFaces(state);

                    // Determine change
                    var hasChange = trackingChanges.ShouldUpdateRecognition(state);

                    // Identify faces if changed.
                    if (hasChange)
                    {
                        Console.WriteLine(DateTime.Now.Ticks + ": Changed, getting new identity!");

                        // TODO: Call Microsoft Cognitive Services.
                    }

                    using (var renderedFaces = RenderFaces(state, image, _staticColor))
                    {
                        window.ShowImage(renderedFaces);
                    }

                    Cv2.WaitKey(timePerFrame);
                }
            }
        }

        private static CascadeClassifier InitializeFaceClassifier()
        {
            return new CascadeClassifier("haarcascade_frontalface_alt.xml");
        }

        private static VideoCapture InitializeCapture()
        {
            VideoCapture capture = new VideoCapture();
            capture.Open(CaptureDevice.MSMF, 1);

            if (!capture.IsOpened())
            {
                Console.WriteLine("Unable to open capture.");
                return null;
            }

            return capture;
        }

        private static Rect[] DetectFaces(CascadeClassifier cascadeClassifier, Mat image)
        {
            return cascadeClassifier
                .DetectMultiScale(
                    image,
                    1.08,
                    2,
                    HaarDetectionType.ScaleImage,
                    new Size(60, 60));
        }

        private static Mat RenderFaces(FrameState state, Mat original, Scalar color)
        {
            Mat result = original.Clone();
            Cv2.CvtColor(original, original, ColorConversionCodes.BGR2GRAY);

            // Render all detected faces
            foreach (var face in state.Faces)
            {
                var center = new Point
                {
                    X = face.Center.X,
                    Y = face.Center.Y
                };
                var axes = new Size
                {
                    Width = (int)(face.Size.Width * 0.5) + 10,
                    Height = (int)(face.Size.Height * 0.5) + 10
                };

                Cv2.Ellipse(result, center, axes, 0, 0, 360, new Scalar(0, 0, 255), 4);
            }

            return result;
        }
    }
}
