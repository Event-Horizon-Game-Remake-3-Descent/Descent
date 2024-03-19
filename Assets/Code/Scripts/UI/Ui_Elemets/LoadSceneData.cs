using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LoadSceneData
{
    [SerializeField] private string SceneToLoad = null;

    public void LoadScene()
    {
        if(SceneToLoad != null)
            SceneManager.LoadScene(SceneToLoad);
    }
}
