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
        public Image Frame;
        public Image Color;
        public GameObject rating;
        [SerializeField] Text RateText;
        [SerializeField] GameObject alter;

        private float max = 5f;
        private float min = 1f;
        private float masterRate = 0.1f;
        private string pathKey;
        private Dictionary<string,List<float>> changePath;

       //内部関数
        private void SetSettingData()
        {
          changePath = new Dictionary<string,List<float>>();
          if (PlayerPrefs.HasKey("rate"))
          {
            changePath.Add("ノーツ速度", new List<float>(){0.1f, 1f, 5f, PlayerPrefs.GetFloat("rate")});
            changePath.Add("音", new List<float>(){1f, -999f, 999f, PlayerPrefs.GetFloat("audiogap")});
            changePath.Add("判定", new List<float>(){1f, -999f, 999f, PlayerPrefs.GetFloat("judgegap")});
            changePath.Add("音量", new List<float>(){1f, 0f, 100f, PlayerPrefs.GetFloat("volume")});
          }
          else
          {
            changePath.Add("ノーツ速度", new List<float>(){0.1f, 1f, 5f, 3.5f});
            changePath.Add("音", new List<float>(){1f, -999f, 999f, 0f});
            changePath.Add("判定", new List<float>(){1f, -999f, 999f, 0f});
            changePath.Add("音量", new List<float>(){1f, 0f, 100f, 50f});
          }
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
          if (pathKey != null)
          {
            changePath[pathKey][3] = float.Parse(RateText.text);
          }
          masterRate = changePath[Item.name][0];
          min = changePath[Item.name][1];
          max = changePath[Item.name][2];
          RateText.text = changePath[Item.name][3].ToString();
          pathKey = Item.name;

        }

        public void SettingOpen(GameObject rating)
        {
            if (alter.activeSelf == false)
            {
              alter.SetActive(true);
              SetSettingData();
              ChangeSettingData(rating);
              Color.color = new Color32(154, 154, 154, 255);
            }
            else
            {
              Check();
            }

        }

        public void Check()
        {
          changePath[pathKey][3] = float.Parse(RateText.text);
          PlayerPrefs.SetFloat("rate", changePath["ノーツ速度"][3]);
          PlayerPrefs.SetFloat("judgegap", changePath["判定"][3]);
          PlayerPrefs.SetFloat("audiogap", changePath["音"][3]);
          PlayerPrefs.SetFloat("volume", changePath["音量"][3]);
          Cancel();
        }

        public void Cancel()
        {
          alter.SetActive(false);
          pathKey = null;
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
