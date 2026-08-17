using System;
using Game.Scripts.Enums;

namespace Game.Scripts.Data
{
    [Serializable]
    public class LevelSaveData
    {
        public int currentLevel;
        public int queueWidth;

        public TargetBlockSaveData[] targetBlocks;
        public CannonQueueSaveData[] cannonQueues;
        public CannonSlotSaveData[] cannonSlots;
    }
    
    [Serializable]
    public class TargetBlockSaveData
    {
        public int column;
        public int row;
        public BlockColor color;
        public int health;
    }

    [Serializable]
    public class CannonQueueSaveData
    {
        public int queueIndex;
        public int queueDepth;
        public BlockColor color;
        public int ammo;
    }

    [Serializable]
    public class CannonSlotSaveData
    {
        public int slotIndex;
        public BlockColor color;
        public int ammo;
    }
}