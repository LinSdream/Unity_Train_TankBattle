using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyWork
{
    [CreateAssetMenu(fileName ="Level Info")]
    public class Level :ScriptableObject
    {
        public string LevelName;
        public LevelData data;
    }
}
