#if !UNITY_ANDROID
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;


public class DllNativePluginTests : XrFunctionalTestBase
{
    private bool sceneObjectsLoaded;
    private bool renderingImage;

    private GameObject renderPlane;
    private GameObject baseSphere;

    private Light spotLight;

    private RenderTexture currentRenderTexture;

    private int nonPerformantFrameCount;

    // we have observed a drop in performance between simulation and runtime
    // on the device - in the editor, we've seen it fluctuate from 54-60 FPS
    // when the device runs just fine (also giving a little bit of elbow room
    // for when simulation tanks the frame rate a bit more than what we've seen)
    const float KFrameTimeMax = 1f / 52f;


    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        CreateObjectsFromPrefabs();
    }

    [TearDown]
    public override void TearDown()
    {
        Object.Destroy(renderPlane);
        Object.Destroy(baseSphere);
        Object.Destroy(spotLight.gameObject);

        if (GameObject.Find("Spotlight(Clone)"))
        {
            Object.Destroy(GameObject.Find("Spotlight(Clone)"));
        }
        base.TearDown();
    }

    public void Update()
    {
        if (Time.deltaTime > KFrameTimeMax)
            ++nonPerformantFrameCount;
    }

    private void CreateObjectsFromPrefabs()
    {
        renderPlane = Object.Instantiate(Resources.Load<GameObject>("Prefabs/_PlaneThatCallsIntoPlugin"));
        spotLight = Object.Instantiate(Resources.Load<Light>("Prefabs/Spotlight"));
        baseSphere = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Sphere"));

        var plane = GameObject.Find(renderPlane.name).scene.IsValid();
        var light = GameObject.Find(spotLight.name).scene.IsValid();
        var sphere = GameObject.Find(baseSphere.name).scene.IsValid();

        if(plane && light && sphere)
            sceneObjectsLoaded = true;
    }
    
    [Test]
    public void VerifyDLLPlugin_SceneObjectsLoaded()
    {
        Assert.IsTrue(sceneObjectsLoaded, "Scene Objects was not created");
    }

    // NOTE: Skipping on WSA because cannot find RenderingPlugin for UWP
    [UnityPlatform(exclude = new[] {RuntimePlatform.WSAPlayerX64})]
    [UnityTest]
    public IEnumerator VerifyDLLPlugin_IsPlaneRendering()
    {
        yield return SkipFrame(1);
        Assert.IsTrue(IsPlaneRendering(), "Image rendering couldn't be found");
    }

    // TODO: what is this test checking?
    // NOTE: Skipping on WSA because cannot find RenderingPlugin for UWP
    [UnityPlatform(exclude = new[] {RuntimePlatform.WSAPlayerX64})]
    [UnityTest]
    public IEnumerator VerifyDLLPlugin_RenderingFps()
    {
        yield return SkipFrame(2);
        Assert.AreEqual(0, nonPerformantFrameCount, "Failed to keep every frame inside the target frame time for the tested window");
    }

    private bool IsPlaneRendering()
    {
        var filter = false;
        var textsize = false;

        var renderer = renderPlane.GetComponent<Renderer>();
        if (renderer.material.mainTexture.filterMode == FilterMode.Point)
        {
            filter = true;
        }

        // TODO pull these magic numbers (256) into readonly vars with description
        if(renderer.material.mainTexture.height == 256 && renderer.material.mainTexture.width == 256)
        {
            textsize = true;
        }

        return filter && textsize;
    }
}
#endif