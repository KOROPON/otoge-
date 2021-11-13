#nullable enable

using System;
using System.Collections.Generic;
using Rhythmium;
using UnityEngine;

namespace Reilas
{
    public sealed class NoteConnector : MonoBehaviour
    {
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

        public static int GetConnectorLane(int lane, List<int> groundLanes)
        {
            var closestGroundLane = (int)Mathf.Floor((lane - 4f) * 0.125f);
            switch (closestGroundLane)
            {
                case 0:
                case 3:
                {
                    var laneDifference = 4;
                    var closeGroundLane = -1;
                    foreach (var groundLane in groundLanes)
                    {
                        var groundLaneDifference = Math.Abs(groundLane - closestGroundLane);
                        if (groundLaneDifference >= laneDifference) continue;
                        laneDifference = groundLaneDifference;
                        closeGroundLane = groundLane;
                    }

                    if (closeGroundLane == -1) Debug.Log("");
                    return closeGroundLane;
                }
                case 1:
                {
                    if (groundLanes.Contains(1)) return 1;
                    if (groundLanes.Contains(0)) return 0;
                    return groundLanes.Contains(2) ? 2 : 3;
                }
                case 2:
                {
                    if (groundLanes.Contains(2)) return 2;
                    if (groundLanes.Contains(3)) return 3;
                    return groundLanes.Contains(1) ? 1 : 0;
                }
                default:
                {
                    Debug.Log("レーンの数を調整してください。");
                    return -1;
                }
            }
        }

        public void Initialize(ConnectingKinds connectKind)
        {
            if (meshFilter == null) return;

            var lanePositions = RhythmGamePresenter.LanePositions;
            var startPosition = lanePositions[connectKind.connector[0]];
            var spX = startPosition.x;
            var spY = startPosition.y;
            var endPosition = lanePositions[connectKind.connector[1]];
            var edX = endPosition.x;
            var edY = endPosition.y;
            switch (connectKind.kind)
            {
                case "Ground-Ground":
                {
                    _vertices = new Vector3[4];
                    _uv = new Vector3[4];
                    _triangles = new int[6];
                    
                    _triangles[0] = 0;
                    _triangles[1] = 1;
                    _triangles[2] = 2;
                    _triangles[3] = 1;
                    _triangles[4] = 3;
                    _triangles[5] = 2;

                    _vertices[0] = new Vector3(spX, spY - 0.1f, 999);
                    _vertices[1] = new Vector3(edX, edY - 0.1f, 999);
                    _vertices[2] = new Vector3(spX, spY + 0.1f, 999);
                    _vertices[3] = new Vector3(edX, edY + 0.1f, 999);
                    
                    _uv[0] = new Vector2(0, 0);
                    _uv[1] = new Vector2(1, 0);
                    _uv[2] = new Vector2(0, 1);
                    _uv[3] = new Vector2(1, 1);
                    break;
                }
                    
                case "Ground-Above":
                {
                    _vertices = new Vector3[4];
                    _uv = new Vector3[4];
                    _triangles = new int[6];

                    _triangles[0] = 0;
                    _triangles[1] = 1;
                    _triangles[2] = 2;
                    _triangles[3] = 1;
                    _triangles[4] = 3;
                    _triangles[5] = 2;
                    
                    _vertices[0] = new Vector3(spX - 0.1f, spY, 999);
                    _vertices[1] = new Vector3(spX + 0.1f, spY, 999);
                    _vertices[2] = new Vector3(edX - 0.1f, edY, 999);
                    _vertices[3] = new Vector3(edX + 0.1f, edY, 999);
                    
                    _uv[0] = new Vector2(0, 0);
                    _uv[1] = new Vector2(1, 0);
                    _uv[2] = new Vector2(0, 1);
                    _uv[3] = new Vector2(1, 1);
                    
                    break;
                }
                case "Above-Above":
                {
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

                    for (var z = 0; z < 1; z++)
                    {
                        for (var x = 0; x < 33; x++)
                        {
                            var angle = Mathf.PI / Div * x; // レーンの角度

                            angle = Mathf.PI - angle;


                            var innerY = Mathf.Sin(angle) * InnerRadius;
                            var innerX = Mathf.Cos(angle) * InnerRadius;

                            var outerY = Mathf.Sin(angle) * OuterRadius;
                            var outerX = Mathf.Cos(angle) * OuterRadius;



                            //zPos += zz;

                            var innerPoint = new Vector3(innerX, innerY, 0);
                            var outerPoint = new Vector3(outerX, outerY, 0);


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
                        _vertices[66] = new Vector3(-4.6f, -0.25f, 0);
                        _vertices[67] = new Vector3(-4.6f, -0.18f, 0);
                        _vertices[68] = new Vector3(4.6f, -0.25f, 0);
                        _vertices[69] = new Vector3(4.6f, -0.18f, 0);

                        if (_mesh != null) _mesh.vertices = _vertices;
                    }
                    break;
                }
                default:
                {
                    Debug.Log("kind is not valid");
                    break;
                }
            }
            
            // メッシュを生成する
            _mesh = new Mesh
            {
                vertices = _vertices,
                triangles = _triangles
            };
            _mesh.MarkDynamic();
        }
        
        public void Render(float judgeTime, float currentTime, List<SpeedChangeEntity> speedChangeEntities)
        {
            if (!gameObject.activeSelf && judgeTime - currentTime < 5f) gameObject.SetActive(true);
            else if (judgeTime < currentTime) NoteConnectorDestroy();

            var berPos = CalculateBarLinePosition(judgeTime, currentTime, speedChangeEntities).z; // Make Mesh 頂点
            var transPos = transform.position;
            gameObject.transform.position = new Vector3(transPos.x, transPos.y, berPos);

            if (meshFilter != null) return;
            Debug.Log("null");
        }

        private void NoteConnectorDestroy()
        {
            //Debug.Log(this.gameObject);
            RhythmGamePresenter.NoteConnectors.Remove();
            BarLines.Remove(BarLines[0]);
            Destroy(gameObject);
        }
    }
}