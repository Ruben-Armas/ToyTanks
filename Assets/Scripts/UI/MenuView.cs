using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class MenuView : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float animationTime = 1;

    private void OnValidate()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Awake()
    {
        //Recoloca todo cuando ejecutamos
        transform.localPosition = Vector3.zero;
        //Le quitamos el alpha, el interactable y el block Raycast
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    //Corrutinas para hacer una transición
    public IEnumerator OpenView()
    {
        float elapsedTime = 0;
        canvasGroup.interactable = true;
        //Reparto la animación en el tiempo
        while (elapsedTime < animationTime)
        {
            //Cambia el alpha de 0 a 1 gradualmente     la división da entre 0 y 1
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime/animationTime);
            yield return new WaitForEndOfFrame(); //Los mismo //yield return 0;
            //elapsedTime += Time.deltaTime;  //Cuando se para el timepo, se queda a 0
            elapsedTime += Time.unscaledDeltaTime;  //Usa el tiempo ignorando la escala (funciona la pausa)   
        }
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }
    public IEnumerator CloseView()
    {
        canvasGroup.blocksRaycasts = false; //Desactivo los raycast primero, para que no se pueda cliclar muchas veces el botón y se vuelva loco
        float elapsedTime = 0;
        canvasGroup.interactable = true;
        //Reparto la animación en el tiempo
        while (elapsedTime < animationTime)
        {
            //Cambia el alpha de 1 a 0 gradualmente     la división da entre 0 y 1
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / animationTime);
            yield return new WaitForEndOfFrame(); //Los mismo //yield return 0;
            //elapsedTime += Time.deltaTime * 2;  //Así tarda menos en cerrar
            elapsedTime += Time.unscaledDeltaTime * 2;  //Usa el tiempo ignorando la escala (funciona la pausa)
        }
        canvasGroup.alpha = 0;
        //canvasGroup.interactable = false;
        gameObject.SetActive(false);
    }
}
