using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DataCreator : MonoBehaviour
{
    [MenuItem("Window/My Editors/Create Game Settings")]
    public static void CreateGameSettings()
    {
        GameSettingsData GameSettings = ScriptableObject.CreateInstance<GameSettingsData>();
        GameSettings.name = "New Game Settings";

        Selection.activeObject = GameSettings;

        AssetDatabase.CreateAsset(GameSettings, "Assets/Datas/"+ GameSettings.name +".asset");
        AssetDatabase.Refresh();
    }
}
