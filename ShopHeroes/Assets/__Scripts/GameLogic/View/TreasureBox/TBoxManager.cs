using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBoxManager : SingletonMono<TBoxManager>
{
    public List<GameObject> boxes;
    private GameObject curBox;
    private Animator curAnim;

    public SkeletonAnimation sceneAnim;
    public Animator testAnim;
    public GameObject openBoxEffect;
    int timerId = 0;
    public void setBox(int index, System.Action completeHandler = null)
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
        if (curBox != null)
        {
            curBox.SetActive(false);
        }
        curBox = boxes[index];
        curAnim = curBox.GetComponent<Animator>();
        sceneAnim.state.SetAnimation(0, "fly", false);
        sceneAnim.state.Complete += (trackEntry) =>
        {
            sceneAnim.state.SetAnimation(0, "idle", true);
        };
        GameTimer.inst.AddTimer(0.3f, 1, () =>
          {
              curBox.SetActive(true);
              curAnim.Play("jump");
              AudioManager.inst.PlaySound(102);
              float animTime = curAnim.GetClipLength("jump");
              timerId = GameTimer.inst.AddTimer(animTime, 1, () =>
                {
                    completeHandler?.Invoke();
                });
          });
    }

    public void setCurFalse()
    {
        if (curBox != null)
        {
            curBox.SetActive(false);
            curAnim = null;
        }
    }

    public void playOpenAnim(System.Action animCompHandler = null, System.Action moveCameraHandler = null)
    {
        if (curBox != null && curAnim != null)
        {
            curAnim.Play("open");
            AudioManager.inst.PlaySound(101);
            float animTimer = curAnim.GetClipLength("open");

            GameTimer.inst.AddTimer(animTimer * 0.7f, 1, () =>
              {
                  moveCameraHandler?.Invoke();
              });

            GameTimer.inst.AddTimer(animTimer, 1, () =>
              {
                  openBoxEffect.SetActive(true);
                  animCompHandler?.Invoke();
              });
        }
    }

    public void playAnim(string animName, System.Action animCompHandler = null)
    {
        if (curBox != null && curAnim != null)
        {
            curAnim.Play(animName,0,0f);
            float animTimer = curAnim.GetClipLength(animName);

            GameTimer.inst.AddTimer(animTimer, 1, () =>
            {
                animCompHandler?.Invoke();
            });
        }
    }

    public void CloseEffect()
    {
        openBoxEffect.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            sceneAnim.state.SetAnimation(0, "fly", false);
            //GameTimer.inst.AddTimer(0.3f, 1, () =>
            //  {
            //      testAnim.gameObject.SetActive(true);
            //      testAnim.Play("jump");
            //      float animTime = testAnim.GetClipLength("jump");
            //      GameTimer.inst.AddTimer(animTime, 1, () =>
            //      {
            //          testAnim.gameObject.SetActive(false);
            //          testAnim.Play("idle");
            //      });
            //  });
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            sceneAnim.state.SetAnimation(0, "idle", false);
        }
    }
}
