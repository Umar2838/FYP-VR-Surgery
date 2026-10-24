using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScreens : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject introduction;
    public GameObject welcome;
    public GameObject objectives;

    public void ShowIntroduction()
    {
        Set(mainMenu, false);
        Set(objectives, false);
        Set(welcome, true);
        Set(introduction, true);
    }

    public void ShowMainMenu()
    {
        Set(introduction, false);
        Set(mainMenu, true);
    }

    public void StartCase()
    {
        SceneManager.LoadScene("Main_Operation_Room");
    }

    static void Set(GameObject go, bool on)
    {
        if (go != null)
            go.SetActive(on);
    }
}
