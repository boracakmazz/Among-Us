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

    private void Awake()
    {
        KILL.performed += KillTarget;
       
    }

    private void OnEnable()
    {
        WASD.Enable();
        KILL.Enable();
        
    }

    private void OnDisable()
    {
        WASD.Disable();
        KILL.Disable();
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
        playerCollider.enabled = false;

        Body tempBody = Instantiate(bodyPrefab, transform.position, transform.rotation).GetComponent<Body>();
        tempBody.SetColor(playerAvatarSprite.color);

    }
}
