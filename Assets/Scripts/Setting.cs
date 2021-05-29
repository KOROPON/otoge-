using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
  [SerializeField] GameObject alter;
  public void SettingOpen(){
    alter.SetActive(true);
  }
  public void SettingClose(){
    GameObject.Find("設定画面").SetActive(false);
  }
}
