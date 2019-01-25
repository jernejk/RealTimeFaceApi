using System;
using System.Collections.Generic;
using System.Text;

namespace RealTimeFaceApi.Cmd.Pose.PoseNet
{
    public struct Vector2Int
    {
        public int x;
        public int y;
        
        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
