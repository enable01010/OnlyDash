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

        public virtual bool IsGuard()
        {
            if (wallArea == null) return true;
            return false;
        }
        public virtual void OnEnter()
        {
            Debug.Log("“ü‚Á‚½‚æ");
            Player instance = Player.instance;

            instance._animator.SetTrigger(instance._animIDClimbingStart);
        }
        public virtual void Climbing()
        {
            RotManagement();
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
    }
}
