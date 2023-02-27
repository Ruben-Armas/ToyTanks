using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectButton : MonoBehaviour
{
    public void OnSelectButton()
    {
        // Deseleccionar el bot�n actual
        //EventSystem.current.SetSelectedGameObject(null);

        //Debug.Log(EventSystem.current.currentSelectedGameObject);
        Debug.Log($"--{this.gameObject}--");

        // Seleccionar el nuevo bot�n
        //EventSystem.current.firstSelectedGameObject = this.gameObject;
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}
