[![Build status](https://ci.appveyor.com/api/projects/status/s8o64bsqd4agvocm/branch/master?svg=true)](https://ci.appveyor.com/project/jernejk/realtimefaceapi/branch/master)

Blog post: [Real-time face recognition with Microsoft Cognitive Services](https://jkdev.me/real-time-face-recognition/)

YouTube video: [.NET User Group: Real-time Face Recognition With Microsoft Cognitive Services](https://www.youtube.com/watch?v=KCSyRO0KotA)

Cognitive Services Explorer (in progress): [https://jernejk.github.io/CognitiveServices.Explorer/face/intro](https://jernejk.github.io/CognitiveServices.Explorer/face/intro)

# RealTimeFaceApi
This is a demo project showing how to use Face API in Cognitive Services with OpenCV.

The demo has several parts to allow real-time facial recognition:
- Get frames from web cam/video with help of OpenCV
- Offline face-detection with help of OpenCV
- Filter out faces that are too small
- Determine if scene has changed and we should recognize new faces
- Sending and identifying faces with help of Microsoft Cognitive Services

# Setup
In `RealTimeFaceApi/Program.cs` configure `FaceSubscriptionKey` and `FaceGroupId` from [Microsoft Cognitive Services](https://azure.microsoft.com/en-au/services/cognitive-services/).
You'll need to upload and train data in MS Face API.

# Run with web came
Make sure there are no command line arguments.

If you have multiple web cams, you can change the camera with `cameraIndex`.

``` C#
// Otherwise use the webcam.
capture = InitializeCapture(/* camera index */ 1);
```

You can run app from console:
``` C#
cd RealTimeFaceApi.Cmd
dotnet run
```

![One Person Moving](one-person-moving.gif)

# Run with a video file
Add path to file. In Visual Studio, go to `RealTimeFaceApi.Cmd` properties, `Debug` and under `Application arguments:` add the path to the video.

You can also run from console:
``` C#
cd RealTimeFaceApi.Cmd
dotnet run -- "C:\Users\JK\Downloads\Real-time Face Recognition With Microsoft Cognitive Services.mp4"
```

![Video File Face Detection](video-face-detection.png)
