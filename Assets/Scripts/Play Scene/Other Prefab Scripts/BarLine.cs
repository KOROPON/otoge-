#nullable enable

using UnityEngine;

namespace Reilas
{
    public sealed class BarLine : MonoBehaviour
    {
        private float _judgeTime;
        
        [SerializeField] private MeshFilter meshFilter = null!;

        private Vector3[]? _vertices;
        private Vector3[]? _uv;
        private int[]? _triangles;
        private const float Div = 32f;
        private const float OuterLaneRadius = 4.4f;

        private Mesh? _mesh;

        private const float InnerRadius = OuterLaneRadius - 0.03f; // 内縁の半径
        private const float OuterRadius = OuterLaneRadius; // 外縁の半径

        private float _position;
        private int _speedChangeIndex;

        public void Initialize(float judgeTime)
        {
            _judgeTime = judgeTime;
            _position = NotePositionCalculatorService.LeftOverPositionCalculator(_judgeTime, 1);
            _speedChangeIndex = 0;
            
            InitializeMesh();
            gameObject.transform.position = new Vector3(0f, 0f, 999f);
        }

        private void InitializeMesh()
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

            for (var x = 0; x < 33; x++)
            {
                var angleBase = Div - x;   // レーンの角度
                var angle = Mathf.PI * angleBase / Div;
                
                var innerY = Mathf.Sin(angle) * InnerRadius;
                var innerX = Mathf.Cos(angle) * InnerRadius;

                var outerY = Mathf.Sin(angle) * OuterRadius;
                var outerX = Mathf.Cos(angle) * OuterRadius;

                //zPos += zz;

                var innerPoint = new Vector3(innerX, innerY, 0f);
                var outerPoint = new Vector3(outerX, outerY, 0f);
                
                //(innerPoint, outerPoint) = (outerPoint, innerPoint);

                if (_vertices != null)
                {
                    _vertices[x * 2] = innerPoint;
                    _vertices[x * 2 + 1] = outerPoint;
                }

                var uvX = 1f / 32 * 0.8f * x + 0.1f;

                // 手前
                if (_uv == null) continue;
                _uv[x * 2 + 0] = new Vector2(uvX, 1f);
                _uv[x * 2 + 1] = new Vector2(uvX, 0f);
                
            }

            if (_vertices != null)
            {
                _vertices[66] = new Vector3(-4.6f, -0.25f, 0f);
                _vertices[67] = new Vector3(-4.6f, -0.18f, 0f);
                _vertices[68] = new Vector3(4.6f, -0.25f, 0f);
                _vertices[69] = new Vector3(4.6f, -0.18f, 0f);

                if (_mesh != null) _mesh.vertices = _vertices;
            }
            
            // メッシュを生成する
            _mesh = new Mesh
            {
                vertices = _vertices,
                triangles = _triangles
            };
            _mesh.MarkDynamic();

            _mesh.SetUVs(0, _uv);
#if UNITY_EDITOR
            _mesh.RecalculateBounds();
#endif
            meshFilter.mesh = _mesh;
        }

        public void Render(float currentTime)
        {
            if (currentTime == 0) return;
            if (!gameObject.activeSelf && _judgeTime - currentTime < 5f) gameObject.SetActive(true);
            else if (_judgeTime < currentTime) BarLineDestroy();


            var berPos = NotePositionCalculatorService.GetPosition(_judgeTime, currentTime, 1, _position, _speedChangeIndex);
            var gameObj = gameObject;
            var transPos = gameObj.transform.position;
            gameObj.transform.position = new Vector3(transPos.x, transPos.y, berPos);

            if (!RhythmGamePresenter.checkSpeedChangeEntity ||
                currentTime < RhythmGamePresenter.CalculatePassedTime(_speedChangeIndex)) return;
            _position -= NotePositionCalculatorService.SpanCalculator(_speedChangeIndex, _judgeTime, 1);
            _speedChangeIndex++;
        }

        public void BarLineDestroy()
        {
            RhythmGamePresenter.BarLines.Remove(this);
            Destroy(gameObject);
        }
    }
}