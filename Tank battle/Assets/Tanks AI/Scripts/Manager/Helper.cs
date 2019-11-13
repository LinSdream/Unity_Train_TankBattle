using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TanksAI
{

    /// <summary>
    /// 辅助类
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// 通过角度来进行射线检测查找player
        /// </summary>
        /// <param name="controller">状态控制器</param>
        /// <param name="eulerAnger">角度</param>
        /// <param name="debugDrawColor">Debug颜色</param>
        public static bool CheckRayByEulerToFindPlayer(StateController controller,Quaternion eulerAnger,Color debugDrawColor)
        {
            Debug.DrawRay(controller.Eyes.position, 
                eulerAnger * controller.Eyes.forward * controller.EnemyTankInfo.LookRange, debugDrawColor);

            RaycastHit hit;
            if(Physics.Raycast(controller.Eyes.position,eulerAnger*controller.Eyes.forward,out hit,controller.EnemyTankInfo.LookRange
                ) && hit.collider.CompareTag("Player"))
            {
                controller.targetObj = hit.transform;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 返回两点之间距离
        /// </summary>
        /// <param name="origin">原点</param>
        /// <param name="target">目标点</param>
        public static float CalculationDistance(Transform origin,Transform target)
        {
            return (target.position - origin.position).magnitude;
        }

        /// <summary>
        /// 获取navmesh上随机的一个点
        /// </summary>
        public static Vector3 GetRandomLocaion(StateController controller)
        {
            NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();//获取所有navmesh上的顶点

            //每三个相邻顶点构成一个三角网格，数组减去3防止越界
            int trianglePoint = Random.Range(0, navMeshData.indices.Length - 3);

            //取任意一个三角网格的三点的中点
            Vector3 point = (navMeshData.vertices[navMeshData.indices[trianglePoint]] +
                navMeshData.vertices[navMeshData.indices[trianglePoint + 1]]
                + navMeshData.vertices[navMeshData.indices[trianglePoint + 2]]) / 3;

            #region how to get a point in triangle

            //int target = Random.Range(0, navMeshData.indices.Length - 3);

            //Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[target]],
            //    navMeshData.vertices[navMeshData.indices[target + 1]],Random.value);

            //point = Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[target + 2]], Random.value);
            //Debug.Log(point);
            #endregion
            return point;
        }

    }
    
}