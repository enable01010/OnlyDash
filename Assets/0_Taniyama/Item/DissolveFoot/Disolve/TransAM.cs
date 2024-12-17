using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

/// <summary>
/// �g�����U���𐶐�����N���X
/// </summary>
[System.Serializable]
public class TransAM
{
    [SerializeField, Header("���ƈ����c��")] AfterImage lowPitch;
    [SerializeField, Header("����������c��")] AfterImage hiPitch;
    [SerializeField, Header("GN���q")] ParticleSystem _gnParticle;

    [SerializeField, Header("�{��")] SkinnedMeshRenderer[] _baseMeshObj;
    [SerializeField, Header("MATERIAL�̐ԐF���쓮������p�̃p�X")] string materialPass = "_isUse";
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
    /// MATERIAL�̐F��ω������鏈��
    /// </summary>
    /// <param name="isUse">0���ǉ��Ȃ��A1���ԉ��Z</param>
    private void SetMaterialColorRed(int isUse)
    {
        for (int i = 0; i < _baseMeshObj.Length; i++)
        {
            _baseMeshObj[i].materials[1].SetInt(materialPass, isUse);
        }

    }
}

/// <summary>
/// �c���𐶐�����N���X
/// </summary>
[System.Serializable]
public class AfterImage
{
    bool isUse = false;
    [SerializeField, Header("�{��")] SkinnedMeshRenderer[] _baseMeshObj;
    int meshLength;

    [SerializeField, Header("�e�����p�̃v���t�@�u")] GameObject _bakeMeshPref;
    List<SkinnedMeshRenderer[]> _meshList;
    List<float> lifeTimeList;
    int nextMeshCount = 0;

    [SerializeField, Header("�����Ԋu")] float createPitch = 0.1f;
    float nowCreatePitch = 0;
    [SerializeField, Header("������")] int amount = 3;

    [SerializeField, Header("MATERIAL�̐i�x��ݒ肷��p")] string MaterialPass = "_clipTimer";

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
    /// ���b�V���̕\����؂�ւ��鏈��
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
    /// �s�b�`�̃��X�g�����������鏈��
    /// </summary>
    private void PitchListClear()
    {
        for(int i = 0; i < amount; i++)
        {
            lifeTimeList[i] = 0; 
        }
    }

    /// <summary>
    /// �\�����Ă��郁�b�V���̃f�B�]�u����i�߂鏈��
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
    /// �V�������b�V������鏈��
    /// </summary>
    private void CreateNewMesh()
    {
        // ���̃X�L�����b�V����Bake����
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
