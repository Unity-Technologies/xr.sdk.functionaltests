using System.Collections;
using UnityEngine.XR;
using NUnit.Framework;
using UnityEngine.TestTools;
#if XR_SDK
using UnityEngine.XR.Management;
#endif //XR_SDK

public class TrackingSpaceTests : XrFunctionalTestBase
{
    [UnityTest]
    public IEnumerator VerifyXRDevice_GetCurrentTrackingSpace()
    {
        yield return SkipFrame(2);

#if !XR_SDK || MOCKHMD_SDK
        var trackingSpace = XRDevice.GetTrackingSpaceType();
        Assert.IsNotNull(trackingSpace, "Tracking space is not reading correctly");
#else
        var inputsystems = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>();
        var originMode = inputsystems.GetTrackingOriginMode();
        Assert.AreNotEqual(TrackingOriginModeFlags.Unknown, originMode, "Tracking space is not reading correctly");
#endif //!XR_SDK || MOCKHMD_SDK
    }
}
