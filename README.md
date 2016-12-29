# TargetLocked
My first project using AvaloniaUI, DynamicData, ReactiveUI, OmniXAML and Microsoft Cognitive Services.

It uses Face API to look for faces in videos.

In order to build the solution you will need a Face API key, that you can get here: https://www.microsoft.com/cognitive-services/en-us/subscriptions?productId=/products/54d85c5b5eefd00dc474a0f0

After you got the Face API:

 1. Create a FaceApi.txt file in the Desktop project
 2. Set its `Build Action` to `None`.
 3. Set its `Copy to output folder` option to `Copy If Newer`
 4. Build the solution and run it.
 5. Add a new Task
 5. Select a video file (preferably MP4 or AVI).
 6. Choose the video extract options like start and end of cut to be processed and the number of frames to skip (this will determine how many frames will be processed looking for faces)
 7. Press `Start` and wait for the video to be processed :)
 8. When finished, you can review the results with the `Review` button
