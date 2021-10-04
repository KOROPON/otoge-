using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{
    public static int combo;
    public static int miss;

    public Text gauge;

    private string _difficulty;
    private Slider _slider;

    private readonly Dictionary<string, int> _comboDataBase = new Dictionary<string, int>()
    {
        {"Easy", 2},
        {"Hard", 4},
        {"Extreme", 7}
    };

    void Start()
    {
        _difficulty = PlayerPrefs.GetString("difficulty");
        GetComponent<Slider>().value = 0f;
        combo = 0;
        miss = 0;
    }

    void LateUpdate()
    {
        while (combo >= _comboDataBase[_difficulty])
        {
            GetComponent<Slider>().value += 0.01f;
            combo -= _comboDataBase[_difficulty];
        }

        GetComponent<Slider>().value -= 0.03f * miss;

        gauge.text = GetComponent<Slider>().value.ToString();
    }
}
