using System;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Object = UnityEngine.Object;
#if !PLATFORM_IOS && !PLATFORM_ANDROID
using System.IO;
#endif

public class RenderingChecks : XrFunctionalTestBase
{
    enum RenderingStates
    {
        MSAA_and_HDR = 0,
        MSAA,
        HDR,
        No_MSAA_and_HDR
    }

    private string fileName;

    private bool runningRenderingTest = false;
    private bool allTestsPassed = false;
    private RenderingStates currentRenderingState;
    private Texture2D mobileTexture;
    private GameObject colorScreen;
    private Material testMat;
    private RenderTexture renderTexture;
    private Camera camera;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        camera = Camera.GetComponent<Camera>();
        testMat = new Material(Resources.Load("Materials/YFlipColorMesh", typeof(Material)) as Material);
        currentRenderingState = RenderingStates.MSAA_and_HDR;
        renderTexture = new RenderTexture(256, 256, 16);
        colorScreen = GameObject.CreatePrimitive(PrimitiveType.Quad);
        colorScreen.GetComponent<Renderer>().material = testMat;
    }

    [TearDown]
    public override void TearDown()
    {
        camera.targetTexture = null;
        RenderTexture.active = null;
        Object.Destroy(renderTexture);
        GameObject.Destroy(colorScreen);
        base.TearDown();
    }

    [UnityTest]
    public IEnumerator VerifyRendering_ParticleSmokeTest()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(0, 0, 10);
        ParticleSystem particles = go.AddComponent<ParticleSystem>();
        go.GetComponent<ParticleSystemRenderer>().material = Resources.Load<Material>("Materials/Particle");
        Assert.IsNotNull(go.GetComponent<ParticleSystemRenderer>().material);
        particles.Play();
        yield return new WaitForSeconds(2);
        particles.Stop();
        GameObject.Destroy(go);
        yield return new WaitForSeconds(1);
    }

    [UnityTest]
    public IEnumerator VerifyTakeScreenShot()
    {
        yield return SkipFrame(2);

        try
        {
#if PLATFORM_IOS || PLATFORM_ANDROID
            var cam = Camera;
            var width = cam.GetComponent<Camera>().scaledPixelWidth;
            var height = cam.GetComponent<Camera>().scaledPixelHeight;

            mobileTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            mobileTexture = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);

#else
            fileName = Application.temporaryCachePath + "/ScreenShotTest.jpg";
            ScreenCapture.CaptureScreenshot(fileName, ScreenCapture.StereoScreenCaptureMode.BothEyes);
#endif
        }
        catch (Exception e)
        {
            Debug.Log("Failed to get capture! : " + e.Message);
            Assert.Fail("Failed to get capture! : " + e.Message);
        }

        yield return SkipFrame(5);

#if PLATFORM_IOS || PLATFORM_ANDROID


        Assert.IsNotNull(mobileTexture, "Texture data is empty for mobile");
#else
        var tex = new Texture2D(2, 2);

        var texData = File.ReadAllBytes(fileName);
        Debug.Log("Screen Shot Success!" + Environment.NewLine + "File Name = " + fileName);

        tex.LoadImage(texData);

        Assert.IsNotNull(tex, "Texture Data is empty");
#endif
    }

    [UnityTest]
    public IEnumerator VerifyRendering_YWorldCoordinateAllRenderingStates()
    {
        // Setup the color screen in front of the camera 
        var rotation = camera.transform.rotation;
        var distance = 0.5f;
        var position = camera.transform.position + (rotation * Vector3.forward * distance);
        colorScreen.transform.position = position;
        colorScreen.transform.rotation = rotation;

        while (!runningRenderingTest)
        {
            SwitchRenderingStates();
            yield return SkipFrame(1);
        }
    }

    void SwitchRenderingStates()
    {
        switch (currentRenderingState)
        {
            case RenderingStates.MSAA_and_HDR:
                camera.allowHDR = true;
                camera.allowMSAA = true;
                Debug.Log("MSAA AND HDR");
                VerifyYWorldCoordinate();
                break;
            case RenderingStates.MSAA:
                camera.allowHDR = false;
                camera.allowMSAA = true;
                Debug.Log("MSAA");
                VerifyYWorldCoordinate();
                break;
            case RenderingStates.HDR:
                camera.allowHDR = true;
                camera.allowMSAA = false;
                Debug.Log("HDR");
                VerifyYWorldCoordinate();
                break;
            case RenderingStates.No_MSAA_and_HDR:
                camera.allowHDR = false;
                camera.allowMSAA = false;
                Debug.Log("NO MSAA and NO HDR");
                VerifyYWorldCoordinate();
                break;
        }

        currentRenderingState +=  1;
        if ((int)currentRenderingState < System.Enum.GetValues(typeof(RenderingStates)).Length) return;
        runningRenderingTest = true;

        Debug.Log(allTestsPassed ? "The y-flip test passed successfully!" : "The y-flip test failed!");
    }

    private void VerifyYWorldCoordinate()
    {
        camera.targetTexture = renderTexture;
        camera.Render();
        RenderTexture.active = renderTexture;

        var dst = new RenderTexture(renderTexture.width, renderTexture.height, renderTexture.depth);

        OnRenderImage(renderTexture, dst);
    }

    bool IsYOrientationCorrect(RenderTexture src)
    {
        var originalActiveRenderTexture = RenderTexture.active;

        RenderTexture.active = src;
        var tex = new Texture2D(src.width, src.height, TextureFormat.RGBA32, src.useMipMap, src.sRGB)
        {
            name = "Y Flip Test Texture"
        };
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();

        // We shouldn't sample directly from (0,0) because chances are that will overlap
        // the occlusion mesh.  Therefore we should try to sample closer to the center bottom of the texture.
        var x = src.width * 0.5f;
        var y = src.height * 0.3f;
        var color = tex.GetPixel((int)x, (int)y);
        tex = null;

        RenderTexture.active = originalActiveRenderTexture;

        // Texture coordinates start at lower left corner.  So (0,0) should be red.
        // https://docs.unity3d.com/ScriptReference/Texture2D.GetPixel.html
        return color == Color.red;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Assert.True(IsYOrientationCorrect(src), string.Format("The texture is y-flipped incorrectly for camera mode {0}", System.Enum.GetName(typeof(RenderingStates), currentRenderingState)));
        Graphics.Blit(src, dst);
    }
}
