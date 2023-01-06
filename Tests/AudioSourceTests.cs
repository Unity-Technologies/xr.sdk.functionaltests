using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if PLATFORM_PS5
using System.IO;
using System.Collections.Generic;
using System.Reflection;
#endif

public class AudioSourceTests : XrFunctionalTestBase
{
    private AudioSource audioSource;

    private readonly int audioPlaySkipFrameCountWait = 2;
    private readonly float audioTolerance = .01f;

#if PLATFORM_PS5
    // allow per-project amendments to the final build
    /*public class PS5EnableBootAudioOptions : UnityEditor.PS5.IPostProcessPS5
    {
        public void OnPostProcessPS5(string projectFolder, string outputFolder)
        {
            Console.WriteLine($"PS5EnableBootAudioOptions script. projectFolder:{projectFolder} outputFolder:{outputFolder}");
​
        // this way works with the new incremental build system.
        Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.PostprocessBuildPlayer").GetMethod("AddProjectBootConfigKeyValue", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { "audio-output-type", "AUDIOOUT2" });
​
		// Add custom lines to the boot.config file (pre inc-build system)... adding them to the end should allow you to override any earlier lines in the file
		var bootConfigPath = Path.Combine(outputFolder, "Data\\boot.config");
            Console.WriteLine($"checking for existence of boot config file at :{bootConfigPath}");
            if (File.Exists(bootConfigPath))
            {
                Console.WriteLine($"file found at :{bootConfigPath}");
                var lines = new List<string>(File.ReadAllLines(bootConfigPath));
                lines.Add("audio-output-type=AUDIOOUT2");
                File.WriteAllLines(bootConfigPath, lines.ToArray());
            }
            else
            {
                Console.WriteLine($"file not found at :{bootConfigPath}");
            }
        }
        public int callbackOrder { get { return 1; } }
    }*/
​
#if UNITY_2022_1_OR_NEWER  // IPostProcessPS5PrepareStagingArea is not in 2021
    /*public class  PS5EnableBootAudioOptions2 : UnityEditor.PS5.IPostProcessPS5PrepareStagingArea
    {
        public bool OnPrepareStagingArea(string projectFolder, string outputFolder, UnityEditor.PS5.PS5BuildSubtarget buildSubtarget, UnityEditor.BuildOptions buildOptions)
        {
            Console.WriteLine($"PS5EnableBootAudioOptions OnPrepareStagingArea(). projectFolder:{projectFolder} outputFolder:{outputFolder}");
            return true;
        }
    ​
        public int callbackOrder { get { return 9999; } }
    }*/
#endif
#endif

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        TestCubeSetup(TestCubesConfig.TestCube);
        Cube.AddComponent<AudioSource>();
        Cube.GetComponent<AudioSource>().clip = Resources.Load("Audio/FOA_speech_ambiX", typeof(AudioClip)) as AudioClip;

        audioSource = Cube.GetComponent<AudioSource>();
        Camera.AddComponent<AudioListener>();
    }

    [TearDown]
    public override void TearDown()
    {
        GameObject.Destroy(audioSource);
        base.TearDown();
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_Play()
    {
        // Act
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        // Assert
        Assert.AreEqual(audioSource.isPlaying, true, "Audio source is not playing");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_Pause()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        // Act
        audioSource.Pause();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        // Assert
        Assert.AreEqual(audioSource.isPlaying, false, "Audio source is not paused");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_UnPause()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        audioSource.Pause();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        // Act
        audioSource.UnPause();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        // Assert
        Assert.AreEqual(audioSource.isPlaying, true, "Audio source didn't un-paused");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_Stop()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        yield return SkipFrame(audioPlaySkipFrameCountWait);

        // Act
        audioSource.Stop();
        yield return SkipFrame(DefaultFrameSkipCount);

        // Assert
        Assert.AreEqual(audioSource.isPlaying, false, "Audio failed to stop");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_Adjust_SpatialBlend()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);
        audioSource.spatialize = true;
        Debug.Log("Enabling Spatialized Audio");

        audioSource.Play();
        Debug.Log("Starting Audio");

        var blendAmount = 0f;

        for (var i = 0f; i < 10f; ++i)
        {
            blendAmount = blendAmount + 0.1f;

            // Act
            audioSource.spatialBlend = blendAmount;
            Debug.Log("Changing blend amount : " + blendAmount);

            yield return SkipFrame();

            // Assert
            Assert.True(Math.Abs(audioSource.spatialBlend - blendAmount) <= audioTolerance, "Spatial Blend failed to be set");
        }
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_Adjust_Volume()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");

        audioSource.volume = 0f;
        Assert.AreEqual(0f, audioSource.volume, "Volume was not set to 0;");

        yield return SkipFrame(DefaultFrameSkipCount);

        var volumeAmount = 0f;

        for (var i = 0f; i < 10f; ++i)
        {
            volumeAmount = volumeAmount + 0.1f;

            // Act
            audioSource.volume = volumeAmount;
            Debug.Log("Changing volume amount : " + volumeAmount);

            yield return SkipFrame();

            // Assert
            Assert.True(Math.Abs(volumeAmount - audioSource.volume) <= audioTolerance, "volume failed to change to the desired level");
        }
    }
}
