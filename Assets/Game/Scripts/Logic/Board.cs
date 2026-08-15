using Game.Scripts.Core;
using Game.Scripts.Data;
using Game.Scripts.Entities;
using Game.Scripts.ObjectPools;
using UnityEngine;

namespace Game.Scripts.Logic
{
    public class Board : Singleton<Board>
    {
        [SerializeField] private TargetBlock targetBlockPrefab;
        [SerializeField] private Transform targetBlocksParent;
        [SerializeField] private MeshRenderer boardVisualMeshRenderer;

        private const int BOARD_WIDTH = 10;
        private const float CELL_SIZE = 0.9f;
        
        private TargetBlock[,] _grid;

        private Vector3 _bottomLeftCornerPos;
        
        // External Classes
        private ObjectPool<TargetBlock, TargetBlockPool> _targetBlockPool;

        public void Initialize(TargetBlockData[] targetBlocks)
        {
            if (!CanInitialize(targetBlocks))
                return;

            _targetBlockPool = TargetBlockPool.Instance;

            _bottomLeftCornerPos = boardVisualMeshRenderer.bounds.min;
            
            Clear();
            CreateTargetBlocks(targetBlocks);
        }

        private bool CanInitialize(TargetBlockData[] targetBlocks)
        {
            if (targetBlocks != null && targetBlocks.Length != 0) return true;
            
            Debug.LogError("Target block data is empty.");
            
            return false;
        }

        private void CreateTargetBlocks(TargetBlockData[] targetBlocks)
        {
            int boardHeight = CalculateBoardHeight(targetBlocks.Length);
            _grid = new TargetBlock[BOARD_WIDTH, boardHeight];

            for (int i = 0; i < targetBlocks.Length; i++)
            {
                TargetBlockData targetBlockData = targetBlocks[i];

                if (targetBlockData == null)
                    continue;

                int x = i % BOARD_WIDTH;
                int z = i / BOARD_WIDTH;

                Transform parent = targetBlocksParent != null ? targetBlocksParent : transform;
                
                TargetBlock targetBlock = _targetBlockPool.GetObject(true);
                targetBlock.transform.position = GetTargetBlockPosition(x, z);
                targetBlock.transform.rotation = Quaternion.identity;
                targetBlock.transform.SetParent(parent);

                targetBlock.Initialize(targetBlockData);
                
                _grid[x, z] = targetBlock;
            }
        }

        private static int CalculateBoardHeight(int targetBlockCount)
        {
            return Mathf.CeilToInt((float)targetBlockCount / BOARD_WIDTH);
        }

        private Vector3 GetTargetBlockPosition(int x, int z)
        {
            return new Vector3(
                _bottomLeftCornerPos.x + (x + 0.5f) * CELL_SIZE,
                boardVisualMeshRenderer.bounds.max.y,
                _bottomLeftCornerPos.z + (z + 0.5f) * CELL_SIZE);
        }

        private void Clear()
        {
            DestroyTargetBlocks();
        }

        // TODO: Object Pool
        private void DestroyTargetBlocks()
        {
            if (_grid == null)
                return;

            foreach (TargetBlock targetBlock in _grid)
            {
                if (targetBlock == null)
                    continue;

                if (Application.isPlaying)
                    Destroy(targetBlock.gameObject);
                else
                    DestroyImmediate(targetBlock.gameObject);
            }

            _grid = null;
        }
    }
}
