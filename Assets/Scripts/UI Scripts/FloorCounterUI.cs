using UnityEngine;
using UnityEngine.UI;

public class FloorCounterUI : MonoBehaviour
{
    public Text text;
    public IntValue floor;

    // Use this for initialization
    void Start()
    {
        onFloorChanged(floor.value);
    }

    public void onFloorChanged(int floor)
    {
        text.text = "Floor " + floor.ToString();
    }
}
