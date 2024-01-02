using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampas : MonoBehaviour
{
    private ControlDelSonido controlDelSonido;
    private Jugador jugador;
    Transform objetivoDelEnemigo;
    float[] dañoDeTrampa = new float[5]{0.15f, 0.20f, 0.25f, 0.30f, 0f};//{0.05f, 0.10f, 0.15f, 0.20f, 0f};      
    private bool habilitarDaño = false;
    private float tiempo = 1;

    void Awake()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Jugador>();
        objetivoDelEnemigo = GameObject.FindGameObjectWithTag("Player").transform; 
    } 

    void Start()
    {
        controlDelSonido = GameObject.FindGameObjectWithTag("Sonidos").GetComponent<ControlDelSonido>();//Ojo importante, permite el Sonido
    }   

    void OnTriggerStay(Collider colliderPersonaje) 
    { 

        if (colliderPersonaje.tag == "Player" && !habilitarDaño)
        {    
            StartCoroutine(EjecutarDañoDeTrampa());

            if (InterfazDeUsuario.nivelAmostrar >= 10 && InterfazDeUsuario.nivelAmostrar < 20)
            {
                colliderPersonaje.GetComponent<IDaño>().RecibirAtaqueDeTrampa(jugador.PuntosDeVidaIniciales * dañoDeTrampa[0]);
                Debug.Log("Daño Recibido - " + jugador.PuntosDeVidaIniciales * dañoDeTrampa[0]);//Mostrar Informacion - 
            } 
            
            else if (InterfazDeUsuario.nivelAmostrar >= 20 && InterfazDeUsuario.nivelAmostrar < 30)
            {
                colliderPersonaje.GetComponent<IDaño>().RecibirAtaqueDeTrampa(jugador.PuntosDeVidaIniciales * dañoDeTrampa[1]);
                Debug.Log("Daño Recibido - " + jugador.PuntosDeVidaIniciales * dañoDeTrampa[1]);//Mostrar Informacion - 
            } 

            else if (InterfazDeUsuario.nivelAmostrar >= 30 && InterfazDeUsuario.nivelAmostrar < 40)
            {
                colliderPersonaje.GetComponent<IDaño>().RecibirAtaqueDeTrampa(jugador.PuntosDeVidaIniciales * dañoDeTrampa[2]);
                Debug.Log("Daño Recibido - " + jugador.PuntosDeVidaIniciales * dañoDeTrampa[2]);//Mostrar Informacion - 
            } 

            else if (InterfazDeUsuario.nivelAmostrar >= 40)
            {
                colliderPersonaje.GetComponent<IDaño>().RecibirAtaqueDeTrampa(jugador.PuntosDeVidaIniciales * dañoDeTrampa[3]);
                Debug.Log("Daño Recibido - " + jugador.PuntosDeVidaIniciales * dañoDeTrampa[3]);//Mostrar Informacion - 
            } 

            else
            {
                colliderPersonaje.GetComponent<IDaño>().RecibirAtaqueDeTrampa(jugador.PuntosDeVidaIniciales * dañoDeTrampa[4]);
                Debug.Log("Daño Recibido - " + jugador.PuntosDeVidaIniciales * dañoDeTrampa[4]);//Mostrar Informacion - 
            }   

            if (ControlDelSonido.instanciaControlDelSonido != null)//Ejecutar el código, sólo cuando exista una instancia.
            {
                controlDelSonido.ActivarPista("Trampa");
            }         
        }        
    }

    IEnumerator EjecutarDañoDeTrampa()
    {
        habilitarDaño = true;
       
        yield return new WaitForSeconds(tiempo);

        habilitarDaño = false;
    }

    void Update()
    {
        //05.01 
        if (InterfazDeUsuario.ganoPartida == true || objetivoDelEnemigo == null || InterfazDeUsuario.misionFallida == true)
        {
            GameObject.Destroy(gameObject);
        }

    }
}
