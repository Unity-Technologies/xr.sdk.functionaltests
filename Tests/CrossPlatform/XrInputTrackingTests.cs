using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Linq;
using Unity.XRTesting;

[ConditionalAssembly(exclude = new []{"Unity.XR.MockHMD"/*, "Unity.XR.Management" */})]
internal class XrInputTrackingTests : XrFunctionalTestBase
{
    private List<XRNodeState> nodeList;
    private readonly List<InputDevice> devices = new List<InputDevice>();
    private readonly List<InputFeatureUsage> featureUsages = new List<InputFeatureUsage>();
    private InputDevice device;
    private readonly float angleComparisonTolerance = 2f;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        nodeList = new List<XRNodeState>();

        InputTracking.trackingAcquired += InputTracking_trackingAcquired;
        InputTracking.trackingLost += InputTracking_trackingLost;
        InputTracking.nodeAdded += InputTracking_nodeAdded;
        InputTracking.nodeRemoved += InputTracking_nodeRemoved;
    }

    [TearDown]
    public override void TearDown()
    {
        InputTracking.trackingAcquired -= InputTracking_trackingAcquired;
        InputTracking.trackingLost -= InputTracking_trackingLost;
        InputTracking.nodeAdded -= InputTracking_nodeAdded;
        InputTracking.nodeRemoved -= InputTracking_nodeRemoved;
        base.TearDown();
    }
    
    [UnityTest]
    public IEnumerator VerifyXrInputTracking_HeadTracking()
    {
        InputTracking.GetNodeStates(nodeList);
        yield return SkipFrame(DefaultFrameSkipCount);

        var headNodes = nodeList.Where(n => n.nodeType == XRNode.Head);
        Assert.True(headNodes.Any(), "Failed to find XRNode.Head node type.");
        Assert.True(headNodes.Select(n=>n.tracked == false).Any(), "Found untracked head node.");
    }

    [UnityPlatform(exclude = new[] { RuntimePlatform.IPhonePlayer, RuntimePlatform.PS5 })]
    [TargetXrDisplays(exclude = new[] { "MagicLeap-Display" })]
    [UnityTest]
    public IEnumerator VerifyXrInputTracking_userPresenceIsPresent()
    {
        //Allow for warmup period since app may be rendering but api is not ready
        yield return new WaitForSeconds(1);

        Debug.Log("Settings.EnabledXrTarget is " + Settings.EnabledXrTarget);

        if (Application.isEditor)
        {
            var reasonString = "Test is running in the Editor";

            Assert.Ignore("{0}: UserPresenceState.Present will always be false. Ignoring", reasonString);
        }
        else
        {
#if XR_SDK
            var device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            Assert.IsTrue(device.isValid, "The userPresence is UnSupported on this device. Expected head device is InValid.");
#if UNITY_2021_3_OR_NEWER
            Assert.IsTrue(device.TryGetFeatureValue(CommonUsages.userPresence, out bool value), "The userPresence was not found or is Unknown on the head device");
#else
            Assert.IsTrue(device.TryGetFeatureValue(new InputFeatureUsage<bool>("UserPresence"), out bool value), "The userPresence is Unknown on the head device");
#endif //UNITY_2021_3_OR_NEWER 
            Assert.IsTrue(value, "userPresence is Not Present on head device.");
#else
            Assert.AreEqual(XRDevice.userPresence, UserPresenceState.Present, string.Format("Not mobile platform. Expected XRDevice.userPresence to be {0}, but is {1}.", UserPresenceState.Present, XRDevice.userPresence));
#endif //XR_SDK
        }
    }

    [UnityTest]
    public IEnumerator VerifyXrInputTracking_EyeTracking()
    {
        InputTracking.GetNodeStates(nodeList);
        yield return SkipFrame(DefaultFrameSkipCount);

        // Verify left eye node
        var leftEyeNodes = nodeList.Where(n => n.nodeType == XRNode.LeftEye);
        Assert.True(leftEyeNodes.Any(), "Failed to find XRNode.LeftEye node type.");
        Assert.True(leftEyeNodes.Select(n => n.tracked == false).Any(), "Found untracked left eye node.");

        // Verify right eye node
        var rightEyeNodes = nodeList.Where(n => n.nodeType == XRNode.RightEye);
        Assert.True(rightEyeNodes.Any(), "Failed to find XRNode.RightEye node type.");
        Assert.True(rightEyeNodes.Select(n => n.tracked == false).Any(), "Found untracked right eye node.");
    }

    [UnityTest]
    public IEnumerator VerifyXrInputTracking_TryGetFeatureValue()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.Head, devices);
        device = devices.FirstOrDefault();
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.IsTrue(device.TryGetFeatureValue(CommonUsages.devicePosition, out var state), "Unable to TryGet feature value: device position");
    }

    [UnityTest]
    public IEnumerator VerifyXrInputTracking_TryGetFeatureUsages()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.Head, devices);
        device = devices.FirstOrDefault();
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.IsTrue(device.TryGetFeatureUsages(featureUsages), "Unable to TryGet feature usages for the head");
    }
    
    [Ignore("Unsure of what devices for this API are supported")]
    [UnityTest]
    public IEnumerator VerifyXrInputTracking_TryGetHapticCapabilities()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.CenterEye, devices);
        device = devices.FirstOrDefault();
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.IsTrue(device.TryGetHapticCapabilities(out var haptic), "Unable to TryGet Haptic Capabilities");
    }

    [Ignore("Unsure of what devices for this API are supported")]
    [UnityTest]
    public IEnumerator VerifyXrInputTracking_EyeDataPosition()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.LeftEye, devices);
        device = devices.FirstOrDefault();
        yield return SkipFrame(DefaultFrameSkipCount);
        
        Assert.IsTrue(device.TryGetFeatureValue(CommonUsages.leftEyePosition, out var eyes), "Unable to TryGet feature value: left eye data");
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.IsNotNull(eyes, "Couldn't get left eye position");
    }

    [Ignore("Unsure of what devices for this API are supported")]
    [UnityTest]
    public IEnumerator VerifyXrInputTracking_EyeDataRotation()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.LeftEye, devices);
        device = devices.FirstOrDefault();
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.IsTrue(device.TryGetFeatureValue(CommonUsages.eyesData, out var eyes), "Unable to TryGet feature value: eye data");
        yield return SkipFrame(DefaultFrameSkipCount);
        Assert.IsTrue(eyes.TryGetRightEyeRotation(out var rotation), "Couldn't get right eye rotation");
        Assert.IsTrue(eyes.TryGetLeftEyeRotation(out rotation), "Couldn't get left eye rotation");
    }

    [Ignore("Unsure of what devices for this API are supported")]
    [UnityTest]
    public IEnumerator VerifyXrInputTracking_EyeDataOpenAmount()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.LeftEye, devices);
        device = devices.FirstOrDefault();
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.IsTrue(device.TryGetFeatureValue(CommonUsages.eyesData, out var eyes), "Unable to TryGet feature value: eye data");
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.IsTrue(eyes.TryGetLeftEyeOpenAmount(out var open), "Couldn't get right eye open amount");
        Assert.IsTrue(eyes.TryGetRightEyeOpenAmount(out open), "Couldn't get left eye open amount");
    }

    [Ignore("Unsure of what devices for this API are supported")]
    [UnityTest]
    public IEnumerator VerifyXrInputTracking_EyeDataFixationPoint()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.LeftEye, devices);
        device = devices.FirstOrDefault();
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.IsTrue(device.TryGetFeatureValue(CommonUsages.eyesData, out var eyes), "Unable to TryGet feature value: eye data");
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.IsTrue(eyes.TryGetFixationPoint(out var fixationPoint), "Couldn't get fixationPoint");
    }

    [UnityTest]
    public IEnumerator VerifyVerifyXrInputTracking_EyesParallelWithHead()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.IsTrue(EyesParallelWithHead(), "Eyes are not parallel with the head");
    }

    bool AnglesApproximatelyEqual(float a, float b)
    {
        var check = Mathf.Abs(a - b) < angleComparisonTolerance;
        return (check);
    }

    public bool EyesParallelWithHead()
    {
        bool eyesParallelWithHead;

        var camera = Camera.GetComponent<Camera>();
        var left = camera.GetStereoViewMatrix(UnityEngine.Camera.StereoscopicEye.Left);
        var right = camera.GetStereoViewMatrix(UnityEngine.Camera.StereoscopicEye.Right);

        var leftEyePos = left.inverse.MultiplyPoint(Vector3.zero);
        var rightEyePos = right.inverse.MultiplyPoint(Vector3.zero);

        var eyesDelta = (rightEyePos - leftEyePos).normalized;
        var rightDir = Camera.transform.right;
        var angle = Vector3.Angle(eyesDelta, rightDir);

        if (AnglesApproximatelyEqual(angle, 0f))
        {
            Debug.Log("Eyes Parallel is OK : " + angle);
            eyesParallelWithHead = true;
        }
        else
        {
            Debug.Log("Eye Parallel is BAD = " + angle);
            eyesParallelWithHead = false;
        }

        return eyesParallelWithHead;
    }

    private void InputTracking_nodeAdded(XRNodeState obj)
    {
        Debug.Log("Node Added : " + obj.nodeType);
    }

    private void InputTracking_trackingAcquired(XRNodeState obj)
    {
        Debug.Log("Tracking Acquired: " + obj.nodeType);
    }

    private void InputTracking_trackingLost(XRNodeState obj)
    {
        Debug.Log("Tracking Lost : " + obj.nodeType);
    }

    private void InputTracking_nodeRemoved(XRNodeState obj)
    {
        Debug.Log("Node Removed : " + obj.nodeType);
    }
}