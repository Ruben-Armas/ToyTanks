using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public MenuView initiallyOpen;

    private MenuView currentView;

    private void Start()
    {
        if (initiallyOpen != null)
        {
            //Inicia con animación
            OpenView(initiallyOpen);
            //Alternativa para que no haga animación la primera vez que aparece el menu
            /*
            initiallyOpen.canvasGroup.alpha = 1;
            initiallyOpen.canvasGroup.blocksRaycasts = true;
            initiallyOpen.canvasGroup.interactable = true;
            currentView = initiallyOpen;
            */
        }
    }

    public void OpenView(MenuView nextView)
    {
        StartCoroutine(DoOpenView(nextView));
    }

    //yield return  -- hace que espere
    IEnumerator DoOpenView(MenuView nextView)
    {
        //Si hay una vista abierta la cerramos
        if (currentView != null)
        {
            yield return StartCoroutine(currentView.CloseView());
        }
        nextView.gameObject.SetActive(true);
        //Para cambiar la gerarquía
        nextView.transform.SetAsLastSibling();

        //Abrimos la nextView
        yield return StartCoroutine(nextView.OpenView());
        currentView = nextView;

    }
}
