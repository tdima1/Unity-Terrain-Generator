using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
   class LODMesh
   {
      public Mesh mesh;
      public bool hasRequestedMesh;
      public bool hasMesh;
      int lod;

      public LODMesh(int lod)
      {
         this.lod = lod;
      }

      void OnMeshDataReceived(MeshData meshData)
      {
         mesh = meshData.CreateMesh();
         hasMesh = true;
      }

      public void RequestMesh(MapData mapData)
      {
         hasRequestedMesh = true;
         EndlessTerrain.mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
      }
   }

   [System.Serializable]
   public struct LODInfo
   {
      public int lod;
      public float visibleDistanceThreshold;
   }
}
