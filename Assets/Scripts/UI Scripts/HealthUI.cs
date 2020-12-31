using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("State Variables")]
    public IntValue playerHealth;
    public Text text;

    void Start()
    {
        OnPlayerHealthChanged();
    }

    public void OnPlayerHealthChanged()
    {
        text.text = "Health: " + playerHealth.value;
    }
}
