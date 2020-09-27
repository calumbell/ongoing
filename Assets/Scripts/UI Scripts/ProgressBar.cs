using System.Collections;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public GameObject foreground;
    public GameObject background;

    private void Start()
    {
        foreground.SetActive(false);
        background.SetActive(false);
    }

    public void Begin(float time)
    {
        StartCoroutine(ProgressBarCoroutine(time));
    }

    public void UpdateBar(float progress, float finish)
    {
        foreground.transform.localScale = new Vector3(
            progress/finish,
            foreground.transform.localScale.y,
            foreground.transform.localScale.z);
    }

    public IEnumerator ProgressBarCoroutine(float finishTime)
    {
        foreground.SetActive(true);
        background.SetActive(true);

        float startTime = Time.time;
        float currentTime = Time.time - startTime;
        UpdateBar(currentTime, finishTime);

        while (currentTime < finishTime)
        {
            currentTime = Time.time - startTime;
            UpdateBar(currentTime, finishTime);
            yield return null;
        }

        foreground.SetActive(false);
        background.SetActive(false);
    }
}
