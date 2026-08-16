using System;
using Game.Scripts.Core;
using Game.Scripts.Data;
using Game.Scripts.Entities;
using Game.Scripts.Enums;
using Game.Scripts.ObjectPools;
using UnityEngine;

namespace Game.Scripts.Logic
{
    public class Board : Singleton<Board>
    {
        [SerializeField] private Transform targetBlocksParent;
        [SerializeField] private MeshRenderer boardVisualMeshRenderer;
        
        public event Action OnBoardStateChanged;

        private const int BOARD_WIDTH = 10;
        private const float CELL_SIZE = 0.9f;
        
        private TargetBlock[,] _grid = new TargetBlock[0, 0];

        private Vector3 _bottomLeftCornerPos;
        
        // TWEEN PROPERTIES
        private const float TWEEN_SHIFT_DURATION = 0.5f;
        private const float TWEEN_DESTROY_DURATION = 0.2f;
        
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

        public bool TryGetTarget(BlockColor color, out TargetBlock target)
        {
            target = null;

            int width = _grid.GetLength(0);
            int height = _grid.GetLength(1);

            for (int i = 0; i < width; i++)
            {
                TargetBlock frontBlock = null;

                for (int j = 0; j < height; j++)
                {
                    if (_grid[i, j] == null)
                        continue;

                    frontBlock = _grid[i, j];
                    
                    break;
                }

                if (frontBlock == null)
                    continue;

                if (!frontBlock.IsFireable)
                    continue;

                if (frontBlock.GetColor() != color)
                    continue;

                target = frontBlock;
                
                return true;
            }

            return false;
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

                int columnIndex = i % BOARD_WIDTH;
                int rowIndex = i / BOARD_WIDTH;

                Transform parent = targetBlocksParent != null ? targetBlocksParent : transform;
                
                TargetBlock targetBlock = _targetBlockPool.GetObject(true);
                targetBlock.transform.position = GetTargetBlockPosition(columnIndex, rowIndex);
                targetBlock.transform.rotation = Quaternion.identity;
                targetBlock.transform.SetParent(parent, true);

                targetBlock.Initialize(targetBlockData, new Vector2Int(columnIndex, rowIndex));
                targetBlock.OnDestroyed += RemoveTargetBlock;
                
                _grid[columnIndex, rowIndex] = targetBlock;
            }
        }
        
        private void RemoveTargetBlock(TargetBlock targetBlock)
        {
            if (targetBlock == null)
                return;

            Vector2Int gridPosition = targetBlock.GetGridPosition();

            int i = gridPosition.x;
            int j = gridPosition.y;

            if (_grid[i, j] != targetBlock)
                return;

            targetBlock.OnDestroyed -= RemoveTargetBlock;

            targetBlock.PlayDestroyTween(TWEEN_DESTROY_DURATION, () =>
            {
                if (_grid[i, j] != targetBlock)
                    return;

                _grid[i, j] = null;
                _targetBlockPool.PullObjectBackImmediate(targetBlock);
                ShiftColumnForward(i, j);
            });
        }

        private void ShiftColumnForward(int i, int emptyJ)
        {
            int height = _grid.GetLength(1);

            for (int j = emptyJ + 1; j < height; j++)
            {
                TargetBlock targetBlock = _grid[i, j];

                _grid[i, j - 1] = targetBlock;
                _grid[i, j] = null;

                if (targetBlock == null)
                    continue;

                Vector2Int newGridPosition = new Vector2Int(i, j - 1);
                targetBlock.SetGridPosition(newGridPosition);
                targetBlock.MoveTo(
                    GetTargetBlockPosition(newGridPosition.x, newGridPosition.y),
                    TWEEN_SHIFT_DURATION,
                    () => OnBoardStateChanged?.Invoke());
            }

            OnBoardStateChanged?.Invoke();
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

        private Vector3 GetTargetBlockPosition(int i, int j)
        {
            return new Vector3(
                _bottomLeftCornerPos.x + (i + 0.5f) * CELL_SIZE,
                boardVisualMeshRenderer.bounds.max.y,
                _bottomLeftCornerPos.z + (j + 0.5f) * CELL_SIZE);
        }

        private void ClearGrid()
        {
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    TargetBlock targetBlock = _grid[i, j];

                    if (targetBlock == null)
                        continue;

                    targetBlock.OnDestroyed -= RemoveTargetBlock;
                    _targetBlockPool.PullObjectBackImmediate(targetBlock);
                    _grid[i, j] = null;
                }
            }
        }
    }
}
