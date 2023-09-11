using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using static IXRDisplayInterface;
using static IAssemblyInterface;
#if XR_SDK
using UnityEngine.XR.Management;
#endif //XR_SDK

public class XrDeviceTests : XrFunctionalTestBase
{
    private string device;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
    }

    [TearDown]
    public override void TearDown()
    {
        XRDevice.fovZoomFactor = 1f;
        base.TearDown();
    }

    [ConditionalAssembly(exclude = new[] { "Unity.XR.Management" })]
    [UnityTest]
    public IEnumerator VerifyXRDevice_GetCurrentTrackingSpace()
    {
        yield return SkipFrame(2);

#if MOCKHMD_SDK
        var trackingSpace = XRDevice.GetTrackingSpaceType();
        Assert.IsNotNull(trackingSpace, "Tracking space is not reading correctly");
#else
        var inputsystems = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>();
        var originMode = inputsystems.GetTrackingOriginMode();
        Assert.AreNotEqual(TrackingOriginModeFlags.Unknown, originMode, "Tracking space is not reading correctly");
#endif // MOCKHMD_SDK
    }

    [Test]
    public void VerifyXrDevice_XrModelSupported()
    {
        AssertNotUsingEmulation();
        Assert.IsFalse(SystemInfo.unsupportedIdentifier.Equals(SystemInfo.deviceModel),
            $"Expected {SystemInfo.deviceModel} to be a supported device but it is not.");
    }

    [Test]
    public void VerifyXrDevice_NativePtrIsNotEmpty()
    {
        var ptr = XRDevice.GetNativePtr().ToString();
        Assert.IsNotEmpty(ptr, "Native Ptr is empty");
    }

    [Test]
    [TargetXrDisplays(exclude = new[] { "MagicLeap-Display" })]
    [ConditionalAssembly(exclude = new[] { "Unity.XR.MockHMD"})]
    public void VerifyXrDevice_RefreshRateGreaterThan0()
    {
        var wmrHmd = "WMRXRSDK";
        if (Settings.EnabledXrTarget == wmrHmd || Application.isEditor)
        {
            var reasonString = Application.isEditor
                ? "Test is running in the Editor"
                : $"EnabledXrTarget == {Settings.EnabledXrTarget}";

            Assert.Ignore("{0}: XRDevice.refreshRate will always be 0. Ignoring", reasonString);
        }
        else
        {
            AssertNotUsingEmulation();
            Assert.True(XRDevice.refreshRate > 0, "Expected XRDevice.refreshRate > 0, but is {0}",
                XRDevice.refreshRate);
        }
    }

    [Test]
    public void VerifyXrDevice_IsMobilePlatform()
    {
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            Assert.IsTrue(Application.isMobilePlatform, "Expected Application.isMobilePlatform == true, but is false ");
        else
            Assert.IsFalse(Application.isMobilePlatform, "Expected Application.isMobilePlatform == false, but is true ");
    }

    [UnityTest]
    [ConditionalAssembly(exclude = new[] { "Unity.XR.MockHMD", "Unity.XR.WindowsMR"})]
    [TargetXrDisplays(exclude = new[] { "MagicLeap-Display" })]
    public IEnumerator VerifyXrDevice_RefreshRate()
    {
        AssertNotUsingEmulation();
        yield return SkipFrame(DefaultFrameSkipCount);

        var refreshRate = XRDevice.refreshRate;


#if PLATFORM_IOS || PLATFORM_ANDROID || (UNITY_METRO && UNITY_EDITOR) || UNITY_WSA
        Assert.GreaterOrEqual(refreshRate, 60, "Refresh rate returned to lower than expected");
#else
        Assert.GreaterOrEqual(refreshRate, 89, "Refresh rate returned to lower than expected");
#endif
    }

    [UnityTest]
    public IEnumerator VerifyXrDevice_AdjustDeviceZoom()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

        var zoomAmount = 0f;
        var zoomCount = 0f;

        for (var i = 0; i < 2; i++)
        {
            zoomAmount += 1f;
            zoomCount += 1f;

            XRDevice.fovZoomFactor = zoomAmount;

            yield return SkipFrame(DefaultFrameSkipCount);

            Debug.Log("fovZoomFactor = " + zoomAmount);
            Assert.AreEqual(zoomCount, XRDevice.fovZoomFactor, "Zoom Factor is not being respected");
        }
    }

    [UnityTest]
    public IEnumerator VerifyXrDevice_AdjustAutoXRCameraTracking()
    {
        var cam = base.Camera.GetComponent<Camera>();
        XRDevice.DisableAutoXRCameraTracking(cam, true);
        yield return SkipFrame(DefaultFrameSkipCount);
    }
}
