using UnityEngine;
using UnityEngine.UI;

namespace Reilas
{
    public class Setting : MonoBehaviour
    {
        //下準備


        public void RateChange(double change_value)
        {
            GameObject.Find("rate").GetComponent<Text>().text = (double.Parse(GameObject.Find("rate").GetComponent<Text>().text) + change_value).ToString();
        }
        [SerializeField] GameObject alter;
        public void SettingOpen()
        {
            alter.SetActive(true);
        }
        //使用関数
        public void Check()
        {
            NotePositionCalculatorService.speedvariable = float.Parse(GameObject.Find("rate").GetComponent<Text>().text);
            GameObject.Find("設定画面").SetActive(false);
        }
        public void Cancel()
        {
            GameObject.Find("rate").GetComponent<Text>().text = NotePositionCalculatorService.speedvariable.ToString();
            GameObject.Find("設定画面").SetActive(false);
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
        public static float GetRate()
        {
            return NotePositionCalculatorService.speedvariable;
        }

    }

}
