using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.XR;

public interface IXRDisplayInterface
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class TargetXrDisplays :NUnitAttribute, IApplyToTest
    {
        
        public string[] include { get; set; }
        public string[] exclude { get; set; }
        private string skippedReason;
        private List<XRDisplaySubsystem> displays { get; set; }

        public TargetXrDisplays()
        {
            include = new List<string>().ToArray();
            exclude = new List<string>().ToArray();
        }

        public TargetXrDisplays(params string[] include)
        {
            this.include = include;
        }

        public void ApplyToTest(Test test)
        {
            displays = new List<XRDisplaySubsystem>();
            SubsystemManager.GetSubsystems(displays);

            foreach (var display in displays)
            {
                if (VerifyIsXrDisplaySupported(display.subsystemDescriptor.id) && display.running)
                {
                    return;
                }

                test.RunState = RunState.Skipped;
                test.Properties.Add(PropertyNames.SkipReason, skippedReason);
            }
        }

        internal bool VerifyIsXrDisplaySupported(string testTargetXrDisplay)
        {
            if (include.Any() && !include.Any(x => x == testTargetXrDisplay))
            {
                skippedReason = string.Format("Only supported on {0}", string.Join(", ", include.Select(x => x.ToString()).ToArray()));
                return false;
            }

            if (exclude.Any(x => x == testTargetXrDisplay))
            {
                skippedReason = string.Format("Test not supported on  {0}", string.Join(", ", exclude.Select(x => x.ToString()).ToArray()));
                return false;
            }
            return true;
        }
    }
}
