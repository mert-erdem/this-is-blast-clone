using System;

namespace Game.Scripts.Data
{
    [Serializable]
    public class LevelData
    {
        public int queueWidth; // Cannon queue width (changing in next levels)
    
        public TargetBlockData[] targetBlocks;
    
        public CannonData[] cannons;
    }
}