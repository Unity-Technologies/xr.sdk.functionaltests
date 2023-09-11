# XR Automated Tests - Functional Tests

Here we provide XR automated test projects for both AR and VR functional testing.
- AR
-- Including test projects for ARCore, ARFoundation, ARMock
- VR
-- Including test projects for CrossPlatform and Windows Mixed Reality

## Target XR Displays or XR Assemblies
*Currently only VR project has the Test Interface scripts (XRDisplay & Assembly) implemented for the Functional Tests*
The attributes to target either XR Displays or Assemblies can be used to both support or exclude certain targets in order for tests, class, methods, or assemblies. This can be used to have only certain tests run on certain platforms while allowing a simple cross platform test matrix to exist.

### Target a XR Display
Tag the Class or Method with the attribute `TargetXrDisplays`
-The attribute takes a string to include or exclude target SDK's 
-- Example on a Unity Test: `[TargetXrDisplays(exclude = new[] { "MagicLeap-Display" })]`

### Target Certain XR Assemblies 
Tag the Class, Assembly, or Method with the attribute `ConditionalAssembly`
-The attribute takes a string to include or exclude target assemblies 
--Example on a Unity Test: `[ConditionalAssembly(exclude = new[] { "Unity.XR.MockHMD", "Microsoft.MixedReality.OpenXR" })]`

All of these tests use the [Unity Test Framework](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) to excercise their functionality. You can choose to open project in Unity to explore and run from the Test Runner tab, or launch tests from Unity command line using the `runTests` Unity.exe command line option.

Unity command line are full defined here:
[Unity Command Line Arguments](https://docs.unity3d.com/Manual/CommandLineArguments.html)

Running these tests from Unity command line provides flexibility for automation by allow you to specify how the test app (player) will be built prior to execution. If you choose to run the tests from the command line, you'll also want to use the `-testResults` option in order to save the test results to the .xml formatted-file, and the `-logfile` option to save the Unity editor log to a specified location. 

Once you've generated a test results file after running your tests, we encourage you to use the [Unity Test Runner Results Reporter](https://github.cds.internal.unity3d.com/unity/UnityTestRunnerResultsReporter). This test results reporter will generate an HTML report from your results, making it easy to read and understand test results and even can be used in a continuous integration system

For usage and details please check each individual test projects.
