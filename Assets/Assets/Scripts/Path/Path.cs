using System;
using System.Collections.Generic;
using System.Linq;
using KabreetGames.SceneReferences;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KabreetGames.PathSystem
{
    public class Path : ValidatedMonoBehaviour
    {
        [SerializeField, HideInInspector] private List<PathPoint> points = new();

        [SerializeField] private bool loop;
        [SerializeField] private bool useBezier;
        [SerializeField] private bool useWorldSpace;
        [SerializeField] private bool showGizmos = true;
        public bool UseWorldSpace => useWorldSpace;

        private readonly Dictionary<object, int> currentPointIndexCache = new();

        [Button]
        public void AddPoint()
        {
            var position = useWorldSpace ? transform.position : Vector3.zero;
            points.Add(new PathPoint(this, position));
        }

        public void AddPoint(int index)
        {
            Vector3 position;

            if (index < 0 || index > points.Count)
            {
                if (points.Count > 1)
                {
                    position = (points[index - 1].Position + points[index].Position) / 2f;
                }
                else
                {
                    position = points.Count > 0 ? points[0].Position : transform.position;
                }
            }
            else if (index == points.Count)
            {
                position = points[^1].Position + new Vector3(1f, 0, 0);
            }
            else
            {
                position = (points[index - 1].Position + points[index].Position) / 2f;
            }

            if (!useWorldSpace)
                position = transform.InverseTransformPoint(position);

            points.Insert(index, new PathPoint(this, position));
        }

        public void AddPoint(Vector3 position)
        {
            points.Add(new PathPoint(this, position));
        }

        public void AddPoint(PathPoint point)
        {
            points.Add(point);
        }

        public void AddPoints(IEnumerable<PathPoint> ps)
        {
            points.AddRange(ps);
        }

        public void AddPoints(IEnumerable<Vector3> ps)
        {
            points.AddRange(ps.Select(p => new PathPoint(this, p)));
        }

        public void RemovePoint(int index)
        {
            points.RemoveAt(index);
        }

        public void RemovePoint(PathPoint point)
        {
            points.Remove(point);
        }

        public void RemovePoints(IEnumerable<PathPoint> ps)
        {
            points.RemoveAll(ps.Contains);
        }

        public void RemovePoints(IEnumerable<Vector3> ps)
        {
            points.RemoveAll(p => ps.Contains(p.Position));
        }


        public PathPoint[] Points => points.ToArray();

        public int Count => points.Count;

        public PathPoint this[int index]
        {
            get => points[index];
            set => points[index] = value;
        }

        public void Clear()
        {
            points.Clear();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showGizmos)
                return;

            var sceneView = UnityEditor.SceneView.lastActiveSceneView;
            if (sceneView == null)
                return;

            var sceneCamera = sceneView.camera;
            const float minGizmoSize = 0.01f;
            const float maxGizmoSize = 0.1f;
            const float distanceFactor = 0.01f;

            for (var i = 0; i < points.Count; i++)
            {
                var p = points[i];
                var distanceToCamera = Vector3.Distance(sceneCamera.transform.position, p.Position);
                var gizmoSize = Mathf.Clamp(distanceToCamera * distanceFactor, minGizmoSize, maxGizmoSize);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(p.Position, gizmoSize);

                if (i <= 0) continue;
                Gizmos.color = Color.white;
                Gizmos.DrawLine(points[i - 1].Position, p.Position);
            }

            if (!useBezier) return;
            const int segments = 20;

            var fullBezierSegmentCount = (points.Count - 1) / 3;
            for (var i = 0; i < fullBezierSegmentCount; i++)
            {
                var p0 = points[i * 3].Position;
                var p1 = points[i * 3 + 1].Position;
                var p2 = points[i * 3 + 2].Position;
                var p3 = points[i * 3 + 3].Position;

                var prevPoint = BezierCurve.GetPoint(p0, p1, p2, p3, 0f);
                for (var j = 1; j <= segments; j++)
                {
                    var t = j / (float)segments;
                    var nextPoint = BezierCurve.GetPoint(p0, p1, p2, p3, t);
                    Gizmos.DrawRay(prevPoint, (nextPoint - prevPoint) * 0.7f);
                    prevPoint = nextPoint;
                }
            }

            var start = fullBezierSegmentCount * 3;
            var remainingPoints = points.Count - start;

            switch (remainingPoints)
            {
                case 2:
                {
                    var p0 = points[start].Position;
                    var p1 = points[start + 1].Position;
                    Gizmos.DrawLine(p0, p1);
                    Gizmos.DrawRay(p0, (p1 - p0) * 0.7f);

                    break;
                }
                case 3:
                {
                    var p0 = points[start].Position;
                    var p1 = points[start + 1].Position;
                    var p2 = points[start + 2].Position;

                    var prevPoint = BezierCurve.GetPoint(p0, p1, p2, 0f);
                    for (var j = 1; j <= segments; j++)
                    {
                        var t = j / (float)segments;
                        var nextPoint = BezierCurve.GetPoint(p0, p1, p2, t);
                        Gizmos.DrawRay(prevPoint, (nextPoint - prevPoint) * 0.7f);
                        prevPoint = nextPoint;
                    }

                    break;
                }
            }
        }
#endif

        public void UsePath(object obj)
        {
            currentPointIndexCache.TryAdd(obj, 0);
        }

        public void UnUsePath(object obj)
        {
            currentPointIndexCache.Remove(obj);
        }

        public Vector3 GetNextPoint(object obj, bool skipFirstPoint = true)
        {
            if (points.Count == 0) return transform.position;

            if (!currentPointIndexCache.TryGetValue(obj, out var currentPointIndex))
            {
                currentPointIndex = 0;
                currentPointIndexCache.Add(obj, currentPointIndex);
                if (!skipFirstPoint)
                {
                    return points[currentPointIndex].Position;
                }
            }

            currentPointIndex++;

            if (currentPointIndex >= points.Count)
            {
                if (loop)
                {
                    currentPointIndex = 0;
                }
                else
                {
                    currentPointIndex = points.Count - 1;
                }
            }

            currentPointIndexCache[obj] = currentPointIndex;
            return points[currentPointIndex].Position;
        }

        public Vector3 GetLerpFromPoint(int i, float f)
        {
            if (points.Count == 0) return transform.position;
            if (i < points.Count) return Vector3.Lerp(points[i].Position, points[i + 1].Position, f);
            if (loop)
            {
                i = 0;
            }
            else
            {
                i = points.Count - 1;
            }

            return Vector3.Lerp(points[i].Position, points[i + 1].Position, f);
        }

        public Vector3 GetLerpFromCurrentPoint(object obj, float f)
        {
            if (points.Count == 0) return transform.position;
            var currentPointIndex = currentPointIndexCache[obj];
            if (currentPointIndex < points.Count)
                return Vector3.Lerp(points[currentPointIndex].Position, points[currentPointIndex + 1].Position, f);
            if (loop)
            {
                currentPointIndex = 0;
            }
            else
            {
                currentPointIndex = points.Count - 1;
            }

            return Vector3.Lerp(points[currentPointIndex].Position, points[currentPointIndex + 1].Position, f);
        }


        public Vector3 GetLerpFromAllPath(float f)
        {
            if (points.Count == 0) return transform.position;
            f = Mathf.Clamp01(f);
            if (useBezier && points.Count >= 3)
            {
                return GetPositionOnBezierPath(f);
            }

            var totalSegments = points.Count - 1;
            var percent = f * totalSegments;
            var index = (int)percent;
            var segmentProgress = percent - index;
            var nextIndex = Mathf.Min(index + 1, points.Count - 1);
            return Vector3.Lerp(points[index].Position, points[nextIndex].Position, segmentProgress);
        }

        private Vector3 GetPositionOnBezierPath(float t)
        {
            var fullBezierSegmentCount = (points.Count - 1) / 3;
            float totalSegments = fullBezierSegmentCount;

            var remainingPoints = points.Count - (fullBezierSegmentCount * 3);
            switch (remainingPoints)
            {
                case 2:
                case 3:
                    totalSegments += 1f;
                    break;
            }

            var segmentT = t * totalSegments;
            var segmentIndex = Mathf.Min((int)segmentT, fullBezierSegmentCount);

            var localT = segmentT - segmentIndex;

            if (segmentIndex < fullBezierSegmentCount)
            {
                var startIndex = segmentIndex * 3;
                var p0 = points[startIndex].Position;
                var p1 = points[startIndex + 1].Position;
                var p2 = points[startIndex + 2].Position;
                var p3 = points[startIndex + 3].Position;

                return BezierCurve.GetPoint(p0, p1, p2, p3, localT);
            }

            var start = fullBezierSegmentCount * 3;

            switch (remainingPoints)
            {
                case 2:
                {
                    var p0 = points[start].Position;
                    var p1 = points[start + 1].Position;
                    return Vector3.Lerp(p0, p1, localT);
                }
                case 3:
                {
                    var p0 = points[start].Position;
                    var p1 = points[start + 1].Position;
                    var p2 = points[start + 2].Position;
                    return BezierCurve.GetPoint(p0, p1, p2, localT);
                }
                default:
                    return points[^1].Position;
            }
        }
    }


    [Serializable]
    public struct PathPoint
    {
        [SerializeField] private Vector3 position;
        [SerializeField] private Path path;

        public PathPoint(Path path, Vector3 position)
        {
            this.position = position;
            this.path = path;
        }

        public Vector3 Position
        {
            get => path.UseWorldSpace ? position : path.transform.position + position;
            set
            {
                if (path.UseWorldSpace)
                    position = value;
                else
                {
                    position = value - path.transform.position;
                }
            }
        }
    }

    public static class BezierCurve
    {
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            var u = 1f - t;
            var tt = t * t;
            var uu = u * u;
            var uuu = uu * u;
            var ttt = tt * t;

            var p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            var u = 1f - t;
            var tt = t * t;
            var uu = u * u;

            var p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        }
    }
}