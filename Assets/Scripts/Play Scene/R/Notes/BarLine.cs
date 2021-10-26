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

        [SerializeField] private MeshFilter _meshFilter = null!;

        private Vector3[]? _vertices;
        private Vector3[]? _uv;
        private int[]? _triangles;
        const float div = 32f;
        const float outerLaneRadius = 4.4f;

        private Mesh? _mesh;

        const float innerRadius = outerLaneRadius - 0.03f; // 内縁の半径
        const float outerRadius = outerLaneRadius;        // 外縁の半径

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

        public void Initialize()
        {
            if (_meshFilter == null)
            {
                return;
            }

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
                if (judgeTime < currentTime)
                {
                    NoteDestroy();
                }
            }

            float berPos = CalculateBarLinePosition(judgeTime, currentTime).z; // Make Mesh 頂点

            if (_meshFilter == null)
            {
                Debug.Log("null");
                return;
            }


            for (var z = 0; z < 1; z++)
            {
                for (var x = 0; x < 33; x++)
                {
                    var laneIndex = x;  //レーン番号

                    var angle = Mathf.PI / div * laneIndex;   // レーンの角度

                    angle = Mathf.PI - angle;


                    var innerY = Mathf.Sin(angle) * innerRadius;
                    var innerX = Mathf.Cos(angle) * innerRadius;

                    var outerY = Mathf.Sin(angle) * outerRadius;
                    var outerX = Mathf.Cos(angle) * outerRadius;



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

                    float uvX = 1f / 32 * 0.8f * x + 0.1f;

                    // 手前
                    if (z == 0)
                    {
                        if (_uv != null)
                        {
                            _uv[x * 2 + 0] = new Vector2(uvX, 1f);
                            _uv[x * 2 + 1] = new Vector2(uvX, 0f);
                        }
                    }
                }
            }
            _vertices[66] = new Vector3(-4.6f, -0.25f, berPos);
            _vertices[67] = new Vector3(-4.6f, -0.18f, berPos);
            _vertices[68] = new Vector3(4.6f, -0.25f, berPos);
            _vertices[69] = new Vector3(4.6f, -0.18f, berPos);

            _mesh.vertices = _vertices;

            //GetComponent<MeshRenderer>().material.cal

            _mesh.SetUVs(0, _uv);
#if UNITY_EDITOR
            _mesh.RecalculateBounds();
#endif
            _meshFilter.mesh = _mesh;

        }

        private void NoteDestroy()
        {
            //Debug.Log(this.gameObject);
            RhythmGamePresenter.BarLines.Remove(this);
            BarLines.Remove(BarLines[0]);
            Destroy(this.gameObject);
        }
    }
}