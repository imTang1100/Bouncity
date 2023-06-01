using System;
using UnityEngine;

public class WelcomePopupAnimation: MonoBehaviour
{
    public event Action OnAnimationEnd;

    [SerializeField]
    private Animator animator;

    private static readonly int AnimateHole = Animator.StringToHash("AnimateHole");

    private void AnimationEnd()
    {
        OnAnimationEnd?.Invoke();
    }

    public void StartAnimation()
    {
        animator.SetTrigger(AnimateHole);
    }
}
