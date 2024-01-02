using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurarPersonaje : MonoBehaviour
{
    private ControlDelSonido controlDelSonido;
    private Jugador jugador;
    Transform objetivoDelEnemigo;
    public GameObject efectoCuracion;
    float[] curacion = new float[5]{0.30f, 0.25f, 0.20f, 0.15f, 0f};//{0.40f, 0.30f, 0.20f, 0.10f, 0f};    
    float anguloY;
    private float velocidad = 2.0F;

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
        if (colliderPersonaje.tag == "Player")
        {          
            if (InterfazDeUsuario.nivelAmostrar <= 10)
            {
                colliderPersonaje.GetComponent<IDaño>().RecibirCuracion (jugador.PuntosDeVidaIniciales * curacion[0]);
                Debug.Log("Se curo - " + jugador.PuntosDeVidaIniciales * curacion[0]);
            } 
            
            else if (InterfazDeUsuario.nivelAmostrar >= 11 && InterfazDeUsuario.nivelAmostrar <= 20)
            {
                colliderPersonaje.GetComponent<IDaño>().RecibirCuracion (jugador.PuntosDeVidaIniciales * curacion[1]);
                Debug.Log("Se curo - " + jugador.PuntosDeVidaIniciales * curacion[1]);
            } 

            else if (InterfazDeUsuario.nivelAmostrar >= 21 && InterfazDeUsuario.nivelAmostrar <= 30)
            {
                colliderPersonaje.GetComponent<IDaño>().RecibirCuracion (jugador.PuntosDeVidaIniciales * curacion[2]);
                Debug.Log("Se curo - " + jugador.PuntosDeVidaIniciales * curacion[2]);
            } 

            else if (InterfazDeUsuario.nivelAmostrar >= 31)
            {
                colliderPersonaje.GetComponent<IDaño>().RecibirCuracion (jugador.PuntosDeVidaIniciales * curacion[3]);
                Debug.Log("Se curo - " + jugador.PuntosDeVidaIniciales * curacion[3]);
            } 

            else
            {
                colliderPersonaje.GetComponent<IDaño>().RecibirCuracion (jugador.PuntosDeVidaIniciales * curacion[4]);
                Debug.Log("Se curo - " + jugador.PuntosDeVidaIniciales * curacion[4]);
            }

            if (ControlDelSonido.instanciaControlDelSonido != null)//Ejecutar el código, sólo cuando exista una instancia.
            {
                controlDelSonido.ActivarPista("Curacion");
            }             

            GameObject.Destroy(gameObject); //despues de curar destruir el objeto

            Destroy(Instantiate(efectoCuracion, transform.position - Vector3.up * 0.4f, Quaternion.identity), 1.5f);
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

    /*
        float anguloX = Utilidades.ObtenerNumeroAleatorio(-45, 45);
        float anguloY = Utilidades.ObtenerNumeroAleatorio(-180, 180);
        float anguloZ = Utilidades.ObtenerNumeroAleatorio(-180, 180);
        transform.rotation = Quaternion.Euler(0f, anguloY, 0f);
        transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, Time.time * speed);
            
    */
}
