using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Veil : MonoBehaviour
{
    /*
        Aunque _instance este a null no significa que no se hay inicializado ya
        (Puede ser que no exista) o que exista pero no se haya usado
    */ 
    private static Veil _instance;  //SINGLETON
    public static Veil instance
    {
        get
        {
            // comprobar si existe (_instance != null)
            if (_instance != null)
                return _instance;
            // si no, lo buscamos
            _instance = FindObjectOfType<Veil>();
            // si no existe lo creo
            if (_instance == null)
            {
                //Lo creamos
                GameObject prefab = Resources.Load("Veil") as GameObject;   //Busco el prefab en el proyecto Carpeta Resources
                GameObject go = Instantiate(prefab);
                _instance = go.GetComponent<Veil>();
            }
            return _instance;   //Ahora ya hay un Singleton Veil
        }
    }

    public CanvasGroup canvasGroup;
    public float animationTime = 0.5f;
    public bool fadeOutAwake;

    private void Awake()
    {
        //Para que no se destruya el velo cuando se carguen otras escenas, así no desaparece y causa que sea brusco
        //DontDestroyOnLoad(gameObject);
        
        if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (fadeOutAwake)
        {
            StartCoroutine(Fade(0));
        }
    }

    IEnumerator Fade(float to)
    {
        canvasGroup.blocksRaycasts = to == 1;

        float elapsedTime = 0;
        float from = canvasGroup.alpha;
        while (elapsedTime < animationTime)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsedTime / animationTime);
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.unscaledDeltaTime;  //Usa el tiempo ignorando la escala (funciona la pausa)
        }
        canvasGroup.alpha = to;
    }

    public void LoadScene(string nextScene)
    {
        //Primero fundido a negro   |   Espera  |   Después carga de la escena
        StartCoroutine(DoLoadScene(nextScene));
    }

    IEnumerator DoLoadScene(string nextScene)
    {
       //OCULTO EL JUEGO
        yield return StartCoroutine(Fade(1));

       //CARGO EL JUEGO
        //SceneManager.LoadScene(nextScene);      //Cargo la escena
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene); //Cargo Asíncrona de la escena (NO BLOQUEA)
            //.LoadSceneAsync(nextScene, LoadSceneMode.Additive);   modo Aditivo que permite CARGAR escenas SIN DESCARGAR la anterior
        //op.progress   //devuelve el progreso
        op.allowSceneActivation = false;   //pulsa para iniciar, después de cargar
        while (op.progress < 0.9f)
        {
            Debug.Log(op.progress);
            yield return 0;
        }
        op.allowSceneActivation = true; //Activa la escena (deberíamos poner un botón para empezar)

       //OCULTO EL VELO
        yield return StartCoroutine(Fade(0));
    }
}