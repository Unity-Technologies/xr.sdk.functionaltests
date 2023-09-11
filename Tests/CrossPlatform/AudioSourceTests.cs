using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AudioSourceTests : XrFunctionalTestBase
{
    private AudioSource audioSource;

    private readonly int audioPlaySkipFrameCountWait = 2;
    private readonly float audioTolerance = .01f;

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

    [UnityTest]
    public IEnumerator VerifyAudioSource_Adjust_Pitch()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");

        audioSource.pitch = 0f;
        Assert.AreEqual(0f, audioSource.pitch, "Pitch was not set to 0;");

        yield return SkipFrame(DefaultFrameSkipCount);

        var pitchAmount = 0f;

        for (var i = 0f; i < 10f; ++i)
        {
            pitchAmount = pitchAmount + 0.1f;

            // Act
            audioSource.pitch = pitchAmount;
            Debug.Log("Changing pitch amount : " + pitchAmount);

            yield return SkipFrame();

            // Assert
            Assert.True(Math.Abs(pitchAmount - audioSource.pitch) <= audioTolerance, "pitch failed to change to the desired level");
        }
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_Adjust_Doppler()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");

        audioSource.dopplerLevel = 0f;
        Assert.AreEqual(0f, audioSource.dopplerLevel, "DopplerLevel was not set to 0;");

        yield return SkipFrame(DefaultFrameSkipCount);

        var dopplerLevel = 0f;

        for (var i = 0f; i < 10f; ++i)
        {
            dopplerLevel += 0.1f;

            // Act
            audioSource.dopplerLevel = dopplerLevel;
            Debug.Log("Changing dopplerLevel amount : " + dopplerLevel);

            yield return SkipFrame(DefaultFrameSkipCount);

            // Assert
            Assert.True(Math.Abs(dopplerLevel - audioSource.dopplerLevel) <= audioTolerance, "dopplerLevel failed to change to the desired level");
        }
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_Mute()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");

        audioSource.mute = true;
        yield return SkipFrame(audioPlaySkipFrameCountWait);
        Debug.Log("Mute");
        Assert.True(audioSource.mute, "Audio didn't mute");

        audioSource.mute = false;
        yield return SkipFrame(audioPlaySkipFrameCountWait);
        Debug.Log("Un-Mute");
        Assert.IsFalse(audioSource.mute, "Audio didn't un-mute");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_MinDistance()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");


        audioSource.minDistance = 0f;
        Assert.AreEqual(0f, audioSource.minDistance, "minDistance was not set to 0;");

        yield return SkipFrame(DefaultFrameSkipCount);

        var minDistance = 0f;

        for (var i = 0f; i < 10f; ++i)
        {
            minDistance = minDistance + 0.1f;

            // Act
            audioSource.minDistance = minDistance;
            Debug.Log("Changing minDistance amount : " + minDistance);

            yield return SkipFrame();

            // Assert
            Assert.True(Math.Abs(minDistance - audioSource.minDistance) <= audioTolerance,
                "minDistance failed to change to the desired level");
        }
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_MaxDistance()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");
        
        audioSource.maxDistance = 1f;
        Assert.AreEqual(1f, audioSource.maxDistance, "maxDistance was not set to 0;");

        var maxDistance = 0f;

        for (var i = 0f; i < 500f; ++i)
        {
            maxDistance += 10f;

            // Act
            audioSource.maxDistance = maxDistance;
            Debug.Log("Changing maxDistance amount : " + maxDistance);

            yield return SkipFrame();

            // Assert
            Assert.True(Math.Abs(maxDistance - audioSource.maxDistance) <= audioTolerance,
                "maxDistance failed to change to the desired level");
        }
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_IgnoreListenerPause()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");

        audioSource.ignoreListenerPause = false;
        yield return SkipFrame(DefaultFrameSkipCount);
        Assert.IsFalse(audioSource.ignoreListenerPause, "ignoreListenerPause was not set to false;");

        audioSource.ignoreListenerPause = true;
        yield return SkipFrame(DefaultFrameSkipCount);
        Assert.IsTrue(audioSource.ignoreListenerPause, "ignoreListenerPause was not set to true;");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_RollOffMode()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");
        yield return SkipFrame(DefaultFrameSkipCount);

        foreach (AudioRolloffMode mode in Enum.GetValues(typeof(AudioRolloffMode)))
        {
            audioSource.rolloffMode = mode;
            yield return SkipFrame(DefaultFrameSkipCount);
            Assert.AreEqual(audioSource.rolloffMode, mode, "Didn't correctly set the roll off Mode");
        }
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_AudioVelocityUpdateMode()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");
        yield return SkipFrame(DefaultFrameSkipCount);

        foreach (AudioVelocityUpdateMode mode in Enum.GetValues(typeof(AudioVelocityUpdateMode)))
        {
            audioSource.velocityUpdateMode = mode;
            Assert.AreEqual(audioSource.velocityUpdateMode, mode, "Didn't correctly set the AudioVelocityUpdateMode");
        }
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_IgnoreListenerVolume()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.ignoreListenerVolume = false;

        yield return SkipFrame(DefaultFrameSkipCount);
        Assert.IsFalse(audioSource.ignoreListenerVolume, "ignoreListenerVolume was not set to false;");

        audioSource.ignoreListenerPause = true;
        yield return SkipFrame(DefaultFrameSkipCount);
        Assert.IsTrue(audioSource.ignoreListenerPause, "ignoreListenerVolume was not set to true;");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_PanStereo()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.panStereo = 0f;
        Assert.AreEqual(0f, audioSource.panStereo, "panStereo was not set to 0;");

        yield return SkipFrame(DefaultFrameSkipCount);

        var panStereo = 0f;

        for (var i = 0f; i < 10f; ++i)
        {
            panStereo += 0.1f;

            // Act
            audioSource.panStereo = panStereo;
            Debug.Log("Changing maxDistance amount : " + panStereo);

            yield return SkipFrame();

            // Assert
            Assert.True(Math.Abs(panStereo - audioSource.panStereo) <= audioTolerance,
                "maxDistance failed to change to the desired level");
        }
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_Priority()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.priority = 0;
        Assert.AreEqual(0, audioSource.priority, "priority was not set to 0;");

        yield return SkipFrame(DefaultFrameSkipCount);

        var priority = 0;

        for (var i = 0f; i < 10f; ++i)
        {
            priority += 1;

            // Act
            audioSource.priority = priority;
            Debug.Log("Changing priority amount : " + priority);

            yield return SkipFrame();

            // Assert
            Assert.True(Math.Abs(priority - audioSource.priority) <= audioTolerance,
                "priority failed to change to the desired level");
        }
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_BypassEffects()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.bypassEffects = true;
        Assert.IsTrue(audioSource.bypassEffects, "bypassEffects was not set to true;");

        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.bypassEffects = false;
        Assert.IsFalse(audioSource.bypassEffects, "bypassEffects was not set to false;");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_BypassListenerEffects()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.bypassListenerEffects = true;
        Assert.IsTrue(audioSource.bypassListenerEffects, "bypassListenerEffects was not set to true;");

        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.bypassListenerEffects = false;
        Assert.IsFalse(audioSource.bypassListenerEffects, "bypassListenerEffects was not set to false;");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_BypassReverbZones()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.bypassReverbZones = true;
        Assert.IsTrue(audioSource.bypassReverbZones, "bypassReverbZones was not set to true;");

        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.bypassReverbZones = false;
        Assert.IsFalse(audioSource.bypassReverbZones, "bypassReverbZones was not set to false;");
    }

    [UnityTest]
    public IEnumerator VerifyAudioSource_AudioSourceCheck()
    {
        // Arrange
        yield return SkipFrame(DefaultFrameSkipCount);

        audioSource.Play();
        Debug.Log("Starting Audio");
        yield return SkipFrame(DefaultFrameSkipCount);

        var clip  = audioSource.clip;
        Assert.AreEqual(audioSource.clip.name, "FOA_speech_ambiX", "bypassReverbZones was not set to true;");
    }
}
