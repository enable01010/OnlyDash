using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceSerializedTest : MonoBehaviour
{
    [SerializeReference, SubclassSelector] I_Test test;


    
    public interface I_Test
    {

    }

    [System.Serializable]
    public class I_Test_1 : I_Test
    {
        [SerializeField] int i;
    }

    [System.Serializable]
    public class I_Test_2 : I_Test
    {
        [SerializeField] int b;
    }
}
