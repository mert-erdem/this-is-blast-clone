using Game.Scripts.Enums;
using UnityEngine;

namespace Game.Scripts.Common
{
    public static class Helper
    {
        public static Color ToUnityColor(BlockColor color)
        {
            return color switch
            {
                BlockColor.Red => Color.red,
                BlockColor.Blue => Color.blue,
                BlockColor.Green => Color.green,
                BlockColor.Yellow => Color.yellow,
                _ => Color.red
            };
        }
    }
}