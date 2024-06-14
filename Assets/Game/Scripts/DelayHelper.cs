using System;
using System.Collections;
using UnityEngine;

public class DelayHelper : MonoBehaviour
{
    private static DelayHelper _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static void DelayAction(float delay, Action action)
    {
        _instance.StartCoroutine(DelayCoroutine(delay, action));
    }

    private static IEnumerator DelayCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}