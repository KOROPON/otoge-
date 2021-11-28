using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Reilas
{
    public class Setting : MonoBehaviour
    {
        //下準備
        public Image color;
        public GameObject rating;
        public Image frame;
        public Text infoT;
        [SerializeField] Text rateText;
        [SerializeField] GameObject alter;

        private float _max = 5f;
        private float _min = 1f;
        private float _masterRate = 0.1f;
        private string _pathKey;
        private Dictionary<string,List<float>> _changePath;

        private void Start()
        {
            if (!PlayerPrefs.HasKey("rate")) PlayerPrefs.SetFloat("rate", 5f);
        }
       //内部関数
        private void SetSettingData()
        {
          _changePath = new Dictionary<string,List<float>>();
          if (PlayerPrefs.HasKey("rate"))
          {
            _changePath.Add("ノーツ速度", new List<float>(){0.1f, 1f, 10f, PlayerPrefs.GetFloat("rate")});
            _changePath.Add("音", new List<float>(){1f, -999f, 999f, PlayerPrefs.GetFloat("audiogap")});
            _changePath.Add("判定", new List<float>(){1f, -999f, 999f, PlayerPrefs.GetFloat("judgegap")});
            _changePath.Add("音量", new List<float>(){1f, 0f, 100f, PlayerPrefs.GetFloat("volume")});
          }
          else
          {
            _changePath.Add("ノーツ速度", new List<float>(){0.1f, 1f, 10f, 5f});
            _changePath.Add("音", new List<float>(){1f, -999f, 999f, 0f});
            _changePath.Add("判定", new List<float>(){1f, -999f, 999f, 0f});
            _changePath.Add("音量", new List<float>(){1f, 0f, 100f, 50f});
          }
        }

        private float Clamp(float input, float min, float max)
        {
          float output = Mathf.Max(input, max);
          output = Mathf.Min(output,min);
          return  output;
        }

        private void RateChange(float changeValue, float min, float max)
        {
            rateText.text = (Clamp(float.Parse(rateText.text) + changeValue, min, max)).ToString();
        }



　　　　//利用関数
        public void ChangeSettingData(GameObject item)
        {
          if (_pathKey != null)
          {
            _changePath[_pathKey][3] = float.Parse(rateText.text);
          }
          _masterRate = _changePath[item.name][0];
          _min = _changePath[item.name][1];
          _max = _changePath[item.name][2];
          rateText.text = _changePath[item.name][3].ToString();
          _pathKey = item.name;
          frame.sprite = Resources.Load<Sprite>("Frame/Frame_" + item.name);
          switch (item.name)
          {
            case "ノーツ速度": infoT.text = "ノーツの流れる速度を設定します"; break;
            case "音量": infoT.text = "ノーツを叩いたときに発生するタップ音量を調整します"; break;
            case "判定": infoT.text = "流れてくるノーツに対する判定を行うタイミングを調整します\nノーツより早く叩かなければならないときは負の方向へ。\n(こちらより先にAudioの調整を推奨します)"; break;
            case "音": infoT.text = "曲とノーツが流れてくるタイミングのズレを修正します\n曲よりも早くノーツが流れてくるときは負の方向へ。\n(judgeより先にこちらの調整を推奨します)"; break;
          }
        }

        public void SettingOpen(GameObject rating)
        {
            if (alter.activeSelf == false)
            {
              alter.SetActive(true);
              SetSettingData();
              ChangeSettingData(rating);
              color.color = new Color32(154, 154, 154, 255);
            }
            else
            {
              Check();
            }

        }

        public void Check()
        {
          _changePath[_pathKey][3] = float.Parse(rateText.text);
          PlayerPrefs.SetFloat("rate", _changePath["ノーツ速度"][3]);
          PlayerPrefs.SetFloat("judgegap", _changePath["判定"][3]);
          PlayerPrefs.SetFloat("audiogap", _changePath["音"][3]);
          PlayerPrefs.SetFloat("volume", _changePath["音量"][3]);
          Cancel();
        }

        public void Cancel()
        {
          alter.SetActive(false);
          _pathKey = null;
          color.color = new Color32(255, 255, 255, 255);
        }

        public void Up()
        {
            RateChange(_masterRate, _max, _min);
        }

        public void ShiftUp()
        {
            RateChange(_masterRate * 10, _max, _min);
        }

        public void Down()
        {
            RateChange(-_masterRate, _max, _min);
        }

        public void ShiftDown()
        {
            RateChange(-_masterRate * 10,_max,_min);
        }
    }

}
