#if URP_GRAPHICS
using System;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Object = UnityEngine.Object;
#if !PLATFORM_IOS && !PLATFORM_ANDROID
using System.IO;
#endif

public class URPAPITests : XrFunctionalTestBase
{
 
    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
    }

    [TearDown]
    public override void TearDown()
    {
        var rpAsset = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;
        var urpAsset = (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)rpAsset;
        urpAsset.renderScale = 1f;
        base.TearDown();
    }
    
    [ConditionalAssembly(exclude = new[] { "Unity.XR.MetaOpenXR" })]
    [UnityTest] 
    public IEnumerator VerifyURPAPI_AdjustRenderScale()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

        var scale = 0.1f;
        var scaleIncrement = 0.1f;
        var scaleLimit = 2f;

        var rpAsset = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;
        var urpAsset = (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)rpAsset;

        do
        {
            scale += scaleIncrement;

            // I guess because of float math, incrementing the scale was also adding just a little bit more 
            // when 1.0 was reached the value was 1.000000012 and it was too much for the AreEqual assert.
            // so round off the extra bit.
            scale = (float)Math.Round((double)scale, 3);

            urpAsset.renderScale = scale;

            yield return new WaitForSeconds(1f);

            Debug.Log("VerifyRenderScale = " + scale);
            Assert.AreEqual(scale, urpAsset.renderScale,
                "Render Resolution scale is not being respected");
        } while (scale < scaleLimit);
    }
}
#endif