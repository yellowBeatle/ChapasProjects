using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    public KeyCode keyPauseMenu;

    public GameObject pauseMenu;
    public GameObject optionsMenu;

    private GameManager m_gameManager;

    public ParticleSystem m_blueVFX;
    public ParticleSystem m_redVFX;

    public AudioSource click;
    public AudioSource menuMusic;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            pauseMenu = null;

            m_blueVFX.Play();

            m_redVFX.Play();

            click = GetComponent<AudioSource>();
        }
        else
        {

        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            
        }
        else
        {
            if (Input.GetKeyDown(keyPauseMenu))
            {
                if (pauseMenu.active)
                {
                    DisablePauseMenu();
                }
                else
                {
                    EnablePauseMenu();
                }
            }
        }
    }

    public void LoadScene(string scene)
    {
        Time.timeScale = 1.0f;

        if (SceneManager.GetActiveScene().name == "Scene")
        {
            m_gameManager = FindObjectOfType<GameManager>();
            Destroy(m_gameManager);
        }
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void EnablePauseMenu()
    {
        pauseMenu.SetActive(true);

        Time.timeScale = 0.0f;
    }

    public void DisablePauseMenu()
    {
        pauseMenu.SetActive(false);

        Time.timeScale = 1.0f;
    }

    public void EnableOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

    public void DisableOptionsMenu()
    {
        optionsMenu.SetActive(false);
    }
    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Click()
    {
        click.Play();
    }

    public void StopMenuMusic()
    {
        menuMusic.Stop();
    }
}
