using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlAvatar : MonoBehaviour
{
    [SerializeField]
    private AvatarController controller;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null)
        {
            return;
        }

        if (Input.GetKey("q"))
        {
            controller.SetMood(MoodState.NEUTRAL);
        }
        if (Input.GetKey("w"))
        {
            controller.SetMood(MoodState.HAPPY_LOW);
        }
        if (Input.GetKey("e"))
        {
            controller.SetMood(MoodState.HAPPY_HIGH);
        }
        if (Input.GetKey("r"))
        {
            controller.SetMood(MoodState.SAD_LOW);
        }
        if (Input.GetKey("t"))
        {
            controller.SetMood(MoodState.SAD_HIGH);
        }

        if (Input.GetKey("a"))
        {
            controller.expressEmotion(ExpressionState.NEUTRAL);
        }
        if (Input.GetKey("s"))
        {
            controller.expressEmotion(ExpressionState.HAPPY_LOW);
        }
        if (Input.GetKey("d"))
        {
            controller.expressEmotion(ExpressionState.HAPPY_HIGH);
        }
        if (Input.GetKey("f"))
        {
            controller.expressEmotion(ExpressionState.SAD_LOW);
        }
        if (Input.GetKey("g"))
        {
            controller.expressEmotion(ExpressionState.SAD_HIGH);
        }
        if (Input.GetKey("h"))
        {
            controller.expressEmotion(ExpressionState.ANGER_LOW);
        }
        if (Input.GetKey("j"))
        {
            controller.expressEmotion(ExpressionState.ANGER_HIGH);
        }
        if (Input.GetKey("k"))
        {
            controller.expressEmotion(ExpressionState.FEAR_LOW);
        }
        if (Input.GetKey("l"))
        {
            controller.expressEmotion(ExpressionState.FEAR_HIGH);
        }
        if (Input.GetKey("z"))
        {
            controller.expressEmotion(ExpressionState.DISGUST_LOW);
        }
        if (Input.GetKey("x"))
        {
            controller.expressEmotion(ExpressionState.DISGUST_HIGH);
        }
        if (Input.GetKey("c"))
        {
            controller.expressEmotion(ExpressionState.SURPRISE_LOW);
		}
		if (Input.GetKey("v"))
		{
			controller.expressEmotion(ExpressionState.SURPRISE_HIGH);
		}
		if (Input.GetKey("b"))
		{
			controller.expressEmotion(ExpressionState.HEAD_NOD);
		}
		if (Input.GetKey("n"))
		{
			controller.expressEmotion(ExpressionState.VISEMES);
		}
    }
}
