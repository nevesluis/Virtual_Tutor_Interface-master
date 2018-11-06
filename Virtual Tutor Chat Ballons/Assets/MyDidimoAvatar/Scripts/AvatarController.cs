using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public partial class AvatarController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        if (!hasAnimator())
        {
            animator = GetComponent<Animator>();
        }
    }

    public bool hasAnimator()
    {
        return animator != null;
    }

    public bool hasFinnishedAnim()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0);
    }

    public void getAnimator()
    {
        animator = GetComponent<Animator>();
    }

    public void expressEmotion(ExpressionState expression)
    {
        animator.SetInteger("Expression", (int)expression);
        animator.SetTrigger("Express");
    }

    public void SetMood(MoodState moodState)
    {
        if(animator != null)
        {
            animator.SetInteger("Mood", (int)moodState);
        }
        
    }
}