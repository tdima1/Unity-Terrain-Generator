using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
   public LODInfo[] detailLevels;
   public static float maxViewDistance;

   public Transform viewer;
   public Material mapMaterial;

   public static Vector2 viewerPosition;
   public static MapGenerator mapGenerator;
   int chunkSize;
   int chunksVisibleInViewDistance;

   Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
   List<TerrainChunk> terrainChunksVisibleLatUpdate = new List<TerrainChunk>();

   void Start()
   {
      mapGenerator = FindObjectOfType<MapGenerator>();
      maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
      chunkSize = MapGenerator.mapChunkSize - 1;
      chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
   }

   void Update()
   {
      viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
      UpdateVisibleChunks();
   }

   void UpdateVisibleChunks()
   {
      for (int i = 0; i < terrainChunksVisibleLatUpdate.Count; i++) {
         terrainChunksVisibleLatUpdate[i].SetVisible(false);
      }
      terrainChunksVisibleLatUpdate.Clear();

      //Gets the central chunk
      int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
      int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

      //Goes through all visible chunks in range
      for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++) {
         for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++) {

            //Neighboring chunks:
            Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

            if (terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
               //Update visibility of chunk depending on distance of viewer from chunk nearest edge.
               terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk(viewerPosition, maxViewDistance);
               //If chunk remained visible, add it to VisibleLastUpdate list.
               if (terrainChunkDictionary[viewedChunkCoord].IsVisible()) {
                  terrainChunksVisibleLatUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
               }
            } else {
               //If invisible, keep it in the total chunk list.
               terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
            }
         }
      }
   }
}
