using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowVinesScript : MonoBehaviour
{
    public List<MeshRenderer> growVinesMeshes;    
    [Space]
    public float timeToGrow = 5;
    public float refreshRate = 0.05f;
    [Space]
    [Range(0,1)]
    public float minGrow = 0.2f;
    [Range(0,1)]
    public float maxGrow = 0.97f;
    [Space]
    public bool adjustScale = false;
    [Range(-5,0)]
    public float minScale = -2.5f;
    [Range(-5,0)]
    public float maxScale = -3f;

    private List<Material> growVinesMaterials = new List<Material>();
    private bool fullyGrown;
    
    void Start()
    {
        for(int i=0; i<growVinesMeshes.Count; i++)
        {
            for(int j=0; j<growVinesMeshes[i].materials.Length; j++)
            {
                if(growVinesMeshes[i].materials[j].HasProperty("Grow_"))
                {            
                    growVinesMeshes[i].materials[j].SetFloat("Grow_", minGrow);
                    if(adjustScale)
                        growVinesMeshes[i].materials[j].SetFloat("Scale_", maxScale);
                    growVinesMaterials.Add(growVinesMeshes[i].materials[j]);
                }
            }
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            for(int i=0; i<growVinesMaterials.Count; i++)
            {                
                StartCoroutine (GrowVines(growVinesMaterials[i]));                
            }
        }
    }

    IEnumerator GrowVines (Material mat)
    {
        float growValue = mat.GetFloat("Grow_");
        float scaleValue = mat.GetFloat("Scale_");

        if(!fullyGrown)
        {
            while(growValue < maxGrow)
            {
                growValue += 1/(timeToGrow / refreshRate);
                mat.SetFloat("Grow_", growValue);
                
                if(adjustScale)
                {
                    scaleValue += (Mathf.Abs(maxScale)-Mathf.Abs(minScale))/(timeToGrow / refreshRate);
                    mat.SetFloat("Scale_", scaleValue);
                }

                yield return new WaitForSeconds (refreshRate);
            }
        }
        else
        {
            while(growValue > minGrow)
            {
                growValue -= 1/(timeToGrow / refreshRate);
                mat.SetFloat("Grow_", growValue);
                
                if(adjustScale)
                {
                    scaleValue -= (Mathf.Abs(maxScale)-Mathf.Abs(minScale))/(timeToGrow / refreshRate);
                    mat.SetFloat("Scale_", scaleValue);
                }

                yield return new WaitForSeconds (refreshRate);
            }
        }


        if(growValue >= maxGrow)
            fullyGrown = true;
        else
            fullyGrown = false;
    }
}
