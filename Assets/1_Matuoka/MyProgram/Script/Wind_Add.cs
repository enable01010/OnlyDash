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
        public virtual void OnEnter()
        {

        }

        public virtual void OnUpdate()
        { 

        }

        public virtual void OnExit()
        {

        }
    }
}
