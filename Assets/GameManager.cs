using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public List<EnemyMovement> enemies = new();

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOver;
    [SerializeField] GameObject AttackRato;
    [SerializeField] Image BlackImage;

    [SerializeField] Image FinalImage;

    private bool isPaused;

    [SerializeField] private PlayerMovement player;
    [SerializeField] AudioSource RatAudioSource;
    public bool RatPickedUp = false;
    public bool GameEnded = false;

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

    public void PickupHelmet()
    {
        RatPickedUp = true;
        FindObjectOfType<PlayerAttack>()?.PlayIntroMonologue();
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

    public void GameEnd()
    {
        RatPickedUp = false;
        GameEnded = true;
        FindObjectOfType<PlayerAttack>()?.OnGameEnd();
        AttackRato.SetActive(true);
        Invoke(nameof(ActivateBlackImage), 1.7f);
    }
    public void ActivateBlackImage()
    {
        Invoke(nameof(ActivateFinalImage), 3f);
        BlackImage.gameObject.SetActive(true);

    }
    public void ActivateFinalImage()
    {
        BlackImage.gameObject.SetActive(false);
        FinalImage.gameObject.SetActive(true);
        GetComponent<AudioSource>().Play();
    }
}
