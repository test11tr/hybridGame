using System;
using System.Collections;
using UnityEngine;

public class DelayHelper : MonoBehaviour
{
    public static void DelayAction(float delay, Action action)
    {
        GameManager.Instance.delayHelper.StartCoroutine(DelayCoroutine(delay, action));
    }

    private static IEnumerator DelayCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}