using System;
using Game.Scripts.Data;

[Serializable]
public class LevelData
{
    public int boardWidth;
    
    public int boardHeight;

    public int queueWidth; // Cannon queue width (changing in next levels)
    
    public TargetBlockData[] blocks;
    
    public CannonData[] cannons;
}