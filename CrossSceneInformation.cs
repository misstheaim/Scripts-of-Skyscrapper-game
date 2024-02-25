using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CrossSceneInformation
{
    public static bool isLoading { get;  set; } = false;
    public static int seed { get; set; } = 0;
    public static bool IsSeedInstanstioned
    { 
        get { return (seed != 0); }
    }
    public static int count { get; set; }
    public static int difficulty { get; set; }

    public static GameData currentData { get; set; }

    public static DataManager dataManager = new DataManager();
}
