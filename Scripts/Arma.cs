using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arma : MonoBehaviour
{    
    AlmacenDeObjetos almacenDeObjetos;
    public Bala bala;
    private ControlDelSonido controlDelSonido;
    public Transform[] bocaDelArma;
    Bala nuevaBala;
    public float tiempoEntreDisparos = 10;
    public float velocidadDelDisparo = 35;
    float tiempoDelSiguienteDisparo;
    const float MILISEGUNDOS = 100;//1000
    bool seSoltoElGatillo;
    bool esObstaculoParaJugador = false;
    bool esObstaculoParaEnemigo = false;
    string dueñoDelArma;

    bool esJugador;
    string ejecutoDisparo; 

    public void Start() 
    {
        dueñoDelArma = transform.parent.tag;
        controlDelSonido = GameObject.FindGameObjectWithTag("Sonidos").GetComponent<ControlDelSonido>();//Ojo importante, permite el Sonido
    }

    public void Disparar()
    {
        if (Time.time > tiempoDelSiguienteDisparo)
        {
            
            if (!seSoltoElGatillo)
            {
                return;
            }
            
            for (int i = 0; i < 1; i++)
            {
                GenerarNuevaBala();
                
                if (ejecutoDisparo == "Player")
                {
                    nuevaBala.MostrarQuienDisparo(ejecutoDisparo);             
                }
            }
        }        
    } // 06.10 Posible Refactorización - Lanzar la bala cada segundo a traves de una Corutina 

    public void Disparar(bool esElEnemigo)
    {
        if (Time.time > tiempoDelSiguienteDisparo && esElEnemigo && esObstaculoParaEnemigo == false)//29.03
        {
            for (int i = 0; i < 1; i++)
            {
                GenerarNuevaBala();                
                nuevaBala.MostrarQuienDisparo(ejecutoDisparo);                                     
            }            
        }        
    }  

    public void AlPresionarGatillo()
    {  
        if (esObstaculoParaJugador == false)    
        {
            Disparar();
            seSoltoElGatillo = false;  
        }      
    }

    public void AlSoltarGatillo()
    {
        seSoltoElGatillo = true;

        if (ControlDelSonido.instanciaControlDelSonido != null)
        {
            if (InterfazDeUsuario.juegoPausado == false && InterfazDeUsuario.ganoPartida == false && esObstaculoParaJugador == false)
            {
                controlDelSonido.ActivarPista("Disparo");
            }                
        }
    }

    private void GenerarNuevaBala()
    {
        for (int i = 0; i < 1; i++)
        {
            tiempoDelSiguienteDisparo = Time.time + tiempoEntreDisparos / MILISEGUNDOS;
            nuevaBala = Instantiate(bala, bocaDelArma[i].position, bocaDelArma[i].rotation) as Bala;
            nuevaBala.AsignarVelocidad(velocidadDelDisparo);                
            ejecutoDisparo = transform.parent.tag;
            Debug.Log("ejecutoDisparo" + ejecutoDisparo);//Comentario - Quien ejecuto el Disparo
        }
    }

    void OnTriggerStay(Collider colliderPersonaje) 
    {
        if (colliderPersonaje.tag == "Obstaculo" && dueñoDelArma == "Player")//dueñoDelArma == "Player"
        {
            esObstaculoParaJugador = true;
        }

        else if (colliderPersonaje.tag == "Obstaculo" && dueñoDelArma != "Player")
        {
            esObstaculoParaEnemigo = true;
            Debug.Log("El Arma de -" + dueñoDelArma + "se encuentra en el obstaculo - "+ esObstaculoParaEnemigo); 
        }
        
    }

    void OnTriggerExit(Collider colliderPersonaje) 
    {
        if (colliderPersonaje.tag == "Obstaculo" && dueñoDelArma == "Player")//dueñoDelArma == "Player"
        {
            esObstaculoParaJugador = false;
        }

        else if (colliderPersonaje.tag == "Obstaculo" && dueñoDelArma != "Player")
        {
            esObstaculoParaEnemigo = false;
            Debug.Log("El Arma de -" + dueñoDelArma + ", se encuentra en el obstaculo - "+ esObstaculoParaEnemigo); 
        }

        //Debug.Log("Player - Obstaculo, "+ esObstaculoParaJugador); 
    }



    /*
    void Update()
    {
        Debug.Log(dueñoArma +  " - Obstaculo, "+ esObstaculoParaJugador);
    }
    */
}
