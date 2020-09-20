using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    public Text text;
    private float fps;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        fps = Mathf.Floor(1.0f / Time.deltaTime);
        text.text = "FPS: " + fps.ToString();
    }
}
