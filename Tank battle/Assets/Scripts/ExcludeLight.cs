using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyWork
{
    public class ExcludeLight : MonoBehaviour
    {
        public Shader unlitShader;

        void Start()
        {
            unlitShader = Shader.Find("Unlit/Texture");
            GetComponent<Camera>().SetReplacementShader(unlitShader, "");
        }
    }
}