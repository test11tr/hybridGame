using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class floatingText : MonoBehaviour
{
    public TMP_Text textContent;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetText(string text, Color color, float fontSize)
    {
        textContent.text = text;
        textContent.color = color;
        textContent.fontSize = fontSize;
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
