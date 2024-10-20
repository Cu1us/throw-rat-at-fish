using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public List<EnemyMovement> enemies = new();
    [SerializeField] private PlayerMovement player;

    [SerializeField] private GameObject pauseMenu;
    private bool isPaused;

    [SerializeField] private GameObject gameOver;
    
    private void Start()
    {
        pauseMenu.SetActive(false);
        gameOver.SetActive(false);
    }
    
    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
            PauseGame();
    }

    private void PauseGame()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            player.enabled = false;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            player.enabled = true;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void GameOver()
    {
        gameOver.SetActive(true);
        player.enabled = false;
    }

    public void OnClickMenu()
    {
        SceneManager.LoadScene(0);
    }
}
