// NEW FILE
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FadeToGrey
{
    /// <summary>
    /// Utility component for loading scenes by name, restarting, or advancing to the next scene.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        #region Serialized Fields
        /// <summary>
        /// If true, wraps to the first scene when reaching the end of build settings.
        /// </summary>
        [SerializeField] private bool wrapAtEnd = true;
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads a scene by its name.
        /// </summary>
        /// <param name="name">Scene name listed in the build settings.</param>
        public void LoadScene(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            SceneManager.LoadScene(name);
        }

        /// <summary>
        /// Reloads the currently active scene.
        /// </summary>
        public void Restart()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }

        /// <summary>
        /// Loads the next scene in the build settings order.
        /// </summary>
        public void LoadNext()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            int nextIndex = activeScene.buildIndex + 1;

            if (nextIndex >= SceneManager.sceneCountInBuildSettings)
            {
                // Loop back to the first scene if configured to wrap.
                if (wrapAtEnd)
                {
                    nextIndex = 0;
                }
                else
                {
                    return;
                }
            }

            SceneManager.LoadScene(nextIndex);
        }
        #endregion
    }
}

