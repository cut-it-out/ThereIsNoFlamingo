using UnityEngine;
using TMPro;

public class DisplayLevel : MonoBehaviour
{
    [SerializeField] TMP_Text levelText;

    Spawner spawner;

    private void Awake()
    {
        spawner = Spawner.GetInstance();
        DisplayLevel_OnProgressMultiplierChange(0f);
    }

    private void OnEnable()
    {
        spawner.OnProgressMultiplierChange += DisplayLevel_OnProgressMultiplierChange;
        DisplayLevel_OnProgressMultiplierChange(0f);
    }
    private void OnDisable()
    {
        spawner.OnProgressMultiplierChange -= DisplayLevel_OnProgressMultiplierChange;
    }

    private void DisplayLevel_OnProgressMultiplierChange(float newSpeed)
    {
        levelText.text = "Level: " + spawner.GetProgressMultiplier().ToString();
    }
}
