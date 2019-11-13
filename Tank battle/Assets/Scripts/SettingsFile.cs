using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyWork {
    
    [CreateAssetMenu(fileName ="Settings")]
    public class SettingsFile: ScriptableObject
    {
        /// <summary> 版本号</summary>
        [Tooltip("版本号")]
        public string Version = "0.0.0";

        /// <summary>/ 存档文件名 </summary>
        [Tooltip("存档文件名")]
        public string SaveName = "";

        
    }


}