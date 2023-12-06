using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 21f;
    public GameObject speedBoost;
    public AudioSource speedSound;
    public GameObject speedUI;
    public float playerSpeed = 1f;
    public float moveDirection = 1f;
    public GameObject inverseUI;
    public AudioSource alertSound;
    public GameObject alert;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
        Instantiate(speedBoost, new Vector3(-2.01f, -0.32f, 5.18f), Quaternion.identity);
        Instantiate(speedBoost, new Vector3(-1.17f, -0.8f, 10.45f), Quaternion.identity);
        StartCoroutine("ChangeDirection");
    }

    // Update is called once per frame
    void FixedUpdate()
    {    
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //Multiply each movement value by a variable. When variable is positive, movement is normal. When the variable is negative, movement becomes inversed.
        m_Movement.Set(horizontal * moveDirection, 0f * moveDirection, vertical * moveDirection);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop();
        }

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);
    }

    void OnAnimatorMove()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude * playerSpeed);
        m_Rigidbody.MoveRotation(m_Rotation);
    }

    void OnTriggerEnter(Collider pickup)
    {
        if (pickup.name == "Bottle_Mana(Clone)")
        {
            speedUI.SetActive(true);
            playerSpeed = 2f;
            StartCoroutine("SpeedGone");
            Destroy(pickup.gameObject);
            speedSound.Play();
        }
    }

    IEnumerator SpeedGone()
    {
        yield return new WaitForSeconds(5f);
        playerSpeed = 1;
        speedUI.SetActive(false);
    }

    IEnumerator ChangeDirection()
    {
        //Inverts player's controls after a random amount of time within the range and displays text on screen.
        yield return new WaitForSeconds(Random.Range(12.5f, 16.5f));
        alertSound.Play();
        alert.SetActive(true);
        moveDirection = -1f;
        inverseUI.SetActive(true);
        StartCoroutine("ResetDirection");
        yield return new WaitForSeconds(2.1f);
        alert.SetActive(false);
    }

    IEnumerator ResetDirection()
    {
        //Returns player's controls back to normal after a set amount of time and repeats the cycle.
        yield return new WaitForSeconds(Random.Range(5.5f, 7.5f));
        moveDirection = 1f;
        inverseUI.SetActive(false);
        StartCoroutine("ChangeDirection");
    }    
}