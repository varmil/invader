using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleManager : SingletonMonoBehaviour<ParticleManager>
{
    private List<ParticlePooler> particlePoolerList = new List<ParticlePooler>();

    /// <summary>
    /// 指定した名前のパーティクル再生
    /// 初めて再生するパーティクルはプール用オブジェクトを生成
    /// </summary>
    public void Play(string particleName, Vector3 position, Material material = null)
    {
        //リストから指定した名前のプール用オブジェクトを取得
        ParticlePooler pooler = particlePoolerList.Where(tempPooler => tempPooler.ParticleName == particleName).FirstOrDefault();
        if (pooler == null)
        {
            //取得できなければ新たに生成
            pooler = new ParticlePooler(particleName, this.transform);
            particlePoolerList.Add(pooler);
        }
        pooler.Play(position, material);
    }
}

/// <summary>
///  パーティクルのプーリング用クラス
/// </summary>
public class ParticlePooler
{
    public ParticlePooler(string particleName, Transform parent)
    {
        this.particleName = particleName;
        this.parent = parent;
    }

    /// <summary>
    /// パーティクル名
    /// </summary>
    private string particleName;
    public string ParticleName
    {
        get { return particleName; }
    }

    /// <summary>
    /// パーティクルを保持しておくリスト
    /// </summary>
    private List<ParticleSystem> particleList = new List<ParticleSystem>();
    public List<ParticleSystem> ParticleList
    {
        get { return particleList; }
    }

    /// <summary>
    /// 生成元のパーティクル
    /// </summary>
    private GameObject particleOrigin = null;

    private Transform parent;

    /// <summary>
    /// 指定の座標で再生
    /// </summary>
    /// <param name="position">Position.</param>
    public void Play(Vector3 position, Material material = null)
    {
        ParticleSystem particle = GetPlayableParticle();
        if (particle == null)
        {
            particle = InstantiateParticle();
        }
        particle.transform.position = position;
        particle.transform.parent = parent;

        if (material != null)
        {
            particle.GetComponent<ParticleSystemRenderer>().material = material;
        }

        particle.Play();
    }

    /// <summary>
    /// 再生可能なパーティクルを取得
    /// </summary>
    /// <returns>再生可能なパーティクル.</returns>
    private ParticleSystem GetPlayableParticle()
    {
        return particleList.Where(particle => !particle.isPlaying).FirstOrDefault();
    }

    /// <summary>
    /// パーティクル生成
    /// </summary>
    /// <returns>The particle.</returns>
    private ParticleSystem InstantiateParticle()
    {
        LoadOrigin();
        GameObject particleGO = GameObject.Instantiate(particleOrigin) as GameObject;
        ParticleSystem particle = particleGO.GetComponent<ParticleSystem>();
        particleList.Add(particle);
        return particle;
    }

    /// <summary>
    /// 生成元のオブジェクトをロード
    /// </summary>
    private void LoadOrigin()
    {
        if (particleOrigin == null)
        {
            particleOrigin = Resources.Load(particleName) as GameObject;
            particleOrigin.name = particleName;
        }
    }

    /// <summary>
    /// 破棄時処理
    /// </summary>
    private void Clean()
    {
        particleList.Clear();
        particleList = null;
        particleOrigin = null;
    }
}