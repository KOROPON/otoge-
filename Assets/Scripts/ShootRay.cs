using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShootRay : MonoBehaviour
{
    public Text Text1;
    void Start()
    {
        Text1.text = "qwertyuioplkjhgfdsazxcvbnm";
    }

    // Update is called once per frame
    void Update()
    {
        // タッチされているとき
        if (0 < Input.touchCount)
        {
            // タッチされている指の数だけ処理
            for (int i = 0; i < Input.touchCount; i++)
            {
                //タッチした位置からRayを飛ばす
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                RaycastHit hit = new RaycastHit();


                float maxDist = 8f;
                bool rayhit = Physics.Raycast(ray, out hit, maxDist);
                if (rayhit)
                {
                    GameObject hitObject = GameObject.Find(hit.transform.name);
                    Debug.Log(hit.transform.name);

                }
            }
        }
    }
}