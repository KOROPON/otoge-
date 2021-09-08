using UnityEngine;
using UnityEngine.UI;
using System;

namespace Reilas
{
    public class Setting : MonoBehaviour
    {
        //下準備
        public Text rateText;
        [SerializeField] GameObject alter;

        public static float rate = 3.5f;
        public static float judgegap = 0f;
        public static float audiogap = 0f;
        public static float volume = 5f;

        private double Clamp(double input, float min, float max)
        {
          float output = Mathf.Max( (float) input, max);
          output = Mathf.Min(output,min);
          return (double) output;
        }

        public void RateChange(double change_value)
        {
            rateText.text = (double.Parse(rateText.text) + change_value).ToString();
        }
        public void SettingOpen()
        {
            alter.SetActive(true);
        }
        //使用関数
        public void Check()
        {
            alter.SetActive(false);
        }


        public void Cancel()
        {

            alter.SetActive(false);
        }
        public void Up()
        {
            RateChange(0.1);
        }
        public void ShiftUP()
        {
            RateChange(1);
        }
        public void Down()
        {
            RateChange(-0.1);
        }
        public void ShiftDown()
        {
            RateChange(-1);
        }
    }

}
