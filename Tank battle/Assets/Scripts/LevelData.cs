using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MyWork
{
    public class LevelData
    {
        /// <summary>关卡名 </summary>
        public string LevelName = string.Empty;
        /// <summary> 关卡状态 0 未开启，1开启 </summary>
        public int Status = 0;
        /// <summary> 分数 </summary>
        public float Score = 0;
    }
}
