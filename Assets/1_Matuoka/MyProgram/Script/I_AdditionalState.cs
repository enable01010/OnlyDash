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
        /// State‚ÌOnEnter
        /// </summary>
        public void OnEnter();

        /// <summary>
        /// State‚ÌOnUpdate
        /// </summary>
        public void OnUpdate();

        /// <summary>
        /// State‚ÌOnExit
        /// </summary>
        public void OnExit();
    }
}