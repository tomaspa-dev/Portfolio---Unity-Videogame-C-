using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtraparObjetoDeMision : MonoBehaviour
{
    private ControlDelSonido controlDelSonido;
    private Jugador jugador;
    float anguloY;

    void Awake()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Jugador>();
    }

    void Start()
    {
        controlDelSonido = GameObject.FindGameObjectWithTag("Sonidos").GetComponent<ControlDelSonido>();//Ojo importante, permite el Sonido
        StartCoroutine(GenerarAnguloAleatorio());
    }

    void OnTriggerEnter(Collider colliderPersonaje) 
    {        
        if (colliderPersonaje.tag == "Player")
        {                      
            InterfazDeUsuario.CalcularObjetivosDeMision();
            Debug.Log("Objetos de Mision Atrapados: + " + InterfazDeUsuario.objetoDeMisionConseguido);//Mostrar Informacion -            
            GameObject.Destroy (gameObject);

            if (ControlDelSonido.instanciaControlDelSonido != null)//Ejecutar el código, sólo cuando exista una instancia.
            {
                controlDelSonido.ActivarPista("Mision");
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
    }
}
