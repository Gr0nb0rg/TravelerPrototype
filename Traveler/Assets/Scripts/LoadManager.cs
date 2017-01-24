using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class LoadManager : MonoBehaviour {

        public static LoadManager Instance { set; get; }

        private void Awake()
        {
            //Scenes loaded on start
            Instance = this;
            Load("MainScene");
            Load("Player");
            Load("1");
            // Load("2");
        }

        public void Load(string sceneName)
        {
            if(!SceneManager.GetSceneByName(sceneName).isLoaded)
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        public void Unload(string sceneName)
        {
            if (SceneManager.GetSceneByName(sceneName).isLoaded)
                SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
