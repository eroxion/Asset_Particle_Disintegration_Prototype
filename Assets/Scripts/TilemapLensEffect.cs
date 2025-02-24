using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapLensEffect : MonoBehaviour {
    public Tilemap tilemap;
    public float _reduceFactor = 0.8f;
    public float _vibrationAmount = 0.05f;
    public float _vibrationSpeed = 10f;

    private Dictionary<Vector3Int, Matrix4x4> originalTransforms = new Dictionary<Vector3Int, Matrix4x4>();
    private HashSet<Vector3Int> affectedTiles = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> previouslyAffectedTiles = new HashSet<Vector3Int>();
    private CircleCollider2D lensCollider;

    void Start() {
        lensCollider = GetComponent<CircleCollider2D>();
    }

    void Update() {
        DetectAndApplyLensEffect();
        RestoreTiles();
    }

    void DetectAndApplyLensEffect() {
        affectedTiles.Clear();
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin) {
            if (tilemap.HasTile(pos)) {
                Vector3 worldPos = tilemap.CellToWorld(pos);
                if (IsInsideLens(worldPos)) {
                    affectedTiles.Add(pos);
                    if (!originalTransforms.ContainsKey(pos)) {
                        originalTransforms[pos] = tilemap.GetTransformMatrix(pos); // Store original transform
                    }
                    ApplyTileEffect(pos, worldPos);
                }
            }
        }
        ApplyBrownianMotion();
    }

    void ApplyTileEffect(Vector3Int pos, Vector3 worldPos) {
        Vector3 scaledPos = Vector3.Lerp(worldPos, transform.position, 1 - _reduceFactor);
        tilemap.SetTransformMatrix(pos, Matrix4x4.TRS(scaledPos, Quaternion.identity, Vector3.one * _reduceFactor));
    }

    void ApplyBrownianMotion() {
        float timeFactor = Time.time * _vibrationSpeed;
        foreach (Vector3Int pos in affectedTiles) {
            if (tilemap.HasTile(pos)) {
                Matrix4x4 originalMatrix = originalTransforms[pos];
                Vector3 originalPos = originalMatrix.GetColumn(3);  // Extract position from transform matrix

                float offsetX = (Mathf.PerlinNoise(timeFactor, pos.x) - 0.5f) * _vibrationAmount;
                float offsetY = (Mathf.PerlinNoise(timeFactor, pos.y) - 0.5f) * _vibrationAmount;

                tilemap.SetTransformMatrix(pos, Matrix4x4.TRS(originalPos + new Vector3(offsetX, offsetY, 0), Quaternion.identity, Vector3.one * _reduceFactor));
            }
        }
    }

    void RestoreTiles() {
        HashSet<Vector3Int> tilesToRestore = new HashSet<Vector3Int>(previouslyAffectedTiles);
        tilesToRestore.ExceptWith(affectedTiles);

        foreach (Vector3Int pos in tilesToRestore) {
            if (tilemap.HasTile(pos) && originalTransforms.ContainsKey(pos)) {
                tilemap.SetTransformMatrix(pos, originalTransforms[pos]); // Restore exact original transform
                originalTransforms.Remove(pos);
            }
        }

        previouslyAffectedTiles = new HashSet<Vector3Int>(affectedTiles);
    }

    bool IsInsideLens(Vector3 worldPos) {
        float radius = lensCollider.radius * transform.lossyScale.x;
        return Vector3.Distance(transform.position, worldPos) <= radius;
    }
}
