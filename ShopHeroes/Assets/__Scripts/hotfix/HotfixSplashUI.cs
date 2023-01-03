using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotfixSplashUI : MonoBehaviour
{
    public Image img_splash;
    public float showPeriod;
    // Start is called before the first frame update
    void Start()
    {
        img_splash.color = Color.black;
        StartCoroutine(procedureShow());
    }

    IEnumerator procedureShow () {
        yield return null;
        Color c = Color.black;
        float period = 0.05f;
        Color p = new Color(period, period, period);
        while(c.r < 1) {
            yield return new WaitForSeconds(period);
            c += p;
            img_splash.color = c;
        }
    }
    
}
