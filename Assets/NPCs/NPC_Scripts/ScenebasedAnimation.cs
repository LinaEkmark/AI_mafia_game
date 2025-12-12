using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBasedAnimation : MonoBehaviour
{
    public Animator animator;

    [Header("Animation Settings")]
    public string sceneAnimation;


    void Awake()
    {
        if (!animator)
            animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Play correct animation for current scene
        animator.Play(sceneAnimation);
    }
}