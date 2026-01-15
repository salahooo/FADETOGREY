// NEW FILE
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FadeToGrey.EditorTools
{
    /// <summary>
    /// Builds the Fade to Grey folder layout and placeholder scenes inside the Unity Editor.
    /// </summary>
    public static class ProjectStructureBuilder
    {
        #region Constants
        /// <summary>
        /// Relative folder paths that should exist under the Assets folder.
        /// </summary>
        private static readonly string[] FolderPaths =
        {
            "Scenes",
            "Scripts/Player",
            "Scripts/Obstacles",
            "Scripts/Managers",
            "Scripts/Audio",
            "Scripts/Effects",
            "Prefabs/Player",
            "Prefabs/Obstacles",
            "Prefabs/Collectibles",
            "Art/Characters",
            "Art/Environment",
            "Art/UI",
            "Materials",
            "Animations",
            "Audio"
        };

        /// <summary>
        /// Scene asset names to create in the Scenes folder.
        /// </summary>
        private static readonly string[] SceneNames =
        {
            "MainMenu",
            "Level_Stad",
            "Level_Platteland",
            "Level_Berg",
            "UI"
        };
        #endregion

        #region Menu Items
        /// <summary>
        /// Creates the project folders and empty scenes on demand.
        /// </summary>
        [MenuItem("Tools/Fade to Grey/Build Project Structure")]
        public static void BuildProjectStructure()
        {
            // Avoid losing edits if the user has unsaved scenes open.
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            CreateFolders();
            CreateScenes();

            AssetDatabase.Refresh();
            Debug.Log("Fade to Grey: Project structure created or verified.");
        }
        #endregion

        #region Folder Creation
        /// <summary>
        /// Iterates over the folder list and ensures each one exists.
        /// </summary>
        private static void CreateFolders()
        {
            foreach (string folderPath in FolderPaths)
            {
                CreateFolderRecursive("Assets", folderPath);
            }
        }

        /// <summary>
        /// Creates nested folders under the provided root path.
        /// </summary>
        /// <param name="root">Root folder to build under.</param>
        /// <param name="relativePath">Folder path relative to the root.</param>
        private static void CreateFolderRecursive(string root, string relativePath)
        {
            string[] parts = relativePath.Split('/');
            string current = root;

            foreach (string part in parts)
            {
                string next = Path.Combine(current, part).Replace("\\", "/");
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, part);
                }

                current = next;
            }
        }
        #endregion

        #region Scene Creation
        /// <summary>
        /// Creates empty scenes for each required scene name if missing.
        /// </summary>
        private static void CreateScenes()
        {
            const string scenesFolder = "Assets/Scenes";

            foreach (string sceneName in SceneNames)
            {
                string scenePath = $"{scenesFolder}/{sceneName}.unity";
                if (File.Exists(scenePath))
                {
                    continue;
                }

                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, scenePath);
            }
        }
        #endregion
    }
}
#endif

