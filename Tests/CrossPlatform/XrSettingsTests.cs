using System;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools;
using UnityEngine.XR;
using static IXRDisplayInterface;
#if XR_SDK
using UnityEngine.XR.Management;
#endif //XR_SDK

public class XrSettingsTests : XrFunctionalTestBase
{
    [TearDown]
    public override void TearDown()
    {
        XRSettings.eyeTextureResolutionScale = 1f;
        XRDevice.fovZoomFactor = 1f;
        XRSettings.renderViewportScale = 1f;

        base.TearDown();
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
        Assert.IsNotEmpty(XRSettings.loadedDeviceName,
            $"Expected {XRSettings.loadedDeviceName} to be a non-empty string, but it is empty.");
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

    [TargetXrDisplays(exclude = new[] { "PSVR2 Display" })]
    [Test]
    public void VerifyXrSettings_RenderViewportScale_GreaterThan0()
    {
        Assert.IsTrue(XRSettings.renderViewportScale > 0f);
    }

    //[Ignore("Currently broken in 2019.3/staging as the fix in trunk has yet to be backported. https://ono.unity3d.com/unity/unity/pull-request/94692/_/xr/sdk/display/apis")]
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
#endif //!XR_SDK

    [Test]
    public void VerifyXrSettings_GameViewRenderMode()
    {
        foreach (GameViewRenderMode mode in Enum.GetValues(typeof(GameViewRenderMode)))
        {
            var renderMode = XRSettings.gameViewRenderMode = mode;
            Assert.AreEqual(renderMode, mode, "Didn't correctly set the game view render mode");
        }
    }

    [UnityTest]
    public IEnumerator VerifyXrSettings_DeviceEyeTextureDimension()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

        var dimension = XRSettings.deviceEyeTextureDimension;

        Assert.AreNotEqual(dimension, TextureDimension.Unknown, "deviceEyeTextureDimension is Unknown");
    }

    [ConditonalAssembly(exclude = new[] { "Unity.XR.MetaOpenXR" })]
    [UnityTest]
    public IEnumerator VerifyXrSettings_AdjustRenderViewportScale()
    {
        AssertNotUsingEmulation();
        yield return new WaitForSeconds(1f);
        var tolerance = .005;

        // Arrange
        var expRenderViewPortScale = 1f;
        // Act
        XRSettings.renderViewportScale = expRenderViewPortScale;
        // Assert
        var actRenderViewPortScale = XRSettings.renderViewportScale;
        Assert.AreEqual(
            expRenderViewPortScale,
            actRenderViewPortScale,
            tolerance,
            string.Format("Expected XRSettings.renderViewPortScale to {0}, but is {1}", expRenderViewPortScale,
                actRenderViewPortScale));

        yield return new WaitForSeconds(1f);

        // Arrange
        expRenderViewPortScale = 0.7f;
        // Act
        XRSettings.renderViewportScale = expRenderViewPortScale;
        // Assert
        actRenderViewPortScale = XRSettings.renderViewportScale;
        Assert.AreEqual(
            expRenderViewPortScale,
            actRenderViewPortScale,
            tolerance,
            string.Format("Expected XRSettings.renderViewPortScale to {0}, but is {1}", expRenderViewPortScale,
                actRenderViewPortScale));

        yield return new WaitForSeconds(1f);

        // Arrange
        expRenderViewPortScale = 0.5f;
        // Act
        XRSettings.renderViewportScale = expRenderViewPortScale;
        // Assert
        actRenderViewPortScale = XRSettings.renderViewportScale;
        Assert.AreEqual(
            expRenderViewPortScale,
            actRenderViewPortScale,
            tolerance,
            string.Format("Expected XRSettings.renderViewPortScale to {0}, but is {1}", expRenderViewPortScale,
                actRenderViewPortScale));
    }
    
    [ConditionalAssembly(exclude = new[] { "Unity.XR.MetaOpenXR" })]
    [UnityTest]
    public IEnumerator VerifyXrSettings_AdjustEyeTextureResolutionScale()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

        var scale = 0.1f;
        var scaleIncrement = 0.1f;
        var scaleLimit = 2f;

        do
        {
            scale += scaleIncrement;

            // I guess because of float math, incrementing the scale was also adding just a little bit more 
            // when 1.0 was reached the value was 1.000000012 and it was too much for the AreEqual assert.
            // so round off the extra bit.
            scale = (float)Math.Round((double)scale, 3);

            XRSettings.eyeTextureResolutionScale = scale;

            yield return new WaitForSeconds(1f);

            Debug.Log("VerifyAdjustEyeTextureResolutionScale = " + scale);
            Assert.AreEqual(scale, XRSettings.eyeTextureResolutionScale,
                "Eye texture resolution scale is not being respected");
        } while (scale < scaleLimit);
    }
}


