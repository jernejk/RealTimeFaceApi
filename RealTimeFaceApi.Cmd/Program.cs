using Microsoft.Azure.CognitiveServices.Vision.Face;
using OpenCvSharp;
using RealTimeFaceApi.Core.Data;
using RealTimeFaceApi.Core.Filters;
using RealTimeFaceApi.Core.Trackers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeFaceApi.Cmd
{
    public static class Program
    {
        // TODO: Add Face API subscription key.
        private static string FaceSubscriptionKey = "";

        // TODO: Add face group ID.
        private static string FaceGroupId = "";

        private static Task _faceRecognitionTask = null;
        private static readonly Scalar _staticColor = new Scalar(0, 0, 255);

        public static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
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

                    // Identify faces if changed and previous identification finished.
                    if (hasChange && _faceRecognitionTask == null)
                    {
                        _faceRecognitionTask = StartRecognizing(image);
                    }

                    using (var renderedFaces = RenderFaces(state, image, _staticColor))
                    {
                        // Update popup window.
                        window.ShowImage(renderedFaces);
                    }

                    Cv2.WaitKey(timePerFrame);
                }
            }
        }

        private static async Task StartRecognizing(Mat image)
        {
            try
            {
                Console.WriteLine(DateTime.Now + ": Attempting to recognize faces...");

                var client = new FaceClient(new ApiKeyServiceClientCredentials(FaceSubscriptionKey))
                {
                    Endpoint = "https://westus.api.cognitive.microsoft.com"
                };

                var stream = image.ToMemoryStream();
                var detectedFaces = await client.Face.DetectWithStreamAsync(stream, true, true);
                var faceIds = detectedFaces.Where(f => f.FaceId.HasValue).Select(f => f.FaceId.Value).ToList();
                var potentialUsers = await client.Face.IdentifyAsync(faceIds, FaceGroupId);

                foreach (var candidate in potentialUsers.Select(u => u.Candidates.FirstOrDefault()))
                {
                    Console.WriteLine(DateTime.Now + ": Identified user ID: " + candidate.PersonId);
                }
            }
            catch
            {
                Console.WriteLine("Getting identity failed.");
            }

            _faceRecognitionTask = null;
        }

        private static CascadeClassifier InitializeFaceClassifier()
        {
            return new CascadeClassifier("Data/haarcascade_frontalface_alt.xml");
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
