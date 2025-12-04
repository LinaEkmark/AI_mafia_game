using KinematicCharacterController;
using UnityEngine;


public class AnimationStateController : MonoBehaviour
{
    public Animator animator;
    public KinematicCharacterMotor motor;

    int isWalkingHash;

    void Awake()
    {
        if (!animator)
            animator = GetComponent<Animator>();

        if (!motor)
            motor = GetComponentInParent<KinematicCharacterMotor>();

        isWalkingHash = Animator.StringToHash("isWalking");
    }

    void Update()
    {
        if (motor == null || animator == null)
            return;

        // Get horizontal velocity (ignore vertical, so jumping doesn't count)
        Vector3 vel = motor.BaseVelocity;
        vel.y = 0f;

        bool isWalking = vel.magnitude > 0.1f; // tweak threshold if needed

        animator.SetBool(isWalkingHash, isWalking);
    }
}
