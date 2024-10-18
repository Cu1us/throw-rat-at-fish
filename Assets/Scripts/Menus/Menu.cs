using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene((int)GameMenu.PersonView);
    }

    public void OnClickControls()
    {
        
    }

    public void OnClickCredits()
    {
        
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}

public enum GameMenu
{
    MainMenu = 0,
    PersonView = 1,
    RatView = 2,
}
