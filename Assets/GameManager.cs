using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public List<EnemyMovement> enemies = new();

    [SerializeField] private GameObject pauseMenu;
    private bool isPaused;
    
    [SerializeField] private PlayerMovement player;

    private void Start()
    {
        pauseMenu.SetActive(false);
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
}
