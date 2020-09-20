using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("Dungeon");
        }
    }
}
