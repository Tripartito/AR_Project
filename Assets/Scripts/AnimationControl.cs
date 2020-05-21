using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    private Animator animator;
    private Vector3 last_position;
    private float timer;

    private enum States
    {
        Idle,
        Move, 
        Attack
    };
    States state = States.Idle;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        last_position = gameObject.transform.position;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("attack") == true)
        {
            if (state == States.Idle || state == States.Move)
                timer = Time.time;

            state = States.Attack;
        }

        else if (Mathf.Abs((gameObject.transform.position.magnitude - last_position.magnitude)) > 0.1f)
        {
            timer = Time.time;
            state = States.Move;
        }

        else if (Mathf.Abs(timer - Time.time) > 1.0f)
            state = States.Idle;


        switch (state)
        {
            case States.Idle:
                animator.SetBool("moving", false);
                break;
            case States.Move:
                animator.SetBool("moving", true);
                animator.SetBool("attack", false);
                break;
            case States.Attack:
                if (Mathf.Abs(timer - Time.time) > 1.0f)
                {
                    state = States.Idle;
                    animator.SetBool("attack", false);
                }
                break;
            default:
                break;
        }

        last_position = gameObject.transform.position;
    }

    public void Attack()
    {
        animator.SetBool("attack", true);
    }
}
