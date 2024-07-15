using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : SingletonActionListener<Player>
{
    public interface I_Climbing
    {
        public bool IsGuard();
        public void OnEnter();
        public void Climbing();
        public void OnExit();
        public void SetArea(WallArea wallArea);
    }

    [System.Serializable]
    public class DefaultClimbing:I_Climbing
    {
        private WallArea wallArea;
        private bool isClimbing = false;
        [SerializeField] float START_MOVE_SPEED = 5.0f;
        [SerializeField] Vector3 POSISION_OFFSET = new Vector3(0, 0.15f, 0.15f);
        [SerializeField] float MAX_DISTANCE = 0.1f;

        public virtual bool IsGuard()
        {
            if (wallArea == null) return true;
            return false;
        }
        public virtual void OnEnter()
        {
            Player instance = Player.instance;

            instance._animator.SetTrigger(instance._animIDClimbingStart);
        }
        public virtual void Climbing()
        {
            RotManagement();
            MoveManagement();
        }
        public virtual void OnExit()
        {

        }

        public virtual void SetArea(WallArea wallArea)
        {
            this.wallArea = wallArea;
        }


        private void RotManagement()
        {
            Player instance = Player.instance;
            float rotation = Mathf.SmoothDampAngle(instance.transform.eulerAngles.y, wallArea.rot.y, ref instance._rotationVelocity, instance.ROTATION_SMOOTH_TIME);
            instance.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        private void MoveManagement()
        {
            Player instance = Player.instance;
            Vector3 moveDir = wallArea.pos.position - instance.transform.position - POSISION_OFFSET;
            if (MAX_DISTANCE > moveDir.magnitude) return;
            moveDir = moveDir.normalized * START_MOVE_SPEED * Time.deltaTime;
            instance._controller.Move(moveDir);
        }
    }
}
