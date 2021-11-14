#nullable enable

using System.Collections.Generic;
using Rhythmium;
using UnityEngine;

namespace Reilas
{
    public sealed class BarLine : MonoBehaviour
    {
        private static TextAsset? _jsonFile;

        private static SongDataBase? _songData;

        public static readonly List<float> BarLines = new List<float>();

        [SerializeField] private MeshFilter meshFilter = null!;

        private Vector3[]? _vertices;
        private Vector3[]? _uv;
        private int[]? _triangles;
        private const float Div = 32f;
        private const float OuterLaneRadius = 4.4f;

        private Mesh? _mesh;

        private const float InnerRadius = OuterLaneRadius - 0.03f; // 内縁の半径
        private const float OuterRadius = OuterLaneRadius;        // 外縁の半径

        private static float CalculateZPos(float judgeTime, List<SpeedChangeEntity> speedChangeEntities, float currentTime)
        {
            var t = currentTime - judgeTime;
            if (speedChangeEntities.Count == 0)
                return NotePositionCalculatorService.PositionCalculator(t,
                    NotePositionCalculatorService.SpeedCalculator(NotePositionCalculatorService.firstChartSpeed));
            var zPos = 0f;
            for (var i = speedChangeEntities.Count - 1; i >= 0; i--)
            {
                var nextNotePassedTime = RhythmGamePresenter.CalculatePassedTime(speedChangeEntities, i + 1);
                if (currentTime >= nextNotePassedTime) break;
                var notePassedTime = RhythmGamePresenter.CalculatePassedTime(speedChangeEntities, i);
                if (judgeTime < notePassedTime) continue;
                var highSpeed = NotePositionCalculatorService.SpeedCalculator(speedChangeEntities[i].Speed);
                var nextNotePosition =
                    NotePositionCalculatorService.PositionCalculator(nextNotePassedTime - judgeTime, highSpeed);
                zPos += currentTime < RhythmGamePresenter.CalculatePassedTime(speedChangeEntities, i - 1)
                    ? NotePositionCalculatorService.PositionCalculator(t, highSpeed) - nextNotePosition
                    : NotePositionCalculatorService.PositionCalculator(notePassedTime - judgeTime, highSpeed) -
                      nextNotePosition;
            }

            return zPos;
        }
        private static Vector3 CalculateBarLinePosition(float judgeTime, float currentTime, List<SpeedChangeEntity> speedChangeEntities)
        {
            var highSpeed = NotePositionCalculatorService.normalizedSpeed;
            
            // 0 なら判定ライン
            // 1 ならレーンの一番奥
            var t = currentTime - judgeTime;
            var normalizedTime = -t * highSpeed / 600f;

            return normalizedTime switch
            {
                var time when time >= 1 => new Vector3(0f, 0f, 999f),
                var time when time >= 0 => new Vector3(0f, 0f, CalculateZPos(judgeTime, speedChangeEntities, currentTime)),
                _ => new Vector3(0f, 0f, 0f)
            };
        }

        public void Initialize()
        {
            if (meshFilter == null) return;

            _vertices = new Vector3[70];
            _uv = new Vector3[70];
            _triangles = new int[198];

            // 前面
            for (var i = 0; i < 32; i++)
            {
                _triangles[i * 6 + 0] = 0 + i * 2;
                _triangles[i * 6 + 1] = 1 + i * 2;
                _triangles[i * 6 + 2] = 3 + i * 2;
                _triangles[i * 6 + 3] = 2 + i * 2;
                _triangles[i * 6 + 4] = 0 + i * 2;
                _triangles[i * 6 + 5] = 3 + i * 2;
            }
            _triangles[192] = 66;
            _triangles[193] = 68;
            _triangles[194] = 67;
            _triangles[195] = 68;
            _triangles[196] = 69;
            _triangles[197] = 67;


            // メッシュを生成する
            _mesh = new Mesh
            {
                vertices = _vertices,
                triangles = _triangles
            };
            _mesh.MarkDynamic();
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
                for (float time = 0; time < audioLength + spacing; time += spacing) BarLines.Add(time);
            }
        }

        public void Render(float judgeTime, float currentTime, List<SpeedChangeEntity> speedChangeEntities)
        {
            if (!gameObject.activeSelf && judgeTime - currentTime < 5f) gameObject.SetActive(true);
            else if (judgeTime < currentTime) NoteDestroy();

            var berPos = CalculateBarLinePosition(judgeTime, currentTime, speedChangeEntities).z; // Make Mesh 頂点

            if (meshFilter == null)
            {
                Debug.Log("null");
                return;
            }


            for (var z = 0; z < 1; z++)
            {
                for (var x = 0; x < 33; x++)
                {
                    var angle = Mathf.PI / Div * x;   // レーンの角度

                    angle = Mathf.PI - angle;


                    var innerY = Mathf.Sin(angle) * InnerRadius;
                    var innerX = Mathf.Cos(angle) * InnerRadius;

                    var outerY = Mathf.Sin(angle) * OuterRadius;
                    var outerX = Mathf.Cos(angle) * OuterRadius;



                    //zPos += zz;

                    var innerPoint = new Vector3(innerX, innerY, berPos);
                    var outerPoint = new Vector3(outerX, outerY, berPos);


                    //(innerPoint, outerPoint) = (outerPoint, innerPoint);

                    var p = 66 * z;

                    if (_vertices != null)
                    {
                        _vertices[p + x * 2 + 0] = innerPoint;
                        _vertices[p + x * 2 + 1] = outerPoint;
                    }

                    var uvX = 1f / 32 * 0.8f * x + 0.1f;

                    // 手前
                    if (z != 0 || _uv == null) continue;
                    _uv[x * 2 + 0] = new Vector2(uvX, 1f);
                    _uv[x * 2 + 1] = new Vector2(uvX, 0f);
                }
            }

            if (_vertices != null)
            {
                _vertices[66] = new Vector3(-4.4f, -0.25f, berPos);
                _vertices[67] = new Vector3(-4.4f, -0.18f, berPos);
                _vertices[68] = new Vector3(4.4f, -0.25f, berPos);
                _vertices[69] = new Vector3(4.4f, -0.18f, berPos);

                if (_mesh != null) _mesh.vertices = _vertices;
            }

            //GetComponent<MeshRenderer>().material.cal

            if (_mesh == null) return;
            _mesh.SetUVs(0, _uv);
#if UNITY_EDITOR
            _mesh.RecalculateBounds();
#endif
            meshFilter.mesh = _mesh;
        }

        public void NoteDestroy()
        {
            //Debug.Log(this.gameObject);
            RhythmGamePresenter.BarLines.Remove(this);
            BarLines.Remove(BarLines[0]);
            Destroy(gameObject);
        }
    }
}