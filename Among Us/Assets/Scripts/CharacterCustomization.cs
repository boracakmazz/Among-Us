using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCustomization : MonoBehaviour
{
    [SerializeField] Color[] colors;

    public void SetColor(int colorIndex)
    {
        PlayerController.localPlayer.SetColor(colors[colorIndex]);
    }

    public void NextScene(int sceneIndex)   //To pass main game scene
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
