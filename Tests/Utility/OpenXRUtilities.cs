#if OPENXR_SDK
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using System.Linq;
using System.Collections.Generic;

//Adding here for now. Should be moved to XR Test framework
public static class OpenXRUtilities
{
    private static OpenXRFeature[] GetFeatures()
    {
        return OpenXRSettings.ActiveBuildTargetInstance.GetFeatures();
    }
    
    public static bool IsRunningMockRuntime(){

       var features = GetFeatures();
       return features.Any(f => f.GetType().Name.ToLower() == "mockruntime" && f.enabled == true);
    }

}
#endif