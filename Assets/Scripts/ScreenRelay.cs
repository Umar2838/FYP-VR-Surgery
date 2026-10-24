using UnityEngine;

public class ScreenRelay : MonoBehaviour
{
    public MenuScreens screens;

    public void ShowIntroduction()
    {
        if (screens != null)
            screens.ShowIntroduction();
    }

    public void ShowMainMenu()
    {
        if (screens != null)
            screens.ShowMainMenu();
    }

    public void StartCase()
    {
        if (screens != null)
            screens.StartCase();
    }
}
