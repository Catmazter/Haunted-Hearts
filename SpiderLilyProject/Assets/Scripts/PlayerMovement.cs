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
    [SerializeField] float airStamina;
    [SerializeField] float runStamina;

    Vector3 moveDir;
    int jumpCount;
    int HPOrig;
    float airStaminaOrig;
    float runStaminaOrig;
    bool isSprinting;
    bool isHoldingBreath;

    [Header("Match")]

    [SerializeField] GameObject matchModel;
    [SerializeField] List<Match> matchList = new List<Match>();
    [SerializeField] int matchMax;
    [SerializeField] float matchRadius;
    [SerializeField] float matchTimer;
    float matchTimerOrig;
    int matchListPos;
    bool isMatchLit;

    [Header("Audio")]

    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audSteps;
    [UnityEngine.Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioClip[] audHurt;
    [UnityEngine.Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audJump;
    [UnityEngine.Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audBreath;
    [UnityEngine.Range(0, 1)][SerializeField] float audBreathVol;

    bool isPlayingSteps;
    void Start()
    {
        HPOrig = HP;
        runStaminaOrig = runStamina;
        airStaminaOrig = airStamina;
        matchTimerOrig = matchTimer;
        matchInventory();
        matchListPos = matchList.Count - 1;
    }

    void Update()
    {
        movement();
        sprint();
        holdBreath();
    }
    void movement()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * matchRadius, Color.red);
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveDir = (transform.right * h + transform.forward * v).normalized;
        Vector3 newVel = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);
        rb.linearVelocity = newVel;
        jump();
        lightMatch();
        //holdBreath();
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
            runStamina -= Time.deltaTime;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            if (runStamina < runStaminaOrig)
            {
                runStamina += Time.deltaTime;
            }
            isSprinting = false;
        }
    }
    void throwMatch()
    {
        if (Input.GetButtonDown("Throw Match") && isMatchLit)
        {
            if (matchList.Count > 0)
            {
                matchList.RemoveAt(matchListPos);
                matchListPos--;
            }
        }
    }
    void matchInventory()
    {
        for (int matchListCount = 0; matchListCount < matchMax; matchListCount++)
        {
            Match matches = new Match();
            matchList.Add(matches);
        }
    }
    void lightMatch()
    {
        if ((Input.GetButtonDown("Light Match") || Input.GetButtonUp("Light Match")) && matchTimer > 0)
        {
            //matchTimer = matchList[matchListPos].matchTimer;
            matchTimer -= Time.deltaTime;
            isMatchLit = true;
        }
        else if (isMatchLit && matchTimer <= 0)
        {
            //matchTimer = matchList[matchListPos].matchTimer;
            matchList.RemoveAt(matchListPos);
            matchListPos--;
            matchTimer = matchTimerOrig;
            isMatchLit = false;
        }
        throwMatch();
    }
    void holdBreath()
    {
        if (Input.GetButtonDown("Hold Breath") && !isHoldingBreath)
        {
            airStamina -= Time.deltaTime;
            isHoldingBreath = true;
        }
        else if (Input.GetButtonUp("Hold Breath") && isHoldingBreath)
        {
            if (airStamina < airStaminaOrig)
            {
                airStamina += Time.deltaTime;
            }
            isHoldingBreath = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered" + other.name);
        if (other.TryGetComponent<iPickup>(out var pickup))
        {
            pickup.OnPickup(other.gameObject);
        }
    }
    IEnumerator playSteps()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(audSteps[UnityEngine.Random.Range(0, audSteps.Length)], audStepsVol);
        if (isSprinting)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        isPlayingSteps = false;
    }
}
