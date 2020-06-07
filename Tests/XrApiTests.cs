﻿using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class XrApiTests : XrFunctionalTestBase
{
    [Test]
    public void VerifyApplication_IsMobilePlatform()
    {
#if PLATFORM_IOS || PLATFORM_ANDROID
        Assert.IsTrue(Application.isMobilePlatform, "Exptect Application.isMobilePlatform == true, but is false ");
#else
        Assert.IsFalse(Application.isMobilePlatform, "Exptect Application.isMobilePlatform == false, but is true ");
#endif
    }
    
    [Test]
    public void VerifyXrDevice_IsPresent()
    {
        List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances(displays);

        AssertNotUsingEmulation();
        Assert.IsTrue(displays.Count > 0, "XR Device is not present");
    }

    [UnityPlatform(exclude = new[] { RuntimePlatform.IPhonePlayer })]
    [Test]
    public void VerifyXRDevice_userPresence_isPresent()
    {
        var mockHmd = "MockHMD";

        if (Settings.EnabledXrTarget == mockHmd || Application.isEditor)
        {
            var reasonString = Settings.EnabledXrTarget == mockHmd ? $"EnabledXrTarget == {mockHmd}" : "Test is running in the Editor";

            Assert.Ignore("{0}: UserPresenceState.Present will always be false. Ignoring", reasonString);
        }
        else
        {
            var device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            Assert.IsTrue(device.isValid, "The userPresence is UnSupported on this device. Expected head device is InValid.");
#if UNITY_2019_3_OR_NEWER
            Assert.IsTrue(device.TryGetFeatureValue(CommonUsages.userPresence, out bool value), "The userPresence was not found or is Unknown on the head device");
#else
            Assert.IsTrue(device.TryGetFeatureValue(new InputFeatureUsage<bool>("UserPresence"), out bool value), "The userPresence is Unknown on the head device");
#endif
            Assert.IsTrue(value, "userPresence is Not Present on head device.");
        }
    }

    [Test]
    public void VerifyXrSettings_IsDeviceActive()
    {
        AssertNotUsingEmulation();
        Assert.IsTrue(XRSettings.isDeviceActive, $"Expected {XRSettings.isDeviceActive} to true, but is false.");
    }

    [Test]
    public void VerifyXrSettings_LoadedDeviceName()
    {
        AssertNotUsingEmulation();
        Assert.IsNotEmpty(XRSettings.loadedDeviceName, $"Expected {XRSettings.loadedDeviceName} to be a non-empty string, but it is empty.");
    }

    [Test]
    public void VerifyXrModelSupported()
    {
        AssertNotUsingEmulation();
        Assert.IsFalse(SystemInfo.unsupportedIdentifier.Equals(SystemInfo.deviceModel), $"Expected {SystemInfo.deviceModel} to be a supported device but it is not.");
    }

    [Test]
    public void VerifyXrDevice_NativePtr_IsNotEmpty()
    {
        var ptr = XRDevice.GetNativePtr().ToString();
        Assert.IsNotEmpty(ptr, "Native Ptr is empty");
    }

    [Test]
    public void VerifyRefreshRateGreaterThan0()
    {
        AssertNotUsingEmulation();
        Assert.True(XRDevice.refreshRate > 0, "Expected XRDevice.refreshRate > 0, but is {0}", XRDevice.refreshRate);
    }

    [Test]
    public void VerifyXrSettings_EyeTextureHeight_GreaterThan0()
    {
        Assert.IsTrue(XRSettings.eyeTextureHeight > 0f);
    }

    [Test]
    public void VerifyXrSettings_EyeTextureWidth_GreaterThan0()
    {
        Assert.IsTrue(XRSettings.eyeTextureWidth > 0f);
    }

    [Test]
    public void VerifyXrSettings_EyeTextureResolutionScale_GreaterThan0()
    {
        Assert.IsTrue(XRSettings.eyeTextureResolutionScale > 0f);
    }

    [Test]
    public void VerifyXrSettings_RenderViewportScale_GreaterThan0()
    {
        Assert.IsTrue(XRSettings.renderViewportScale > 0f);
    }

    [Ignore("Currently broken in 2019.3/staging as the fix in trunk has yet to be backported. https://ono.unity3d.com/unity/unity/pull-request/94692/_/xr/sdk/display/apis")]
    [Test]
    public void VerifyXrSettings_UseOcclusionMesh()
    {
        Assert.IsTrue(XRSettings.useOcclusionMesh);
    }

#if !XR_SDK

    [Test]
    public void XrApVerifyXrSettings_StereoRenderingMode()
    {
        if (Application.isEditor)
        {
            Assert.Ignore("StereoRenderMode test not valid in Editor.");
        }

        var expStereoRenderingMode = Settings.StereoRenderingMode;
        var actStereoRenderingMode = XRSettings.stereoRenderingMode.ToString();

        Assert.IsTrue(actStereoRenderingMode.Contains(expStereoRenderingMode), $"Expected StereoRenderMode to contain {expStereoRenderingMode} but it doesn't. Actual StereoRenderMode is: {actStereoRenderingMode}");
    }
#endif
}


