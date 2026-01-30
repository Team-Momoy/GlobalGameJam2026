using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneChangerWindow : OdinEditorWindow
{
    [MenuItem("Window/Scene Changer")]
    private static void OpenWindow()
    {
        GetWindow<SceneChangerWindow>().Show();
    }

    [OnInspectorGUI]
    private void DrawSceneButtons()
    {
        GUILayout.BeginHorizontal();
        foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled))
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
            if (GUILayout.Button(sceneName))
            {
                LoadScene(scene.path);
            }
        }
        GUILayout.EndHorizontal();
    }

    private void LoadScene(string path)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(path);
        }
    }
}