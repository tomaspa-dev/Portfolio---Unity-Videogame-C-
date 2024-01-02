using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtorgarPuntos : MonoBehaviour
{
    private ControlDelSonido controlDelSonido;
    private Jugador jugador;
    Transform objetivoDelEnemigo;
    float anguloY;

    void Awake()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Jugador>();
        objetivoDelEnemigo = GameObject.FindGameObjectWithTag("Player").transform; 
    }

    void Start()
    {
        controlDelSonido = GameObject.FindGameObjectWithTag("Sonidos").GetComponent<ControlDelSonido>();//Ojo importante, permite el Sonido
        StartCoroutine(GenerarAnguloAleatorio());
    }

    void OnTriggerEnter(Collider colliderPersonaje) 
    {        
        if (colliderPersonaje.tag == "Player" && InterfazDeUsuario.ganoPartida == false)
        {
            InterfazDeUsuario.OtorgarPuntos();
            GameObject.Destroy (gameObject);

            if (ControlDelSonido.instanciaControlDelSonido != null)//Ejecutar el código, sólo cuando exista una instancia.
            {
                controlDelSonido.ActivarPista("Puntos");
            }  
        }        
    }

    IEnumerator GenerarAnguloAleatorio()
    {
        yield return null;

        anguloY = Utilidades.ObtenerNumeroAleatorio(-180, 180);
    }
    
    void Update()
    {
        transform.Rotate(new Vector3(0f, anguloY, 0f) * Time.deltaTime);

        //05.01 
        if (InterfazDeUsuario.ganoPartida == true || objetivoDelEnemigo == null || InterfazDeUsuario.misionFallida == true)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
