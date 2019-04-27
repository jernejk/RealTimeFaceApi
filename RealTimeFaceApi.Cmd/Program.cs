using Microsoft.Azure.CognitiveServices.Vision.Face;
using OpenCvSharp;
using RealTimeFaceApi.Core.Data;
using RealTimeFaceApi.Core.Filters;
using RealTimeFaceApi.Core.Trackers;
using System;
using System.IO;
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

        private static readonly Scalar _faceColorBrush = new Scalar(0, 0, 255);
        private static FaceClient _faceClient;
        private static Task _faceRecognitionTask = null;

        public static void Main(string[] args)
        {
            _faceClient = new FaceClient(new ApiKeyServiceClientCredentials(FaceSubscriptionKey))
            {
                Endpoint = "https://westus.api.cognitive.microsoft.com"
            };

            string filename = args.FirstOrDefault();
            Run(filename);
        }

        private static void Run(string filename)
        {
            int timePerFrame;
            VideoCapture capture;
            if (!string.IsNullOrWhiteSpace(filename) && File.Exists(filename))
            {
                // If filename exists, use that as a source of video.
                capture = InitializeVideoCapture(filename);

                // Allow just enough time to paint the frame on the window.
                timePerFrame = 1;
            }
            else
            {
                // Otherwise use the webcam.
                capture = InitializeCapture();
                
                // Time required to wait until next frame.
                timePerFrame = (int)Math.Round(1000 / capture.Fps);
            }

            // Input was not initialized.
            if (capture == null)
            {
                Console.ReadKey();
                return;
            }

            // Initialize face detection algorithm.
            CascadeClassifier haarCascade = InitializeFaceClassifier();
            
            // List of simple face filtering algorithms.
            var filtering = new SimpleFaceFiltering(new IFaceFilter[]
            {
                new TooSmallFacesFilter(20, 20)
            });

            // List of simple face tracking algorithms.
            var trackingChanges = new SimpleFaceTracking(new IFaceTrackingChanged[]
            {
                new TrackNumberOfFaces(),
                new TrackDistanceOfFaces { Threshold = 2000 }
            });

            // Open a new window via OpenCV.
            using (Window window = new Window("capture"))
            {
                using (Mat image = new Mat())
                {
                    while (true)
                    {
                        // Get current frame.
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

                        if (hasChange)
                        {
                            Console.WriteLine("Changes detected...");

                            // Identify faces if changed and previous identification finished.
                            if (_faceRecognitionTask == null && !string.IsNullOrWhiteSpace(FaceSubscriptionKey))
                            {
                                _faceRecognitionTask = StartRecognizing(image);
                            }
                        }

                        using (var renderedFaces = RenderFaces(state, image))
                        {
                            // Update popup window.
                            window.ShowImage(renderedFaces);
                        }

                        // Wait for next frame and allow Window to be repainted.
                        Cv2.WaitKey(timePerFrame);
                    }
                }
            }
        }

        /// <summary>
        /// Use Microsoft Cognitive Services to recognize their faces.
        /// </summary>
        /// <param name="image">Video or web cam frame.</param>
        private static async Task StartRecognizing(Mat image)
        {
            try
            {
                Console.WriteLine(DateTime.Now + ": Attempting to recognize faces...");
                
                var stream = image.ToMemoryStream();
                var detectedFaces = await _faceClient.Face.DetectWithStreamAsync(stream, true, true);
                var faceIds = detectedFaces.Where(f => f.FaceId.HasValue).Select(f => f.FaceId.Value).ToList();

                if (faceIds.Any())
                {
                    var potentialUsers = await _faceClient.Face.IdentifyAsync(faceIds, FaceGroupId);
                    foreach (var candidate in potentialUsers.Select(u => u.Candidates.FirstOrDefault()))
                    {
                        Console.WriteLine(DateTime.Now + ": Identified user ID: " + candidate?.PersonId);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Getting identity failed.");
            }

            _faceRecognitionTask = null;
        }

        /// <summary>
        /// Initialize classifier used for offline face detection.
        /// </summary>
        private static CascadeClassifier InitializeFaceClassifier()
        {
            return new CascadeClassifier("Data/haarcascade_frontalface_alt.xml");
        }

        /// <summary>
        /// Initialize web cam capture.
        /// </summary>
        /// <returns>Returns web cam capture.</returns>
        private static VideoCapture InitializeCapture(int cameraIndex = 0)
        {
            VideoCapture capture = new VideoCapture();
            capture.Open(CaptureDevice.MSMF, cameraIndex);

            if (!capture.IsOpened())
            {
                Console.WriteLine("Unable to open capture.");
                return null;
            }

            return capture;
        }

        /// <summary>
        /// Initializes video capture for video files.
        /// </summary>
        /// <param name="file">Path to a video.</param>
        /// <returns>Return video file capture.</returns>
        private static VideoCapture InitializeVideoCapture(string file)
        {
            var capture = new VideoCapture(file);
            if (!capture.IsOpened())
            {
                Console.WriteLine("Unable to open video file {0}.", file);
                return null;
            }

            return capture;
        }

        /// <summary>
        /// Use OpenCV Cascade classifier to do offline face detection.
        /// </summary>
        /// <param name="cascadeClassifier">OpenCV cascade classifier.</param>
        /// <param name="image">Web cam or video frame.</param>
        /// <returns>Return list of faces as rectangles.</returns>
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

        /// <summary>
        /// Render detected faces via OpenCV.
        /// </summary>
        /// <param name="state">Current frame state.</param>
        /// <param name="image">Web cam or video frame.</param>
        /// <returns>Returns new image frame.</returns>
        private static Mat RenderFaces(FrameState state, Mat image)
        {
            Mat result = image.Clone();
            Cv2.CvtColor(image, image, ColorConversionCodes.BGR2GRAY);

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

                Cv2.Ellipse(result, center, axes, 0, 0, 360, _faceColorBrush, 4);
            }

            return result;
        }
    }
}
