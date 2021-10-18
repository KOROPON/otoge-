#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace Reilas
{
    public sealed class BarLine : MonoBehaviour
    {
        private static TextAsset? _jsonFile;

        private static SongDataBase? _songData;

        public static readonly List<float> BarLines = new List<float>();

        private static Vector3 CalculateBarLinePosition(float judgeTime, float currentTime)
        {
            var highSpeed = NotePositionCalculatorService.gameSpeed;
            
            // 0 なら判定ライン
            // 1 ならレーンの一番奥
            var t = currentTime - judgeTime;
            var normalizedTime = -t * highSpeed / 600f;

            return normalizedTime switch
            {
                var time when time >= 1 => new Vector3(0f, 0f, 999f),
                var time when time >= 0 => new Vector3(0f, 0f, highSpeed * 0.5f * t * t - highSpeed * t),
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

                if (judgeTime - currentTime < 5f)
                {
                    this.gameObject.SetActive(true);
                }
            }
            else
            {
                Vector3 berPos = CalculateBarLinePosition(judgeTime, currentTime);
                if (judgeTime < currentTime)
                {
                    NoteDestroy();
                }
                else
                {
                    transform.position = berPos;
                }
            }
        }

        private void NoteDestroy()
        {
            //Debug.Log(this.gameObject);
            RhythmGamePresenter._barLines.Remove(this);
            BarLines.Remove(BarLines[0]);
            Destroy(this.gameObject);
        }
    }
}