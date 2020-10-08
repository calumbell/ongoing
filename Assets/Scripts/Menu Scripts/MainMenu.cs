using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject fadeInPanel;
    public GameObject fadeOutPanel;
    public float fadeTime;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        audioManager.PlayAudio(AudioType.Music_808Pipes);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
            StartCoroutine(FadeCoroutine());
    }

    public IEnumerator FadeCoroutine()
    {
        if (fadeInPanel != null)
        {
            GameObject fadeIn = Instantiate(fadeInPanel, Vector3.zero, Quaternion.identity) as GameObject;
        }

        yield return new WaitForSeconds(fadeTime);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Dungeon");
        while(!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}
