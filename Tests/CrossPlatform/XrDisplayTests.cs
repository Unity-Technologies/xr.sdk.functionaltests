using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using static IXRDisplayInterface;
using static IAssemblyInterface;

public class XrDisplayTests : XrFunctionalTestBase
{
    List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();
    private bool focusChangedDetected = false;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        SubsystemManager.GetInstances(displays);
        displays[0].displayFocusChanged += XrDisplayTests_displayFocusChanged;
    }

    private void XrDisplayTests_displayFocusChanged(bool obj)
    {
        focusChangedDetected = true;
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();
        displays[0].displayFocusChanged -= XrDisplayTests_displayFocusChanged;
        displays[0].scaleOfAllViewports = 1f;
        displays[0].scaleOfAllRenderTargets = 1f;
        displays[0].occlusionMaskScale = 1f;
        
    }

    [UnityTest]
    public IEnumerator VerifyXrDisplay_Running()
    {
        yield return SkipFrame(DefaultFrameSkipCount);
        Assert.IsTrue(displays[0].running);
    }

    [UnityPlatform(exclude = new[] { RuntimePlatform.WindowsEditor })]
    [Test]
    public void VerifyXrDisplay_IsPresent()
    {
#if XR_SDK
        AssertNotUsingEmulation();
        Assert.IsTrue(displays.Count > 0, "XR Device is not present");
#else
        Assert.IsTrue(XRDevice.isPresent, "XR Device is not present");
#endif //XR_SDK
    }

    [UnityTest]
    [TargetXrDisplays(exclude = new []{"MagicLeap-Display"})]
    public IEnumerator VerifyXrDisplay_SubsystemRefreshRate()
    {
        AssertNotUsingEmulation();
        yield return SkipFrame(DefaultFrameSkipCount);
        
        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        float rate = 0;
        var gotrate = displays[0].TryGetDisplayRefreshRate(out rate);

#if MOCKHMD_SDK || WMR_SDK
        Assert.Ignore("{0}: TryGetDisplayRefreshRate will always be 0. Ignoring", "Platform = MOCKHMD_SDK || WMR_SDK");
#elif PLATFORM_IOS || PLATFORM_ANDROID || (UNITY_METRO && UNITY_EDITOR) || UNITY_WSA
        Assert.IsTrue(gotrate);
        Assert.GreaterOrEqual(rate, 60, "Refresh rate returned to lower than expected");
#else
        Assert.IsTrue(gotrate);
#if OPENXR_SDK
        if(OpenXRUtilities.IsRunningMockRuntime())
        {
            Assert.GreaterOrEqual(rate, 59, "Refresh rate returned to lower than expected");
        }else{
            Assert.GreaterOrEqual(rate, 89, "Refresh rate returned to lower than expected");
        }
#else
        Assert.GreaterOrEqual(rate, 89, "Refresh rate returned to lower than expected");
#endif
#endif
    }

    [ConditionalAssembly(exclude = new[] { "Unity.XR.MockHMD", "Microsoft.MixedReality.OpenXR" })]
    [UnityTest]
    public IEnumerator VerifyXrDisplay_GetAppGPUTime()
    {
        AssertNotUsingEmulation();
        AssertNotUsingAcerHMD();
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        Assert.IsTrue(displays[0].TryGetAppGPUTimeLastFrame(out var frame));
    }

    [ConditionalAssembly(exclude = new[] { "Unity.XR.MockHMD", "Microsoft.MixedReality.OpenXR" })]
    [UnityTest]
    public IEnumerator VerifyXrDisplay_GetCompositorGPUTime()
    {
        AssertNotUsingEmulation();
        AssertNotUsingAcerHMD();
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        Assert.IsTrue(displays[0].TryGetCompositorGPUTimeLastFrame(out var frame));
    }
    
    [UnityTest]
    public IEnumerator VerifyXrDisplay_GetDroppedFrameCount()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        var frame = -1;
        displays[0].TryGetDroppedFrameCount(out frame);

        Assert.AreNotEqual(-1, frame, "Failed to call TryGetDroppedFrameCount");
    }

    [UnityTest]
    public IEnumerator VerifyXrDisplay_GetFramePresentCount()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        var frame = -1;
        displays[0].TryGetFramePresentCount(out frame);

        Assert.AreNotEqual(-1, frame, "Failed to call TryGetFramePresentCount");
    }

    [ConditionalAssembly(exclude = new[] { "Unity.XR.MockHMD", "Microsoft.MixedReality.OpenXR" })]
    [UnityTest]
    public IEnumerator VerifyXrDisplay_GetMotionToPhoton()
    {
        AssertNotUsingEmulation();
        AssertNotUsingAcerHMD();
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        Assert.IsTrue(displays[0].TryGetMotionToPhoton(out var frame));
    }

    [UnityTest]
    public IEnumerator VerifyXrDisplay_SetFocusPlane()
    {
        //AssertNotUsingEmulation();
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        var display = displays[0];

        try
        {
            display.SetFocusPlane(Vector3.one, Vector3.forward, Vector3.zero);
        }
        catch (Exception e)
        {
            Assert.Fail("Failed to change the focus point for the frame");
        }
    }

    [UnityTest]
    public IEnumerator VerifyXrDisplay_GetRenderTextureForRenderPass()
    {
        //AssertNotUsingEmulation();
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        var display = displays[0];
        yield return SkipFrame(DefaultFrameSkipCount);

        Assert.IsNotNull(display.GetRenderTextureForRenderPass(0), "Failed to get Render Texture from Render Pass 0");
    }

    [UnityTest]
    public IEnumerator VerifyXrDisplay_MirrorBlitMode()
    {
        //AssertNotUsingEmulation();
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        var display = displays[0];
        
        Assert.IsNotNull(display.GetPreferredMirrorBlitMode(), "Failed to get preferred Blit Mode");

        foreach (var mode in typeof(XRMirrorViewBlitMode).GetProperties())
        {
            display.SetPreferredMirrorBlitMode(mode.MetadataToken);
            Assert.AreEqual(display.GetPreferredMirrorBlitMode(), mode, "Mirror Blit mode was not set");
        }
    }

    [UnityTest]
    public IEnumerator VerifyXrDisplay_RenderPass()
    {
        //AssertNotUsingEmulation();
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        var display = displays[0];

        var count = display.GetRenderPassCount();
        for (int i = 0; i > count; i++)
        {
            display.GetRenderPass(0, out var renderPass);
            Assert.IsNotNull(renderPass, $"Didn't get the renderPass on index {count}");
        }
    }

    [UnityTest]
    public IEnumerator VerifyXrDisplay_CullingParameters()
    {
        //AssertNotUsingEmulation();
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        var display = displays[0];
        var camera = GameObject.FindObjectOfType<Camera>();
        display.GetCullingParameters(camera, 0, out var cullingParameters);

        Assert.IsNotNull(cullingParameters, "Failed to get Culling Parameters");
    }

    [UnityTest]
    public IEnumerator VerifyXrDisplay_GetRenderTexture()
    {
        //AssertNotUsingEmulation();
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        var display = displays[0];

        var count = display.GetRenderPassCount();
        for (int i = 0; i > count; i++)
        {
            var renderTexture = display.GetRenderTextureForRenderPass(i);
            Assert.IsTrue(renderTexture.IsCreated(), $"Didn't get the render texture on render pass {count}");
        }
    }

    [UnityTest]
    public IEnumerator VerifyXrDisplay_zFarNear()
    {
        //AssertNotUsingEmulation();
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        var display = displays[0];

        for (int i = 0; i < 10f; i++)
        {
            display.zFar = i;
            Assert.AreEqual(display.zFar, i, "z Far was not set correctly");
        }

        for (int o = 0; o < 5f; o++)
        {
            display.zNear = o;
            Assert.AreEqual(display.zNear, o, "z Near was not set correctly");
        }
    }
    
    [Ignore("Can't set the bool for content protection -> bug?")]
    [UnityTest]
    public IEnumerator VerifyXrDisplay_ContentProtection()
    {
        //AssertNotUsingEmulation();
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        var display = displays[0];

        display.contentProtectionEnabled = true;
        yield return SkipFrame(60);
        Assert.IsTrue(display.contentProtectionEnabled, "Didn't set the content protection correctly");

        display.contentProtectionEnabled = false;
        yield return SkipFrame(60);
        Assert.IsFalse(display.contentProtectionEnabled, "Didn't set the content protection correctly");
    }

    [UnityPlatform(exclude = new[] { Unity.XR.MetaOpenXR })]
    [UnityTest]
    public IEnumerator VerifyXrDisplay_OcclusionMaskScale()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        var display = displays[0];

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
            
            display.occlusionMaskScale = scale;

            yield return SkipFrame(DefaultFrameSkipCount);

            Debug.Log("VerifyXrDisplay_OcclusionMaskScale = " + scale);
            Assert.AreEqual(scale, display.occlusionMaskScale, "Occlusion mask scale scale is not being respected");
        } while (scale < scaleLimit);
    }

    [UnityPlatform(exclude = new[] { Unity.XR.MetaOpenXR })]
    [UnityTest]
    public IEnumerator VerifyXrDisplay_ScaleOfAllViewports()
    {
        AssertNotUsingEmulation();
        yield return new WaitForSeconds(1f);

         SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        var display = displays[0];
        var tolerance = .005;

        // Arrange
        var expRenderViewPortScale = 1f;
        // Act
        display.scaleOfAllViewports = expRenderViewPortScale;
        // Assert
        var actRenderViewPortScale = display.scaleOfAllViewports;
        Assert.AreEqual(
            expRenderViewPortScale,
            actRenderViewPortScale,
            tolerance,
            string.Format("Expected display.scaleOfAllViewports to {0}, but is {1}", expRenderViewPortScale,
                actRenderViewPortScale));

        yield return new WaitForSeconds(1f);

        // Arrange
        expRenderViewPortScale = 0.7f;
        // Act
        display.scaleOfAllViewports = expRenderViewPortScale;
        // Assert
        actRenderViewPortScale = display.scaleOfAllViewports;
        Assert.AreEqual(
            expRenderViewPortScale,
            actRenderViewPortScale,
            tolerance,
            string.Format("Expected display.scaleOfAllViewports to {0}, but is {1}", expRenderViewPortScale,
                actRenderViewPortScale));

        yield return new WaitForSeconds(1f);

        // Arrange
        expRenderViewPortScale = 0.5f;
        // Act
        display.scaleOfAllViewports = expRenderViewPortScale;
        // Assert
        actRenderViewPortScale = display.scaleOfAllViewports;
        Assert.AreEqual(
            expRenderViewPortScale,
            actRenderViewPortScale,
            tolerance,
            string.Format("Expected display.scaleOfAllViewports to {0}, but is {1}", expRenderViewPortScale,
                actRenderViewPortScale));
    }


    [UnityPlatform(exclude = new[] { Unity.XR.MetaOpenXR })]
    [UnityTest]
    
    public IEnumerator VerifyXrDisplay__ScaleOfAllRenderTargets()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0)
            Assert.Ignore("Couldn't find XRDisplay Subsystem");

        var display = displays[0];

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

            display.scaleOfAllRenderTargets = scale;


            yield return new WaitForSeconds(1f);

            Debug.Log("VerifyScaleOfAllRenderTargets = " + scale);
            Assert.AreEqual(scale, display.scaleOfAllRenderTargets,
                "Eye texture resolution scale is not being respected");
        } while (scale < scaleLimit);
    }
}
