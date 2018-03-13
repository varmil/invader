using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialManager : SingletonMonoBehaviour<MaterialManager>
{
    private static readonly int DefaultCapacity = 1000;

    // separate the dict because UI use UI/Default shader, and others use Standard shader
    // key is instance id
    public Dictionary<int, MeshRenderer> MeshRenderers { get; private set; }

    public Dictionary<int, Text> Texts { get; private set; }

    private Dictionary<int, Material> originalGameMaterials;
    private Dictionary<int, Material> originalUIMaterials;
    private Material redStandardMaterial;
    private Material redUIMaterial;

    void Awake()
    {
        MeshRenderers = new Dictionary<int, MeshRenderer>(DefaultCapacity);
        Texts = new Dictionary<int, Text>(DefaultCapacity);
        originalGameMaterials = new Dictionary<int, Material>(DefaultCapacity);
        originalUIMaterials = new Dictionary<int, Material>(DefaultCapacity);

        // create red material for Game objects
        redStandardMaterial = new Material(Shader.Find("Standard"));
        redStandardMaterial.color = Color.red;
        
        // create red material for UI text
        redUIMaterial = new Material(Shader.Find("UI/Default"));
        redUIMaterial.color = Color.red;
    }

    /// <summary>
    /// if the key already exists, overwrite it
    /// </summary>
    public void Add(MeshRenderer mesh)
    {
        MeshRenderers [mesh.gameObject.GetInstanceID()] = mesh;
    }

    public void Add(IEnumerable<MeshRenderer> meshes)
    {
        meshes.ToList().ForEach(m => this.Add(m));
    }

    /// <summary>
    /// if the key already exists, overwrite it
    /// </summary>
    public void Add(Text text)
    {
        Texts [text.gameObject.GetInstanceID()] = text;
    }

    public void Add(IEnumerable<Text> texts)
    {
        texts.ToList().ForEach(f => this.Add(f));
    }
    
    /// <summary>
    /// iterate the dictionaries and switch the material
    /// </summary>
    public void ChangeAllColorRed()
    {
        // switch Game object's material
        MeshRenderers.Values.ToList().ForEach(e => {
            originalGameMaterials [e.GetInstanceID()] = e.material;
            e.material = redStandardMaterial;
        });

        // switch UI material
        Texts.Values.ToList().ForEach(e => {
            originalUIMaterials [e.GetInstanceID()] = e.material;
            e.material = redUIMaterial;
        });
    }

    /// <summary>
    /// iterate the dictionaries and restore the original material
    /// </summary>
    public void RestoreAllColor()
    {
        MeshRenderers.Values.ToList().ForEach(e => {
            if (originalGameMaterials.ContainsKey(e.GetInstanceID()))
            {
                e.material = originalGameMaterials [e.GetInstanceID()];
            }
        });

        Texts.Values.ToList().ForEach(e => {
            if (originalUIMaterials.ContainsKey(e.GetInstanceID()))
            {
                e.material = originalUIMaterials [e.GetInstanceID()];
            }
        });
    }
}
