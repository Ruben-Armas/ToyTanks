using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownPlayerHandler : MonoBehaviour
{
    /*Dropdown NumOfPlayersdropdown;

    public void OnNumOfPlayersDropdownChanged()
    {
        NumOfPlayersdropdown = GetComponent<Dropdown>();
        int numPlayers = NumOfPlayersdropdown.value + 1;

        // Guardar el valor en PlayerPrefs  (para poder pasarloa a la otra escena)
        PlayerPrefs.SetInt("numPlayers", numPlayers);
    }
    */
    private void NumOfPlayersChanged(int num)
    {
        Debug.Log(num);
        int numPlayers = num + 1;

        // Guardar el valor en PlayerPrefs  (para poder pasarloa a la otra escena)
        PlayerPrefs.SetInt("numPlayers", numPlayers);
    }
}
