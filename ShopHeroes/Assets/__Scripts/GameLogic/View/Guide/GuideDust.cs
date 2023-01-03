using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideDust : MonoBehaviour
{
    public ParticleSystem vfx1;
    public ParticleSystem vfx2;
    public ParticleSystem vfx3;

    public void PlayDust()
    {
        vfx1.Simulate(0.0f);
        vfx2.Simulate(0.0f);
        vfx3.Simulate(0.0f);
        vfx1.Play();
        vfx2.Play();
        vfx3.Play();
    }

    public void setDustPos(int curFurnUid)
    {
        if (curFurnUid == 70001)
        {
            vfx1.transform.localPosition = new Vector3(1.2f, 0.01f, 0);
            vfx2.transform.localPosition = new Vector3(1.4f, 0.12f, 0);
            vfx3.transform.localPosition = new Vector3(1.1f, 0.03f, 0);
        }
        else if (curFurnUid == 90001)
        {
            vfx1.transform.localPosition = new Vector3(0.9f, -0.02f, 0);
            vfx2.transform.localPosition = new Vector3(1.3f, -0.05f, 0);
            vfx3.transform.localPosition = new Vector3(1f, 0.05f, 0);
        }
    }
}
