using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
   public class TerrainChunk
   {
      GameObject meshObject;
      Vector2 position;
      Bounds bounds;

      MeshRenderer meshRenderer;
      MeshFilter meshFilter;
      LODInfo[] detailLevels;
      LODMesh[] lodMeshes;
      MapData mapData;
      bool mapDataReceived;
      int previousLODIndex = -1;

      public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
      {
         this.detailLevels = detailLevels;

         position = coord * size;
         bounds = new Bounds(position, Vector2.one * size);
         Vector3 positionV3 = new Vector3(position.x, 0, position.y);

         meshObject = new GameObject("Terrain Chunk");
         meshRenderer = meshObject.AddComponent<MeshRenderer>();
         meshFilter = meshObject.AddComponent<MeshFilter>();
         meshRenderer.material = material;
         meshObject.transform.position = positionV3;
         meshObject.transform.parent = parent;
         SetVisible(false);

         lodMeshes = new LODMesh[detailLevels.Length];
         for(int i = 0; i < detailLevels.Length; i++) {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
         }

         try {
            EndlessTerrain.mapGenerator.RequestMapData(OnMapDataReceived);
         } catch (Exception e) {
            Console.WriteLine(e.Message);
         }
      }

      void OnMapDataReceived(MapData mapData)
      {
         //EndlessTerrain.mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
         this.mapData = mapData;
         mapDataReceived = true;
      }

      void OnMeshDataReceived(MeshData meshData)
      {
         meshFilter.mesh = meshData.CreateMesh();
      }

      public void UpdateTerrainChunk(Vector2 viewerPosition, float maxViewDistance)
      {
         if (mapDataReceived) {
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistanceFromNearestEdge <= maxViewDistance;

            if (visible) {
               int lodIndex = 0;

               for (int i = 0; i < detailLevels.Length - 1; i++) {
                  if (viewerDistanceFromNearestEdge > detailLevels[i].visibleDistanceThreshold) {
                     lodIndex = i + 1;
                  } else {
                     break;
                  }
               }

               //MESH CREATION
               if (lodIndex != previousLODIndex) {
                  LODMesh lodMesh = lodMeshes[lodIndex];
                  if (lodMesh.hasMesh) {
                     previousLODIndex = lodIndex;
                     meshFilter.mesh = lodMesh.mesh;
                  } else if (!lodMesh.hasRequestedMesh) {
                     lodMesh.RequestMesh(mapData);
                  }
               }
            }
            SetVisible(visible);
         }
      }

      public void SetVisible(bool visible)
      {
         meshObject.SetActive(visible);
      }

      public bool IsVisible()
      {
         return meshObject.activeSelf;
      }
   }
}
