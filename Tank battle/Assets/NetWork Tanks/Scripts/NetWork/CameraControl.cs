using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyWork.Network.Tanks
{
    public class CameraControl : MonoBehaviour
    {
        #region Public Fields

        public float DampTime = 0.2f;//相机移动缓冲时间
        public float ScreenEdgeBuffer = 4f;//场景边缘缓冲
        public float MinSize = 6.5f;
        [HideInInspector]
        public List<GameObject> Targets;
        
        #endregion

        #region Private Fields

        Camera _camera;
        float _zoomSpeed;
        Vector3 _moveVelocity;
        Vector3 _desiredPosition;//两个players，甚至是多个players的中央点位置，Camera设置在改点

        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            Targets = new List<GameObject>();
            _camera = GetComponentInChildren<Camera>();
        }

        private void FixedUpdate()
        {
            Debug.Log("CameraControl :" + Targets.Count);
            Move();
            Zoom();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// //设置初始相机位置与相机
        /// </summary>
        public void SetStartPositionAndSize()
        {
            FindAveragePosition();
            transform.position = _desiredPosition;
            _camera.orthographicSize = FindRequiredSize();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 移动
        /// </summary>
        void Move()
        {
            FindAveragePosition();
            transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _moveVelocity, DampTime);
        }

        /// <summary>
        /// 缩放
        /// </summary>
        void Zoom()
        {
            float requiredSize = FindRequiredSize();
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, requiredSize, ref _zoomSpeed, DampTime);
        }

        /// <summary>
        /// 计算多个目标对象的中间点位置
        /// </summary>
        void FindAveragePosition()
        {
            
            Vector3 averagePos = new Vector3();
            int numTargets = 0;
            for (int i = 0; i < Targets.Count; i++)
            {
                Debug.Log("Into FindAveragePosition");
                if (!Targets[i].gameObject.activeSelf)
                    continue;
                averagePos += Targets[i].transform.position;
                numTargets++;
            }
            if (numTargets == 0)//if all tanks destroy , return current position
                return;
            averagePos /= numTargets;
            averagePos.y = transform.position.y;
            _desiredPosition = averagePos;

        }

        /// <summary>
        /// 计算需要缩放的大小
        /// </summary>
        float FindRequiredSize()
        {
            Vector3 desiredLocalPos = transform.InverseTransformPoint(_desiredPosition);
            float size = 0f;
            for (int i = 0; i < Targets.Count; i++)
            {
                if (!Targets[i].gameObject.activeSelf)
                    continue;
                Vector3 targetLocalPos = transform.InverseTransformPoint(Targets[i].transform.position);
                Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / _camera.aspect);
            }
            size += ScreenEdgeBuffer;
            size = Mathf.Max(size, MinSize);
            return size;
        }

        #endregion
      
    }
}