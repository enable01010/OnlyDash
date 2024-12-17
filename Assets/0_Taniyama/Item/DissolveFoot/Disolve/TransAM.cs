using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

/// <summary>
/// トランザムを生成するクラス
/// </summary>
[System.Serializable]
public class TransAM
{
    [SerializeField, Header("あと引く残像")] AfterImage lowPitch;
    [SerializeField, Header("すぐ消える残像")] AfterImage hiPitch;
    [SerializeField, Header("GN粒子")] ParticleSystem _gnParticle;

    [SerializeField, Header("本体")] SkinnedMeshRenderer[] _baseMeshObj;
    [SerializeField, Header("MATERIALの赤色を作動させる用のパス")] string materialPass = "_isUse";
    public void Init()
    {
        lowPitch.Init();
        hiPitch.Init();
    }

    public void OnEnter()
    {
        lowPitch.OnEnter();
        hiPitch.OnEnter();
        _gnParticle.Play();
        SetMaterialColorRed(1);
    }

    public void OnUpdate()
    {
        lowPitch.OnUpdate();
        hiPitch.OnUpdate();
    }

    public void OnFixedUpdate()
    {
        lowPitch.OnFixedUpdate();
        hiPitch.OnFixedUpdate();
    }

    public void OnExit()
    {
        lowPitch.OnExit();
        hiPitch.OnExit();
        _gnParticle.Pause();
        SetMaterialColorRed(0);
    }

    /// <summary>
    /// MATERIALの色を変化させる処理
    /// </summary>
    /// <param name="isUse">0→追加なし、1→赤加算</param>
    private void SetMaterialColorRed(int isUse)
    {
        for (int i = 0; i < _baseMeshObj.Length; i++)
        {
            _baseMeshObj[i].materials[1].SetInt(materialPass, isUse);
        }

    }
}

/// <summary>
/// 残像を生成するクラス
/// </summary>
[System.Serializable]
public class AfterImage
{
    bool isUse = false;
    [SerializeField, Header("本体")] SkinnedMeshRenderer[] _baseMeshObj;
    int meshLength;

    [SerializeField, Header("影生成用のプレファブ")] GameObject _bakeMeshPref;
    List<SkinnedMeshRenderer[]> _meshList;
    List<float> lifeTimeList;
    int nextMeshCount = 0;

    [SerializeField, Header("生成間隔")] float createPitch = 0.1f;
    float nowCreatePitch = 0;
    [SerializeField, Header("生成数")] int amount = 3;

    [SerializeField, Header("MATERIALの進度を設定する用")] string MaterialPass = "_clipTimer";

    public void Init()
    {
        meshLength = _baseMeshObj.Length;

        int i, j;
        _meshList = new List<SkinnedMeshRenderer[]>();
        lifeTimeList = new List<float>();
        for (i = 0; i < amount; i++)
        {
            SkinnedMeshRenderer[] meshes = new SkinnedMeshRenderer[meshLength];
            for(j = 0; j < meshLength; j++)
            {
                GameObject ins = GameObject.Instantiate(_bakeMeshPref);
                meshes[j] = ins.GetComponent<SkinnedMeshRenderer>();
                meshes[j].enabled = false;
            }
            _meshList.Add(meshes);
            lifeTimeList.Add(0);
        }
        
    }

    public void OnEnter()
    {
        isUse = true;
        MeshSetEnabled(true);
    }

    public void OnUpdate()
    {
        if (!isUse) return;

        nowCreatePitch += Time.deltaTime;

        MeshUpdate();
    }

    public void OnFixedUpdate()
    {
        if (!isUse || nowCreatePitch < createPitch) return;
        nowCreatePitch = 0;
        CreateNewMesh();
    }

    public void OnExit()
    {
        isUse = false;
        nextMeshCount = 0;
        MeshSetEnabled(false);
        PitchListClear();
    }

    /// <summary>
    /// メッシュの表示を切り替える処理
    /// </summary>
    /// <param name="isActive"></param>
    private void MeshSetEnabled(bool isActive)
    {
        int i, j;
        for (i = 0; i < amount; i++)
        {
            for (j = 0; j < _baseMeshObj.Length; j++)
            {
                _meshList[i][j].enabled = isActive;
            }
        }
    }

    /// <summary>
    /// ピッチのリストを初期化する処理
    /// </summary>
    private void PitchListClear()
    {
        for(int i = 0; i < amount; i++)
        {
            lifeTimeList[i] = 0; 
        }
    }

    /// <summary>
    /// 表示しているメッシュのディゾブルを進める処理
    /// </summary>
    private void MeshUpdate()
    {
        int i, j;
        for (i = 0; i < amount; i++)
        {
            lifeTimeList[i] += Time.deltaTime;
            float rat = lifeTimeList[i] / (amount * createPitch);
            for (j = 0; j < meshLength; j++)
            {
                _meshList[i][j].material.SetFloat(MaterialPass, rat);
            }
        }
    }

    /// <summary>
    /// 新しいメッシュを作る処理
    /// </summary>
    private void CreateNewMesh()
    {
        // 今のスキンメッシュをBakeする
        Mesh[] mesh = new Mesh[meshLength];
        for (int i = 0; i < meshLength; i++)
        {
            mesh[i] = new Mesh();
            _baseMeshObj[i].BakeMesh(mesh[i]);
            _meshList[nextMeshCount][i].sharedMesh = mesh[i];
            _meshList[nextMeshCount][i].transform.position = _baseMeshObj[i].transform.position;
            _meshList[nextMeshCount][i].transform.rotation = _baseMeshObj[i].transform.rotation;
        }
        lifeTimeList[nextMeshCount] = 0;
        nextMeshCount = (nextMeshCount + 1) % amount;
        Debug.Log(nextMeshCount);
    }
}
