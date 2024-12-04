using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{
    public float despawnTime;
    public float dissolveTime;

    public Rigidbody torso;
    public Material mat;
    public SkinnedMeshRenderer[] meshes;

    Material dissolve;
    float dissolveAmount = 0.19f;
    public bool wasHeadshot = false;

    void Start()
    {
        foreach (var m in meshes)
        {
            m.material = new Material(mat);
        }

        if (wasHeadshot) SoundManager2.Instance.PlaySound("RobotWeakPointHit");
        else SoundManager2.Instance.PlaySound("RobotHit");

        SoundManager2.Instance.PlaySound("RobotDeath", transform);
        SoundManager2.Instance.PlaySound("RobotDesolving", transform);

        Destroy(gameObject, despawnTime);
    }

    private void Update()
    {
        if (dissolveTime <= 0)
        {
            dissolveAmount -= 0.1f * Time.deltaTime;
            foreach (var m in meshes)
            {
                m.material.SetFloat("_Dissolve_Amount", dissolveAmount);
            }
        }
        else dissolveTime -= Time.deltaTime;
    }

    public void ApplyForce(Vector3 hitNormal, float hitStrength)
    {
        torso.AddForce(hitNormal * hitStrength, ForceMode.Impulse);
    }
}
