using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reilas
{
    public class Setting : MonoBehaviour
    {
        //下準備
        public Text RateText;
        public Image Frame;
        public Image Color;
        public GameObject rating;
        [SerializeField] GameObject alter;

        private float max = 5f;
        private float min = 1f;
        private float masterRate = 0.1f;
        private string pathKey = "ノーツ速度";
        private Dictionary<string,List<float>> changePath;

        public static float rate = 3.5f;
        public static float judgegap = 0f;
        public static float audiogap = 0f;
        public static float volume = 50f;
       //内部関数
        private void SetSettingData()
        {
          changePath = new Dictionary<string,List<float>>();
          changePath.Add("ノーツ速度", new List<float>(){0.1f, 1f, 5f, rate});
          changePath.Add("音", new List<float>(){1f, -999f, 999f, judgegap});
          changePath.Add("判定", new List<float>(){1f, -999f, 999f, audiogap});
          changePath.Add("音量", new List<float>(){1f, 0f, 100f, volume});
          Debug.Log(changePath["音"][3]);
        }

        private float Clamp(float input, float min, float max)
        {
          float output = Mathf.Max(input, max);
          output = Mathf.Min(output,min);
          return  output;
        }

        private void RateChange(float change_value, float min, float max)
        {
            RateText.text = (Clamp(float.Parse(RateText.text) + change_value, min, max)).ToString();
        }

　　　　//利用関数
        public void ChangeSettingData(GameObject Item)
        {
          changePath[pathKey][3] = float.Parse(RateText.text);
          masterRate = changePath[Item.name][0];
          min = changePath[Item.name][1];
          max = changePath[Item.name][2];
          Debug.Log(changePath[Item.name][3]);
          RateText.text = changePath[Item.name][3].ToString();
          pathKey = Item.name;

        }

       //使用関数
        public void SettingOpen(GameObject rating)
        {
            if (alter.activeSelf == false)
            {
              SetSettingData();
              alter.SetActive(true);
              ChangeSettingData(rating);
              Color.color = new Color32(154, 154, 154, 255);
            }
            else
            {
              Check();
              Color.color = new Color32(255, 255, 255, 255);
            }

        }

        public void Check()
        {
          rate =  changePath["ノーツ速度"][3];
          judgegap = changePath["音"][3];
          audiogap = changePath["判定"][3];
          volume = changePath["音量"][3];
          alter.SetActive(false);
          Color.color = new Color32(255, 255, 255, 255);
        }


        public void Cancel()
        {
          alter.SetActive(false);
          Color.color = new Color32(255, 255, 255, 255);
        }
        public void Up()
        {
            RateChange(masterRate, max, min);
        }
        public void ShiftUP()
        {
            RateChange(masterRate * 10, max, min);
        }
        public void Down()
        {
            RateChange(-masterRate, max, min);
        }
        public void ShiftDown()
        {
            RateChange(-masterRate * 10,max,min);
        }
    }

}
