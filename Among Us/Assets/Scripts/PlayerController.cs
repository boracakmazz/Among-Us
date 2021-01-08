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

    private void OnEnable()
    {
        WASD.Enable();
        
    }

    private void OnDisable()
    {
        WASD.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (hasControl)
        {
            localPlayer = this;
        }

        playerRigidbody = GetComponent<Rigidbody>();
        playerAvatar = transform.GetChild(0);
        playerAnimator = GetComponent<Animator>();

        playerAvatarSprite = playerAvatar.GetComponent<SpriteRenderer>();
        if(playerColor == Color.clear)
        {
            playerColor = Color.white;
        }
        playerAvatarSprite.color = playerColor;
    }

    // Update is called once per frame
    void Update()
    {
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
}
