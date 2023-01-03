using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetRambleState : StateBase
{

    Pet _pet;

    public PetRambleState(Pet pet)
    {
        _pet = pet;
    }

    public override int getState()
    {
        return (int)MachinePetState.ramble;
    }

    int[] weights = { 10, 10, 10, 10 };
    string[] animStrs = { "call", "play", "rest", "rest_01", "rest_02", "rest_03" };

    public override void onEnter(StateInfo info)
    {
        Logger.log("!@#$%^&___宠物进入了 闲逛 的状态");

        int index = Helper.getRandomValuefromweights(weights);

        switch (index + 1)
        {
            case 1:
                _pet.moveEndCompleteHandler = () =>
                {
                    _pet.Character.Play("idle", true);
                    GameTimer.inst.AddTimer(2, 1, () =>
                    {
                        if (_pet != null && this != null)
                        {
                            if (_pet.GetCurState() != MachinePetState.ramble) return;

                            onEnter(null);
                        }
                    });
                };
                break;

            case 2:
                _pet.moveEndCompleteHandler = () =>
                {
                    _pet.Character.Play(animStrs[Random.Range(0, animStrs.Length)], completeDele: (t) =>
                    {
                        _pet.Character.Play("idle", true);

                        GameTimer.inst.AddTimer(2, 1, () =>
                        {
                            if (_pet != null && this != null)
                            {
                                if (_pet.GetCurState() != MachinePetState.ramble) return;

                                onEnter(null);
                            }
                        });
                    });
                };
                break;

            case 3:
                _pet.moveEndCompleteHandler = () =>
                {
                    _pet.Character.Play(animStrs[Random.Range(0, animStrs.Length)], completeDele: (t1) =>
                    {
                        _pet.Character.Play(animStrs[Random.Range(0, animStrs.Length)], completeDele: (t2) =>
                        {
                            _pet.Character.Play("idle", true);

                            GameTimer.inst.AddTimer(2, 1, () =>
                            {
                                if (_pet != null && this != null)
                                {
                                    if (_pet.GetCurState() != MachinePetState.ramble) return;

                                    onEnter(null);
                                }
                            });
                        });
                    });
                };

                break;

            case 4:

                _pet.moveEndCompleteHandler = () =>
                {
                    _pet.Character.Play(animStrs[Random.Range(0, animStrs.Length)], completeDele: (t1) =>
                    {
                        _pet.Character.Play(animStrs[Random.Range(0, animStrs.Length)], completeDele: (t2) =>
                        {
                            _pet.Character.Play(/*_pet.Character.Skeleton.Data.Animations.GetRandomElement().Name*/animStrs[Random.Range(0, animStrs.Length)], completeDele: (t3) =>
                            {
                                _pet.Character.Play("idle", true);

                                GameTimer.inst.AddTimer(2, 1, () =>
                                {
                                    if (_pet != null && this != null)
                                    {
                                        if (_pet.GetCurState() != MachinePetState.ramble) return;

                                        onEnter(null);
                                    }
                                });
                            });
                        });
                    });
                };
                break;

        }

        _pet.MoveToRandomPos();
    }


    public override void onUpdate()
    {

    }


    public override void onExit()
    {
        _pet.moveEndCompleteHandler = null;
    }

}
