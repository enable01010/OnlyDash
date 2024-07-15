using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : SingletonActionListener<Player>
{
    public interface I_Climbing
    {
        public bool isGuard();
        public void OnEnter();
        public void Climbing();
        public void OnExit();
    }

    [System.Serializable]
    public class DefaultClimbing:I_Climbing
    {
        public virtual bool isGuard()
        {
            return false;
        }
        public virtual void OnEnter()
        {

        }
        public virtual void Climbing()
        {

        }
        public virtual void OnExit()
        {

        }
    }
}
