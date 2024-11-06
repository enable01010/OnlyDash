using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isOn = false;
    private Material material;

    private void Awake()
    {
        material = this.GetComponent<MeshRenderer>().material;
    }

    public void ChangeIsOn()
    {
        isOn = !isOn;
    }

    public void ChangeMaterial(Color color)
    {
        material.color = color;
    }
}


/// <summary>
/// タイルを押せるオブジェクト用のインターフェース
/// </summary>
public interface I_TileHit
{

}