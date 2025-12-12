using KinematicCharacterController;
using UnityEngine;


public class AnimationStateController : MonoBehaviour
{
    int isWalkingHash;

    [Header("Components")]
    public Animator animator;
    public KinematicCharacterMotor motor;
    public AudioSource audioSource;

    [Header("Settings")]
    public AudioClip[] footstepSounds;
    //readonly string animParam_Walking = "walking";

    //[Header("State")] 
    //public bool walking = false;

    void Awake()
    {
        if (!animator)
            animator = GetComponent<Animator>();

        if (!motor)
            motor = GetComponentInParent<KinematicCharacterMotor>();

        if (!audioSource)
            audioSource = GetComponentInParent<AudioSource>();

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

    public void Footstep()
    {
        int random = Random.Range(0, footstepSounds.Length);
        var clip = footstepSounds[random];
        audioSource.PlayOneShot(clip);
    }
}
