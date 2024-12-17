using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public partial class Player : SingletonActionListener<Player>
{
    public interface I_AdditionalState
    {
        /// <summary>
        /// State��OnEnter
        /// </summary>
        public void OnEnter();

        /// <summary>
        /// State��OnUpdate
        /// </summary>
        public void OnUpdate();

        /// <summary>
        /// State��OnExit
        /// </summary>
        public void OnExit();
    }
}