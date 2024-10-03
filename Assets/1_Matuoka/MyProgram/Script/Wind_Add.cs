using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public partial class Player : SingletonActionListener<Player>
{
    [System.Serializable]
    public class Wind_Add : I_AdditionalState
    {
        #region Fields

        private Transform windAreaTransform;

        [SerializeField, Tooltip("風を受ける最大距離")]
        private float maxDistance = 10f;

        [SerializeField, Tooltip("最大パワー")]
        private float maxPower = 5f;

        [SerializeField, Tooltip("現在パワー"), ReadOnly]
        private float nowPower = 0f;

        //[SerializeField, Tooltip("加速度")] 
        private float acceleration = 1f;

        #endregion


        #region I_AdditionalState

        public virtual void OnEnter()
        {
            nowPower = 0f;
        }

        public virtual void OnUpdate()
        {
            MoveUpdate();

            //PlayerMoveUpdate();// 失敗作
        }

        public virtual void OnExit()
        {
            nowPower = 0f;
        }

        #endregion


        #region CustomMethod

        public void Init(Transform transform)
        {
            windAreaTransform = transform;
        }

        private void PlayerMoveUpdate()
        {
            nowPower += acceleration * Time.deltaTime;
            nowPower = Mathf.Min(nowPower, maxPower);
            Vector3 forward = windAreaTransform.forward;
            instance._controller.Move(nowPower * Time.deltaTime * forward);
        }

        private void MoveUpdate()
        {
            Vector3 forward = windAreaTransform.forward;

            float distance = Vector3.Distance(windAreaTransform.position, instance.transform.position);
            float rate = Mathf.InverseLerp(0f, maxDistance, distance);
            nowPower = Mathf.Lerp(0f, maxPower, 1f- rate);

            instance._controller.Move(nowPower * Time.deltaTime * forward);
        }


        #endregion
    }
}
