using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCompTest1 : MonoBehaviour
{
    [SerializeField] int nuber;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInParent<GetCompTest>().Log();
    }

    public void Log()
    {
        Debug.Log(nuber);
    }
}
