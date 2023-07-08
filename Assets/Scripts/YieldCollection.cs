using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://forum.unity.com/threads/start-multiple-coroutines-and-wait-for-them-to-end.1099648/#post-7079587
public class YieldCollection : CustomYieldInstruction
{
    int _count;

    //Each time you call this, you call the coroutine and count is increased until the end.
    public IEnumerator CountCoroutine(IEnumerator coroutine)
    {
        _count++;
        yield return coroutine;
        _count--;
    }

    //If count is 0,no one coroutine is running.
    public override bool keepWaiting => _count != 0;
}
