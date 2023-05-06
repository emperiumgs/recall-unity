using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour
{
    public Image splash;

    Image logo;

    void Start()
    {
        logo = splash.transform.GetChild(0).GetComponent<Image>();
        StartCoroutine(SplashFade());
    }

    IEnumerator SplashFade()
    {
        yield return new WaitForSeconds(2);
        float time = 0;
        while (time < 2)
        {
            time += Time.deltaTime;
            logo.color = Color.white - Color.white * (time / 2);
            yield return null;
        }
        logo.color = Color.clear;
        splash.gameObject.SetActive(false);
    }

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}