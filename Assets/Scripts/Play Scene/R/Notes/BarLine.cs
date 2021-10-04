#nullable enable

using System.Collections.Generic;
using UnityEngine;
using Rhythmium;

namespace Reilas
{
    public sealed class BarLine : MonoBehaviour
    {
        private static TextAsset? _jsonFile;

        private static SongDataBase? _songData;

        public static readonly List<float> BarLines = new List<float>();

        private static Vector3 CalculateBarLinePosition(float judgeTime, float currentTime)
        {
            float highSpeed;
            if (PlayerPrefs.HasKey("rate"))
            {
                highSpeed = 80 * PlayerPrefs.GetFloat("rate");
            }
            else
            {
                highSpeed = 80 * 3.5f;
            }
            
            //var toLeft = left - LeftPosition;
            
            // 何秒後のノーツまで描画するか
            var 何秒後のノーツまで描画するか = 600f / highSpeed;

            // 0 なら判定ライン
            // 1 ならレーンの一番奥
            var t = currentTime - judgeTime;
            var normalizedTime = -t / 何秒後のノーツまで描画するか;

            return normalizedTime switch
            {
                var time when time >= 1 => new Vector3(0f, 0f, 999f),
                var time when time >= 0 => new Vector3(0f, 0f, highSpeed / 2 * t * t - highSpeed * t),
                _ => new Vector3(0f, 0f, 0f)
            };
        }
        
        public static void GetBarLines(string song, float audioLength)
        {
            _jsonFile = Resources.Load<TextAsset>("Level/SongDataBase");
            _songData = JsonUtility.FromJson<SongDataBase>(_jsonFile.text);

            foreach (SongName songName in _songData.songs)
            {
                if (songName.title != song) continue;
                Beat beat = songName.beat;
                var spacing = 60 / beat.bpm * beat.numerator * 4 / beat.denominator;
                for (float time = 0; time < audioLength + spacing; time += spacing)
                {
                    BarLines.Add(time);
                }
            }
        }
        
        public void Render(float judgeTime, float currentTime)
        {
            if (!this.gameObject.activeSelf)
            {

                if (judgeTime - currentTime < 10f)
                {
                    this.gameObject.SetActive(true);
                }
            }
            else
            {
                if (CalculateBarLinePosition(judgeTime, currentTime) == new Vector3(0f, 0f, 0f))
                {
                    NoteDestroy();
                }
                else
                {
                    transform.position = CalculateBarLinePosition(judgeTime, currentTime);
                }
            }
        }

        private void NoteDestroy()
        {
            //Debug.Log(this.gameObject);
            Destroy(this.gameObject);
            RhythmGamePresenter._barLines.Remove(this);
            BarLines.Remove(BarLines[0]);
        }
    }
}