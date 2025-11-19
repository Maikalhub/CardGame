using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    // Имя или номер сцены
   /// <summary>
   /// blic string sceneName;
   /// </summary>


    public void LoadScenePlay()
    {
        //SceneManager.LoadScene("");
        SceneManager.LoadScene("CardGame");
    }

    public void LoadSceneOptions()
    {
        //SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Остановить Play Mode
#else
        Application.Quit(); // Закрыть EXE
#endif
    }





}
