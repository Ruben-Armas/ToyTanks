using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownPlayerHandler : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown NumOfPlayersDropdown;
    [SerializeField] private TMP_Dropdown InputDropdown;

    private int numPlayers;
    private int inputP1;

    //Guarda el valor tanto si se ha cambiado como si no
    void Start()
    {
        getSaveOption();
        Debug.Log($"Start players: {numPlayers}");
        Debug.Log($"Start input: {inputP1}");
    }
    public void SavePlayerConfigDropdowns()
    {
        getSaveOption();
        Debug.Log($"Players: {numPlayers}");
        Debug.Log($"Input: {inputP1}");
    }

    private void getSaveOption()
    {
        //Valor seleccionado
        numPlayers = NumOfPlayersDropdown.value + 1;
        inputP1 = InputDropdown.value;

        // Guardar el valor en PlayerPrefs  (para poder pasarlo a la otra escena)
        PlayerPrefs.SetInt("numPlayers", numPlayers);
        PlayerPrefs.SetInt("inputP1", inputP1);
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
