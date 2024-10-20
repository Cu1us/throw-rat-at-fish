using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject controlsHUD;
    [SerializeField] private GameObject creditsHUD;

    private void Start()
    {
        controlsHUD.SetActive(false);
        creditsHUD.SetActive(false);
    }
    
    public void OnClickStart()
    {
        SceneManager.LoadScene((int)GameMenu.PersonView);
    }

    public void OnClickControls()
    {
        controlsHUD.SetActive(true);
    }

    public void OnClickCredits()
    {
        creditsHUD.SetActive(true);
    }

    public void OnClickBack(GameObject hudObject)
    {
        hudObject.SetActive(false);
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
