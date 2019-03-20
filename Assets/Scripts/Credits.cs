using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Credits : MonoBehaviour 
{
    CanvasGroup group;

	IEnumerator Start () 
    {
        group = GetComponentInChildren<CanvasGroup>();
        yield return new WaitForSeconds(5);
        float time = 0;
        while (time < 3)
        {
            time += Time.deltaTime;
            group.alpha = 1 - (time / 3);
            yield return null;
        }
        group.alpha = 0;
        SceneManager.LoadScene(0);
	}
}
