#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reilas
{
    public class Connector
    {
        public float currentTime;
        public List<ConnectingKinds> connectingList = new List<ConnectingKinds>();
    }

    public class ConnectingKinds
    {
        public int[] connector = new int[2];
        public string? kind;
    }
    
    public sealed class NoteConnector : MonoBehaviour
    {
        public float judgeTime;
        
        [SerializeField] private MeshFilter meshFilter = null!;

        private Vector3[]? _vertices;
        private Vector3[]? _uv;
        
        private int[]? _triangles;

        private bool _kujo;
        
        private const float Div = 32f;
        private const float OuterLaneRadius = 4.4f;

        private Mesh? _mesh;

        // 内縁の半径
        private const float InnerRadius = OuterLaneRadius - 0.15f;
        
        // 外縁の半径
        private const float OuterRadius = OuterLaneRadius;

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
                    return -1;
                }
            }
        }

        public void Initialize(Connector connector, bool kujo)
        {
            judgeTime = connector.currentTime;
            _kujo = kujo;
            
            foreach (var connectingKind in connector.connectingList)
            {
                InitializeMesh(connectingKind);
                gameObject.transform.position = new Vector3(0f, 0f, 999f);
            }
        }

        private void InitializeMesh(ConnectingKinds connectKind)
        {
            if (meshFilter == null) return;

            var beginning = connectKind.connector[0];
            var finish = connectKind.connector[1];
            if (finish > 35) finish = 35;

            var lanePositions = RhythmGamePresenter.LanePositions;
            var startPosition = lanePositions[beginning];
            var spX = startPosition.x;
            var spY = startPosition.y;
            var endPosition = lanePositions[finish];
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
                    _triangles[1] = 2;
                    _triangles[2] = 1;
                    _triangles[3] = 2;
                    _triangles[4] = 3;
                    _triangles[5] = 1;

                    _vertices[0] = new Vector3(spX, spY - 0.05f, 0f);
                    _vertices[1] = new Vector3(edX, edY - 0.05f, 0f);
                    _vertices[2] = new Vector3(spX, spY + 0.05f, 0f);
                    _vertices[3] = new Vector3(edX, edY + 0.05f, 0f);

                    _uv[0] = new Vector2(0, 0);
                    _uv[1] = new Vector2(0, 1);
                    _uv[2] = new Vector2(1, 1);
                    _uv[3] = new Vector2(1, 0);
                    break;
                }
                case "Ground-Above":
                {
                    _vertices = new Vector3[4];
                    _uv = new Vector3[4];
                    _triangles = new int[6];

                    _triangles[0] = 0;
                    _triangles[1] = 2;
                    _triangles[2] = 1;
                    _triangles[3] = 2;
                    _triangles[4] = 3;
                    _triangles[5] = 1;

                    _vertices[0] = new Vector3(spX - 0.05f, spY, 0f);
                    _vertices[1] = new Vector3(spX + 0.05f, spY, 0f);
                    _vertices[2] = new Vector3(edX - 0.05f, edY - 0.2f, 0f);
                    _vertices[3] = new Vector3(edX + 0.05f, edY - 0.2f, 0f);

                    _uv[0] = new Vector2(0, 0);
                    _uv[1] = new Vector2(0, 1);
                    _uv[2] = new Vector2(1, 1);
                    _uv[3] = new Vector2(1, 0);

                    break;
                }
                case "Above-Above":
                {
                    beginning -= 4;
                    finish -= 4;
                    var size = finish - beginning + 1;
                    var entityBase = size * 10;
                    var uAndV = entityBase * 2;

                    _vertices = new Vector3[uAndV];
                    _uv = new Vector3[uAndV];
                    _triangles = new int[entityBase * 6 + 12];

                    // 前面
                    for (var i = 0; i < size - 1; i++)
                    {
                        _triangles[i * 6 + 0] = 0 + i * 2;
                        _triangles[i * 6 + 1] = 1 + i * 2;
                        _triangles[i * 6 + 2] = 3 + i * 2;
                        _triangles[i * 6 + 3] = 2 + i * 2;
                        _triangles[i * 6 + 4] = 0 + i * 2;
                        _triangles[i * 6 + 5] = 3 + i * 2;
                    }

                    for (var x = 0; x < size; x++)
                    {
                        var laneIndex = beginning + x;
                        
                        // レーンの角度
                        var angleBase = Div - laneIndex;
                        var angle = Mathf.PI * (angleBase / Div);

                        var innerY = Mathf.Sin(angle) * InnerRadius;
                        var innerX = Mathf.Cos(angle) * InnerRadius;

                        var outerY = Mathf.Sin(angle) * OuterRadius;
                        var outerX = Mathf.Cos(angle) * OuterRadius;

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
                    
                    if (_mesh != null) _mesh.vertices = _vertices;
                    break;
                }
            default:
            {
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

        _mesh.vertices = _vertices;
        
        _mesh.SetUVs(0, _uv);
        
#if UNITY_EDITOR
        _mesh.RecalculateBounds();
#endif
        meshFilter.mesh = _mesh;
        }
        
        public void Render(float currentTime)
        {
            if (!gameObject.activeSelf && judgeTime - currentTime < 5f) gameObject.SetActive(true);
            else if (judgeTime < currentTime) NoteConnectorDestroy(_kujo);

            // Make Mesh 頂点
            var berPos = NotePositionCalculatorService.GetPosition(judgeTime, currentTime, 1);
            gameObject.transform.position = new Vector3(0, 0, berPos);

            if (meshFilter != null) return;
        }

        public void NoteConnectorDestroy(bool kujo)
        {
            if (kujo) RhythmGamePresenter.NoteKujoConnectors.Remove(this);
            else RhythmGamePresenter.NoteConnectors.Remove(this);
            
            Destroy(gameObject);
        }
    }
}