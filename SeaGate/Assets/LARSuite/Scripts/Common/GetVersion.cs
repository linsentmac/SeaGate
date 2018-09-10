using LARSuite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetVersion  {

    
    public static string GetVersionfun( )
    {
        int major = LarManager.starkitVersion_major;
        int minor = LarManager.starkitVersion_minor;
        string versionString = "version is :" + major + "." + minor;
        return versionString;
    }
	
}
