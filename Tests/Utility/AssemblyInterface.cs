using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using UnityEngine;

public interface IAssemblyInterface
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Method, AllowMultiple = true)]
    public class ConditionalAssembly : NUnitAttribute, IApplyToTest
    {
        public string[] include { get; set; }
        public string[] exclude { get; set; }
        private string skippedReason;

        public ConditionalAssembly()
        {
            include = new List<string>().ToArray();
            exclude = new List<string>().ToArray();
        }

        public void ApplyToTest(Test test)
        {
            var playerAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var playerAssembly in playerAssemblies)
            {
                var assembly = playerAssembly.FullName.Substring(0, playerAssembly.FullName.IndexOf(","));
                if (!IsAssemblySupported(assembly))
                {
                    test.RunState = RunState.Skipped;
                    test.Properties.Add(PropertyNames.SkipReason, skippedReason);
                }
            }
        }

        internal bool IsAssemblySupported(string assembly)
        {
            if (include.Any() && !include.Any(x => x == assembly))
            {
                skippedReason = string.Format("Only supported on {0}", string.Join(", ", include.Select(x => x.ToString()).ToArray()));
                return false;
            }

            if (exclude.Any(x => x == assembly))
            {
                skippedReason = string.Format("Test not supported on  {0}", string.Join(", ", exclude.Select(x => x.ToString()).ToArray()));
                return false;
            }
            return true;
        }
    }
}
