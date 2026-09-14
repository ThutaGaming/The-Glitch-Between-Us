using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Walkability grid + A* for enemies in rooms that have no NavMesh. Built once from physics: a
/// cell is walkable when there's floor under it and an upright agent capsule fits there, so it
/// automatically respects every barrier, crate, column and bench the level designer places.
/// </summary>
public class CoverNavGrid
{
    private struct HeapItem
    {
        public int Index;
        public float F;
    }

    private static readonly int[] DX = { 1, -1, 0, 0, 1, 1, -1, -1 };
    private static readonly int[] DZ = { 0, 0, 1, -1, 1, -1, 1, -1 };

    private readonly Vector3 origin;
    private readonly float cellSize;
    private readonly int width;
    private readonly int depth;
    private readonly bool[] walkable;

    private readonly float[] gScore;
    private readonly int[] cameFrom;
    private readonly int[] visitStamp;
    private readonly int[] closedStamp;
    private readonly List<HeapItem> heap = new List<HeapItem>();
    private readonly List<Vector3> cellPath = new List<Vector3>();
    private int stamp;

    public int Width => width;
    public int Depth => depth;
    public float FloorY => origin.y;

    public CoverNavGrid(Vector3 min, Vector3 max, float floorY, float cellSize, float agentRadius, float agentHeight, int mask, Func<Collider, bool> ignoreCollider)
    {
        this.cellSize = cellSize;
        width = Mathf.Max(1, Mathf.CeilToInt((max.x - min.x) / cellSize));
        depth = Mathf.Max(1, Mathf.CeilToInt((max.z - min.z) / cellSize));
        origin = new Vector3(min.x + cellSize * 0.5f, floorY, min.z + cellSize * 0.5f);

        int count = width * depth;
        walkable = new bool[count];
        gScore = new float[count];
        cameFrom = new int[count];
        visitStamp = new int[count];
        closedStamp = new int[count];

        var overlaps = new Collider[32];
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 p = CellCenter(x, z);
                walkable[z * width + x] = HasFloor(p, mask, ignoreCollider)
                    && IsClear(p, agentRadius, agentHeight, mask, ignoreCollider, overlaps);
            }
        }
    }

    public Vector3 CellCenter(int x, int z)
    {
        return new Vector3(origin.x + x * cellSize, origin.y, origin.z + z * cellSize);
    }

    public bool IsCellWalkable(int x, int z)
    {
        return x >= 0 && z >= 0 && x < width && z < depth && walkable[z * width + x];
    }

    public bool IsWalkable(Vector3 p)
    {
        return WorldToCell(p, out int x, out int z) && walkable[z * width + x];
    }

    public bool TryGetNearestWalkable(Vector3 p, int maxRing, out Vector3 result)
    {
        result = new Vector3(p.x, origin.y, p.z);
        if (IsWalkable(p)) return true;

        WorldToCell(p, out int cx, out int cz);
        int bestIndex = -1;
        float bestDist = float.MaxValue;
        for (int ring = 1; ring <= maxRing && bestIndex < 0; ring++)
        {
            for (int dz = -ring; dz <= ring; dz++)
            {
                for (int dx = -ring; dx <= ring; dx++)
                {
                    if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dz)) != ring) continue;
                    int x = cx + dx, z = cz + dz;
                    if (!IsCellWalkable(x, z)) continue;
                    float d = (CellCenter(x, z) - result).sqrMagnitude;
                    if (d < bestDist)
                    {
                        bestDist = d;
                        bestIndex = z * width + x;
                    }
                }
            }
        }

        if (bestIndex < 0) return false;
        result = CellCenter(bestIndex % width, bestIndex / width);
        return true;
    }

    /// <summary>Fills <paramref name="path"/> with smoothed world waypoints (start excluded).</summary>
    public bool FindPath(Vector3 start, Vector3 goal, List<Vector3> path)
    {
        path.Clear();
        if (!TryGetNearestWalkable(start, 8, out Vector3 s) || !TryGetNearestWalkable(goal, 8, out Vector3 g))
            return false;

        WorldToCell(s, out int sx, out int sz);
        WorldToCell(g, out int gx, out int gz);
        int startIndex = sz * width + sx;
        int goalIndex = gz * width + gx;

        stamp++;
        heap.Clear();
        gScore[startIndex] = 0f;
        cameFrom[startIndex] = -1;
        visitStamp[startIndex] = stamp;
        HeapPush(startIndex, Heuristic(sx, sz, gx, gz));

        bool found = false;
        while (heap.Count > 0)
        {
            int current = HeapPop();
            if (closedStamp[current] == stamp) continue;
            closedStamp[current] = stamp;
            if (current == goalIndex)
            {
                found = true;
                break;
            }

            int cx = current % width, cz = current / width;
            for (int k = 0; k < 8; k++)
            {
                int nx = cx + DX[k], nz = cz + DZ[k];
                if (!IsCellWalkable(nx, nz)) continue;
                int ni = nz * width + nx;
                if (closedStamp[ni] == stamp) continue;
                // Diagonals may not cut past a blocked corner.
                if (k >= 4 && (!walkable[cz * width + nx] || !walkable[nz * width + cx])) continue;

                float tentative = gScore[current] + (k >= 4 ? 1.41421f : 1f);
                if (visitStamp[ni] == stamp && tentative >= gScore[ni]) continue;
                visitStamp[ni] = stamp;
                gScore[ni] = tentative;
                cameFrom[ni] = current;
                HeapPush(ni, tentative + Heuristic(nx, nz, gx, gz));
            }
        }

        if (!found) return false;

        cellPath.Clear();
        for (int i = goalIndex; i >= 0; i = cameFrom[i])
            cellPath.Add(CellCenter(i % width, i / width));
        cellPath.Reverse();
        cellPath[0] = s;
        if (cellPath.Count > 1) cellPath[cellPath.Count - 1] = g;
        else cellPath.Add(g);

        int anchor = 0;
        while (anchor < cellPath.Count - 1)
        {
            int next = anchor + 1;
            for (int i = cellPath.Count - 1; i > anchor + 1; i--)
            {
                if (IsSegmentWalkable(cellPath[anchor], cellPath[i]))
                {
                    next = i;
                    break;
                }
            }
            path.Add(cellPath[next]);
            anchor = next;
        }
        return true;
    }

    public bool IsSegmentWalkable(Vector3 a, Vector3 b)
    {
        Vector3 d = b - a;
        d.y = 0f;
        int steps = Mathf.CeilToInt(d.magnitude / (cellSize * 0.35f));
        for (int i = 0; i <= steps; i++)
        {
            Vector3 p = a + d * (steps == 0 ? 0f : (float)i / steps);
            if (!IsWalkable(p)) return false;
        }
        return true;
    }

    private bool WorldToCell(Vector3 p, out int x, out int z)
    {
        x = Mathf.FloorToInt((p.x - origin.x) / cellSize + 0.5f);
        z = Mathf.FloorToInt((p.z - origin.z) / cellSize + 0.5f);
        return x >= 0 && z >= 0 && x < width && z < depth;
    }

    private static float Heuristic(int x1, int z1, int x2, int z2)
    {
        int dx = Mathf.Abs(x1 - x2), dz = Mathf.Abs(z1 - z2);
        return dx + dz - 0.58579f * Mathf.Min(dx, dz);
    }

    private static readonly Vector3[] FloorProbeOffsets =
    {
        Vector3.zero, new Vector3(0.3f, 0f, 0f), new Vector3(-0.3f, 0f, 0f), new Vector3(0f, 0f, 0.3f), new Vector3(0f, 0f, -0.3f)
    };

    // Probes a small cross so thin seams between floor pieces (e.g. under a door threshold)
    // don't cut the grid in two.
    private static bool HasFloor(Vector3 p, int mask, Func<Collider, bool> ignore)
    {
        foreach (var offset in FloorProbeOffsets)
            if (HasFloorAt(p + offset, mask, ignore)) return true;
        return false;
    }

    private static bool HasFloorAt(Vector3 p, int mask, Func<Collider, bool> ignore)
    {
        var hits = Physics.RaycastAll(p + Vector3.up * 0.7f, Vector3.down, 1.4f, mask, QueryTriggerInteraction.Ignore);
        float nearest = float.MaxValue;
        bool floorFound = false;
        foreach (var h in hits)
        {
            if (ignore != null && ignore(h.collider)) continue;
            if (h.distance < nearest)
            {
                nearest = h.distance;
                floorFound = Mathf.Abs(h.point.y - p.y) < 0.3f;
            }
        }
        return floorFound;
    }

    private static bool IsClear(Vector3 p, float radius, float height, int mask, Func<Collider, bool> ignore, Collider[] buffer)
    {
        // Bottom sphere lifted off the floor so the floor itself never counts as an obstacle,
        // but low enough that knee-high benches still do.
        Vector3 a = p + Vector3.up * (radius + 0.12f);
        Vector3 b = p + Vector3.up * Mathf.Max(radius + 0.13f, height - radius);
        int n = Physics.OverlapCapsuleNonAlloc(a, b, radius, buffer, mask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < n; i++)
        {
            if (ignore == null || !ignore(buffer[i])) return false;
        }
        return true;
    }

    private void HeapPush(int index, float f)
    {
        heap.Add(new HeapItem { Index = index, F = f });
        int i = heap.Count - 1;
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (heap[parent].F <= heap[i].F) break;
            HeapItem tmp = heap[parent];
            heap[parent] = heap[i];
            heap[i] = tmp;
            i = parent;
        }
    }

    private int HeapPop()
    {
        int result = heap[0].Index;
        int last = heap.Count - 1;
        heap[0] = heap[last];
        heap.RemoveAt(last);
        int i = 0;
        while (true)
        {
            int l = i * 2 + 1, r = l + 1, smallest = i;
            if (l < heap.Count && heap[l].F < heap[smallest].F) smallest = l;
            if (r < heap.Count && heap[r].F < heap[smallest].F) smallest = r;
            if (smallest == i) break;
            HeapItem tmp = heap[smallest];
            heap[smallest] = heap[i];
            heap[i] = tmp;
            i = smallest;
        }
        return result;
    }
}
