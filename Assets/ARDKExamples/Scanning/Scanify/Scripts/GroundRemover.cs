using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ARDKExamples.Scanning
{
    public class GroundRemover : MonoBehaviour
    {
        public static Mesh RemoveGroundPlane(Mesh mesh)
        {
            // Maximum angle from flat for a triangle to be considered ground.
            const float maxAngleDeg = 5.0f;
            var cosMaxAngle = (float)Math.Cos(maxAngleDeg * Math.PI / 180.0);

            // Extract components from the mesh.
            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var uvs = new List<Vector2>();
            mesh.GetVertices(vertices);
            mesh.GetTriangles(indices, 0);
            mesh.GetUVs(0, uvs);

            // Find triangles that are facing upwards and are in the bottom 10% of the bound after discarding outliers.
            var coordsY = vertices.Select((v) => v.y).ToList();
            coordsY.Sort();
            var minY = coordsY[(int)(coordsY.Count * 0.02f)];
            var maxY = coordsY[(int)(coordsY.Count * 0.98f)];
            var limitY = minY + 0.1 * (maxY - minY);
            var totalArea = 0.0f;
            var normalSum = new Vector3();
            var groundPoints = new List<Vector3>();
            for (var i = 0; i < indices.Count;)
            {
                var v0 = vertices[indices[i++]];
                var v1 = vertices[indices[i++]];
                var v2 = vertices[indices[i++]];
                var maxVertexY = Math.Max(Math.Max(v0.y, v1.y), v2.y);
                if (maxVertexY > limitY) continue;

                var normal = Vector3.Cross(v1 - v0, v2 - v0);
                var isUpFacing = normal.normalized.y >= cosMaxAngle;
                if (isUpFacing)
                {
                    var area = 0.5f * normal.magnitude;
                    totalArea += area;
                    normalSum += normal;
                    groundPoints.Add(v0);
                    groundPoints.Add(v1);
                    groundPoints.Add(v2);
                }
            }

            // If the total ground area isn't at least 20% of the cross section, don't remove anything.
            if (totalArea < 0.2 * mesh.bounds.size.x * mesh.bounds.size.z)
            {
                return mesh;
            }
            var planeNormal = normalSum.normalized;

            // Find the point on the ground that's farthest above the plane.
            var maxGroundPointIndex = 0;
            var maxDot = -1.0f;
            for (var i = 0; i < groundPoints.Count; i++)
            {
                var dot = Vector3.Dot(planeNormal, groundPoints[i]);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    maxGroundPointIndex = i;
                }
            }
            var planePoint = groundPoints[maxGroundPointIndex];

            // Clip portions of the mesh that are below the plane.
            var clippedIndices = new List<int>();
            for (var i = 0; i < indices.Count;)
            {
                var i0 = indices[i++];
                var i1 = indices[i++];
                var i2 = indices[i++];
                var inside0 = Vector3.Dot(planeNormal, vertices[i0] - planePoint) > 0;
                var inside1 = Vector3.Dot(planeNormal, vertices[i1] - planePoint) > 0;
                var inside2 = Vector3.Dot(planeNormal, vertices[i2] - planePoint) > 0;
                var numInside = (inside0 ? 1 : 0) + (inside1 ? 1 : 0) + (inside2 ? 1 : 0);
                if (numInside == 3)
                {
                    clippedIndices.Add(i0);
                    clippedIndices.Add(i1);
                    clippedIndices.Add(i2);
                }
                else if (numInside == 2)
                {
                    // This case is a trapezoid that needs to be split into two triangles (A,1,2) and (B,A,2)
                    //             0
                    // outside    / \
                    // - - - - - A - B - - -   ==>  - - - A--B - - -
                    // inside   /     \                  / \_ \        
                    //         /       \                /    \_\
                    //        1---------2              1-------\2
                    Vector3 p0, p1, p2;
                    Vector2 uv0, uv1, uv2;
                    int corner1, corner2;
                    if (!inside0)
                    {
                        p0 = vertices[i0];
                        p1 = vertices[i1];
                        p2 = vertices[i2];
                        uv0 = uvs[i0];
                        uv1 = uvs[i1];
                        uv2 = uvs[i2];
                        corner1 = i1;
                        corner2 = i2;
                    }
                    else if (!inside1)
                    {
                        p0 = vertices[i1];
                        p1 = vertices[i2];
                        p2 = vertices[i0];
                        uv0 = uvs[i1];
                        uv1 = uvs[i2];
                        uv2 = uvs[i0];
                        corner1 = i2;
                        corner2 = i0;
                    }
                    else
                    {
                        p0 = vertices[i2];
                        p1 = vertices[i0];
                        p2 = vertices[i1];
                        uv0 = uvs[i2];
                        uv1 = uvs[i0];
                        uv2 = uvs[i1];
                        corner1 = i0;
                        corner2 = i1;
                    }
                    var a = GetInterpolationFactor(planePoint, planeNormal, p0, p1);
                    var b = GetInterpolationFactor(planePoint, planeNormal, p0, p2);
                    var nextIndex = vertices.Count;
                    vertices.Add(p0 + (p1 - p0) * a);
                    vertices.Add(p0 + (p2 - p0) * b);
                    uvs.Add(uv0 + (uv1 - uv0) * a);
                    uvs.Add(uv0 + (uv2 - uv0) * b);
                    clippedIndices.Add(nextIndex);
                    clippedIndices.Add(corner1);
                    clippedIndices.Add(corner2);
                    clippedIndices.Add(nextIndex + 1);
                    clippedIndices.Add(nextIndex);
                    clippedIndices.Add(corner2);
                }
                else if (numInside == 1)
                {
                    // This case is a triangle that just needs to be cropped to the boundary (0,A,B)
                    //             0                        0
                    // inside     / \                      / \
                    // - - - - - A - B - - -  ===>  - - - A---B - - -
                    // outside  /     \
                    //         1-------2
                    Vector3 p0, p1, p2;
                    Vector2 uv0, uv1, uv2;
                    int corner0;
                    if (inside0)
                    {
                        p0 = vertices[i0];
                        p1 = vertices[i1];
                        p2 = vertices[i2];
                        uv0 = uvs[i0];
                        uv1 = uvs[i1];
                        uv2 = uvs[i2];
                        corner0 = i0;
                    }
                    else if (inside1)
                    {
                        p0 = vertices[i1];
                        p1 = vertices[i2];
                        p2 = vertices[i0];
                        uv0 = uvs[i1];
                        uv1 = uvs[i2];
                        uv2 = uvs[i0];
                        corner0 = i1;
                    }
                    else
                    {
                        p0 = vertices[i2];
                        p1 = vertices[i0];
                        p2 = vertices[i1];
                        uv0 = uvs[i2];
                        uv1 = uvs[i0];
                        uv2 = uvs[i1];
                        corner0 = i2;
                    }
                    var a = GetInterpolationFactor(planePoint, planeNormal, p0, p1);
                    var b = GetInterpolationFactor(planePoint, planeNormal, p0, p2);
                    var nextIndex = vertices.Count;
                    vertices.Add(p0 + (p1 - p0) * a);
                    vertices.Add(p0 + (p2 - p0) * b);
                    uvs.Add(uv0 + (uv1 - uv0) * a);
                    uvs.Add(uv0 + (uv2 - uv0) * b);
                    clippedIndices.Add(corner0);
                    clippedIndices.Add(nextIndex);
                    clippedIndices.Add(nextIndex + 1);
                }
            }

            // After clipping, some vertices may no longer be referenced and can be removed.
            var outVertices = new List<Vector3>();
            var outUVs = new List<Vector2>();
            var usedIndices = new HashSet<int>(clippedIndices);
            var indexMap = new Dictionary<int, int>();
            for (var i = 0; i < vertices.Count; i++)
            {
                if (usedIndices.Contains(i))
                {
                    indexMap[i] = outVertices.Count;
                    outVertices.Add(vertices[i]);
                    outUVs.Add(uvs[i]);
                }
            }
            var outIndices = clippedIndices.Select((i) => indexMap[i]).ToList();

            // Build and return a mesh.
            var result = new Mesh();
            result.SetVertices(outVertices);
            result.SetUVs(0, outUVs);
            result.SetTriangles(outIndices, 0);
            return result;
        }

        // Returns a value between 0 and 1 that will produce a point on the plane defined
        // by point p and normal n when interpolating between points a and b.
        private static float GetInterpolationFactor(Vector3 p, Vector3 n, Vector3 a, Vector3 b)
        {
            var distA = Vector3.Dot(n, a - p);
            var distB = Vector3.Dot(n, b - p);
            return distA / (distA - distB);
        }
    }
}

