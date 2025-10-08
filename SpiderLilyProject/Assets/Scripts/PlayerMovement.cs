using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Rigidbody rb;

    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] float sprintMod;
    [SerializeField] float jumpHeight;
    [SerializeField] int jumpMax;

    Vector3 moveDir;
    int jumpCount;
    int HPOrig;

    bool isSprinting;
    bool isMatchLit;

    [Header("Match")]

    [SerializeField] GameObject matchModel;
    [SerializeField] int matchMax;
    [SerializeField] float matchRadius;
    [SerializeField] float matchTimer;
    int matchListPos;

    [Header("Audio")]

    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audSteps;
    [UnityEngine.Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioClip[] audHurt;
    [UnityEngine.Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audJump;
    [UnityEngine.Range(0, 1)][SerializeField] float audJumpVol;
    void Start()
    {
        HPOrig = HP;
    }

    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * matchRadius, Color.red);
        movement();
        sprint();
    }
    void movement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveDir = (transform.right * h + transform.forward * v).normalized;
        Vector3 newVel = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);
        rb.linearVelocity = newVel;
        jump();
    }
    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            //aud.PlayOneShot(audJump[UnityEngine.Random.Range(0, audJump.Length)], audJumpVol);
            jumpCount++;
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }
    }
    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        { 
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered" + other.name);
        if(other.TryGetComponent<iPickup>(out var pickup))
        {
            pickup.OnPickup(other.gameObject);
        }
    }
}
