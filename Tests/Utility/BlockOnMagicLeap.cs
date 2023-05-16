using System;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using UnityEngine.XR;

[AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
public class BlockOnMagicLeap : NUnitAttribute, IApplyToTest
{

    public void ApplyToTest(Test test)
    {
        List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances(displays);

        foreach (XRDisplaySubsystem display in displays)
        {
            if (display.SubsystemDescriptor.id == "MagicLeap-Display" && display.running)
            {
                test.RunState = RunState.Skipped;
                test.Properties.Add("_SKIPREASON", "Skipping Test: Blocked from running on Magic Leap devices.");
                return;
            }
        }
    }

}