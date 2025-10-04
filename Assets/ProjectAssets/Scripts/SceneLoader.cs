using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static void SimpleLoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}