using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField]  SpriteRenderer bodySprite;

    public void SetColor(Color newColor)
    {
        bodySprite.color = newColor;
    }

    private void OnEnable()
    {
        if(PlayerController.allBodies != null)
        {
            PlayerController.allBodies.Add(transform);
        }
    }

    public void Report()
    {
        Debug.Log("Reported");
        Destroy(gameObject);
    }
}
