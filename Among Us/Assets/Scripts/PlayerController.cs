using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] bool hasControl = true;
    public static PlayerController localPlayer;
    //Components
    Rigidbody playerRigidbody;
    Transform playerAvatar;
    Animator playerAnimator;
    
    //Player movement
    [SerializeField] InputAction WASD;
    Vector2 movementInput;
    [SerializeField] float movementSpeed;

    //Player Color
    static Color playerColor;
    SpriteRenderer playerAvatarSprite;

    //Role
    [SerializeField] bool isImposter;
    [SerializeField] InputAction KILL;
    float killInput;

    List<PlayerController> targets;
    [SerializeField] Collider playerCollider;

    bool isDead;

    [SerializeField] GameObject bodyPrefab;

    public static List<Transform> allBodies;

    List<Transform> bodiesFound;

    [SerializeField] InputAction Report;
    [SerializeField] LayerMask ignoreForBody;

    private void Awake()
    {
        KILL.performed += KillTarget;
        Report.performed += ReportBody;
    }


    private void OnEnable()
    {
        WASD.Enable();
        KILL.Enable();
        Report.Enable();
        
    }

    private void OnDisable()
    {
        WASD.Disable();
        KILL.Disable();
        Report.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (hasControl)
        {
            localPlayer = this;
        }
        targets = new List<PlayerController>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAvatar = transform.GetChild(0);
        playerAnimator = GetComponent<Animator>();

        playerAvatarSprite = playerAvatar.GetComponent<SpriteRenderer>();
        if(playerColor == Color.clear)
        {
            playerColor = Color.white;
        }
        if (!hasControl)
            return;

        playerAvatarSprite.color = playerColor;

        allBodies = new List<Transform>();
        bodiesFound = new List<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!hasControl)
            return;

        movementInput = WASD.ReadValue<Vector2>();  //Getting player's input

        if(movementInput.x != 0)
        {
            playerAvatar.localScale = new Vector2(Mathf.Sign(movementInput.x), 1); //return value is 1 when input is positive, -1 when input is negative.
        }

        playerAnimator.SetFloat("Speed", movementInput.magnitude);

        if(allBodies.Count > 0)
        {
            BodySearch();
        }
    }

    void BodySearch()
    {
        foreach(Transform body in allBodies)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, body.position - transform.position);
            Debug.DrawRay(transform.position, body.position - transform.position, Color.cyan);
            if(Physics.Raycast(ray, out hit, 1000f, ~ignoreForBody))
            {
                if(hit.transform == body)
                {
                   // Debug.Log(hit.transform.name);
                    //Debug.Log(bodiesFound.Count);
                    if(bodiesFound.Contains(body.transform))
                        return;
                    bodiesFound.Add(body.transform);
                }
                else
                {
                    bodiesFound.Remove(body.transform);
                }
            }
        }
    }

    private void ReportBody(InputAction.CallbackContext obj)
    {
        if (bodiesFound == null)
            return;
        if (bodiesFound.Count == 0)
            return;
        Transform tempBody = bodiesFound[bodiesFound.Count - 1];
        allBodies.Remove(tempBody);
        bodiesFound.Remove(tempBody);
        tempBody.GetComponent<Body>().Report();
    }

    private void FixedUpdate()
    {
        playerRigidbody.velocity = movementInput * movementSpeed;
    }

    public void SetColor(Color newColor) //Updating the color of player's sprite renderer component
    {
        playerColor = newColor;
        if(playerAvatarSprite != null)
        {
            playerAvatarSprite.color = playerColor;
        }
    }

    public void SetRole(bool newRole)
    {
        isImposter = newRole;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerController tempTarget = other.GetComponent<PlayerController>();
            if (isImposter)
            {
                if (tempTarget.isImposter)
                    return;
                else
                {
                    targets.Add(tempTarget);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerController tempTarget = other.GetComponent<PlayerController>();
            if (targets.Contains(tempTarget))
            {
                targets.Remove(tempTarget);
            }
        }
    }
    void KillTarget(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            if (targets.Count == 0)
                return;
            else
            {
                if (targets[targets.Count - 1].isDead)
                    return;
                transform.position = targets[targets.Count - 1].transform.position;
                targets[targets.Count - 1].Die();
                targets.RemoveAt(targets.Count - 1);
            }
        }
    }
    
    public void Die()
    {
        isDead = true;
        playerAnimator.SetBool("IsDead", isDead);
        gameObject.layer = 8;
        playerCollider.enabled = false;

        Body tempBody = Instantiate(bodyPrefab, transform.position, transform.rotation).GetComponent<Body>();
        tempBody.SetColor(playerAvatarSprite.color);

    }
}
