using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
  //下準備
  string rate="";

  public void RateChange(double change_value){
    GameObject.Find("rate").GetComponent<Text>().text=(double.Parse(GameObject.Find("rate").GetComponent<Text>().text)+change_value).ToString();
  }
  [SerializeField] GameObject alter;
  public void SettingOpen(){
    alter.SetActive(true);
  }
  //使用関数
  public void SettingClose(){
    GameObject.Find("設定画面").SetActive(false);
  }
  public void reserve(){
    rate=GameObject.Find("rate").GetComponent<Text>().text;
  }
  public void cancel(){
    GameObject.Find("rate").GetComponent<Text>().text=rate;
  }
  public void Up(){
    RateChange(0.1);
  }
  public void shiftUP(){
    RateChange(1);
  }
  public void Down(){
    RateChange(-0.1);
  }
  public void shiftDown(){
    RateChange(-1);
  }
}
