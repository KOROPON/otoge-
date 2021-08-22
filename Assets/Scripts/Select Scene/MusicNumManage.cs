using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Reilas
{
    public class MusicNumManage : MonoBehaviour
    {
        RawImage jack;
        private AudioSource audioSource;
        public int music_number = -1;
        public static string difficulty;

        void Start()
        {
            jack = GameObject.Find("ジャケット1").GetComponent<RawImage>();
            audioSource = GameObject.Find("Audio Source Intro").GetComponent<AudioSource>();
        }

        private void musicInfo(string music_name,string jacketPath,float score)
        {
            audioSource.clip = Resources.Load<AudioClip>(music_name);
            audioSource.Play();
            jack.texture = Resources.Load<Texture2D>(jacketPath);
        }

        public void Tap(GameObject obj)
        {
            Debug.Log(obj.name + "_intro");
            if(music_number == obj.transform.GetSiblingIndex())
            {
                RhythmGamePresenter.musicname = obj.name;
                SceneManager.LoadScene("PlayScene");
            }
            else
            {
                music_number = obj.transform.GetSiblingIndex();
            }
            musicInfo("Songs/Music Select/" + obj.name + "_intro", "Jacket/" + obj.name + "_jacket", 1000000);
        }

        public void Difficulty(GameObject dif)
        {
             difficulty = dif.name;
             Debug.Log(difficulty);
        }
    }
}
