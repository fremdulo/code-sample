using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class EditorUtil
{
    static EditorUtil()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("EditorUtil Init buildIndex=" + buildIndex);
        PlayerPrefs.SetInt("DevScene", buildIndex);
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Staging.unity");
    }
}
