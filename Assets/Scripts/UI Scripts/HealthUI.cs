using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public IntValue playerHealth;
    public Text text;

    void Start()
    {
        onPlayerHealthChanged(playerHealth.value);
    }

    public void onPlayerHealthChanged(int health)
    {
        text.text = "Health: " + health.ToString();
    }
}
