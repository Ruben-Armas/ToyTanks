using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeFirstSelectedButton : MonoBehaviour
{
    public GameObject firstSelectedButton;

    public void SetFirstSelectedButton(string path)
    {
        Debug.Log("-----------------------------------------");
        // Buscar el primer botón dentro de MenuHolder -> ButtonsHolder
        /*GameObject buttonsHolder = transform.Find(path).gameObject;
        if (buttonsHolder != null)
        {
            Button firstButton = buttonsHolder.GetComponentInChildren<Button>();
            if (firstButton != null)
            {
                firstSelectedButton = firstButton.gameObject;
            }
        }
        if (firstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);*/
    }
}
