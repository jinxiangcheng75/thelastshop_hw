using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopEventcomp : MonoBehaviour
{
    public System.Action _event;
    public float distance = 1;
    public int loop = -1;
    public float _time = 0;
    public int _loopcount = 0;
    // Update is called once per frame
    void Update()
    {
        if (_event != null)
        {
            _time += Time.deltaTime;
            if (_time >= distance)
            {
                _time = 0;
                _event?.Invoke();

                if (loop >= 0)
                {
                    _loopcount++;
                    if (_loopcount >= loop)
                    {
                        this.enabled = false;
                    }
                }

            }
        }
    }
}
