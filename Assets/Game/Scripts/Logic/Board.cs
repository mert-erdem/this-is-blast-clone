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
        
        private TargetBlock[,] _grid = new TargetBlock[0, 0];

        private Vector3 _bottomLeftCornerPos;
        
        // External Classes
        private ObjectPool<TargetBlock, TargetBlockPool> _targetBlockPool;

        public void Initialize(TargetBlockData[] targetBlocks)
        {
            if (!CanInitialize(targetBlocks))
                return;

            _targetBlockPool ??= TargetBlockPool.Instance;

            ClearGrid();
            EnsureGrid(CalculateBoardHeight(targetBlocks.Length));

            _bottomLeftCornerPos = boardVisualMeshRenderer.bounds.min;
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
                targetBlock.transform.SetParent(parent, true);

                targetBlock.Initialize(targetBlockData);
                
                _grid[x, z] = targetBlock;
            }
        }

        private static int CalculateBoardHeight(int targetBlockCount)
        {
            return Mathf.CeilToInt((float)targetBlockCount / BOARD_WIDTH);
        }

        /// <summary>
        /// Checks if new allocation required or not
        /// </summary>
        /// <param name="boardHeight"></param>
        private void EnsureGrid(int boardHeight)
        {
            if (_grid.GetLength(0) == BOARD_WIDTH &&
                _grid.GetLength(1) == boardHeight)
            {
                return;
            }

            _grid = new TargetBlock[BOARD_WIDTH, boardHeight];
        }

        private Vector3 GetTargetBlockPosition(int x, int z)
        {
            return new Vector3(
                _bottomLeftCornerPos.x + (x + 0.5f) * CELL_SIZE,
                boardVisualMeshRenderer.bounds.max.y,
                _bottomLeftCornerPos.z + (z + 0.5f) * CELL_SIZE);
        }

        private void ClearGrid()
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int z = 0; z < _grid.GetLength(1); z++)
                {
                    TargetBlock targetBlock = _grid[x, z];

                    if (targetBlock == null)
                        continue;

                    _targetBlockPool.PullObjectBackImmediate(targetBlock);
                    _grid[x, z] = null;
                }
            }
        }
    }
}
