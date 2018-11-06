using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstScreen : MonoBehaviour {

    private float timeMultiplier = 0.3f;

    // Use this for initialization
    void Start () {
        StartCoroutine(FadeInText(timeMultiplier, dollg));
    }
	
	// Update is called once per frame
	void Update () {

    }

    /*Avatars for greeting*/
    [SerializeField] private Image dollg;

    /// <summary>
    /// Fades the in text.
    /// </summary>
    /// <param name="timeSpeed">The time speed.</param>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    private IEnumerator FadeInText(float timeSpeed, Image text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime * timeSpeed));
            yield return null;
        }
    }
    /// <summary>
    /// Fades the out text.
    /// </summary>
    /// <param name="timeSpeed">The time speed.</param>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    private IEnumerator FadeOutText(float timeSpeed, Image text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime * timeSpeed));
            yield return null;
        }
    }
    /// <summary>
    /// Fades the in text.
    /// </summary>
    /// <param name="timeSpeed">The time speed.</param>
    public void FadeInText(float timeSpeed = -1.0f)
    {
        if (timeSpeed <= 0.0f)
        {
            timeSpeed = timeMultiplier;
        }
        StartCoroutine(FadeInText(timeSpeed, dollg));
    }
    /// <summary>
    /// Fades the out text.
    /// </summary>
    /// <param name="timeSpeed">The time speed.</param>
    public void FadeOutText(float timeSpeed = -1.0f)
    {
        if (timeSpeed <= 0.0f)
        {
            timeSpeed = timeMultiplier;
        }
        StartCoroutine(FadeOutText(timeSpeed, dollg));
    }
}
