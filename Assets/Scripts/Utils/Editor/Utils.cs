using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Utils : Editor
{
    #region Public Methods
    [MenuItem("Utils/Open/Persistent DataPath")]
    public static void OpenPersistentDataPath()
    {
        Application.OpenURL(Application.persistentDataPath);
    }

    [MenuItem("Utils/Open/Application DataPath")]
    public static void OpenApplicationDataPath()
    {
        Application.OpenURL(Application.dataPath);
    }
    #endregion
}
