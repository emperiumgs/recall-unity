using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class HudController : MonoBehaviour
{
    public static HudController instance { get; private set; }

    public Slider healthSlider;
    public GameObject pauseMenu;

    void Awake()
    {
        instance = this;
    }

    public void UpdateHealth(float ratio)
    {
        healthSlider.value = ratio;
    }

    public void TogglePause()
    {
        if (pauseMenu.activeSelf)
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }
}