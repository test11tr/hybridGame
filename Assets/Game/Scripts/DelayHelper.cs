using System;
using System.Collections;
using UnityEngine;

public class DelayHelper : MonoBehaviour
{
    public static DelayHelper Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static void DelayAction(float delay, Action action)
    {
        Instance.StartCoroutine(DelayCoroutine(delay, action));
    }

    private static IEnumerator DelayCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}