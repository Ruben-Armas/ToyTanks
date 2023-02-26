using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownPlayerHandler : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    //Guarda el valor tanto si se ha cambiado como si no
    void Start()
    {
        int numPlayers = dropdown.value + 1;
        Debug.Log($"Start players: {numPlayers}");

        // Guardar el valor inicial en PlayerPrefs
        PlayerPrefs.SetInt("numPlayers", numPlayers);
    }
    public void OnPlayButtonClicked()
    {
        //Valor seleccionado
        int numPlayers = dropdown.value + 1;
        Debug.Log($"Players: {numPlayers}");

        // Guardar el valor en PlayerPrefs  (para poder pasarlo a la otra escena)
        PlayerPrefs.SetInt("numPlayers", numPlayers);
    }

    /*//Solo cuando cambia (en el OnChange del DropDown)
    public void NumOfPlayersChanged(int num)
    {
        Debug.Log(num);
        int numPlayers = num + 1;

        // Guardar el valor en PlayerPrefs  (para poder pasarlo a la otra escena)
        PlayerPrefs.SetInt("numPlayers", numPlayers);
    }*/
}
