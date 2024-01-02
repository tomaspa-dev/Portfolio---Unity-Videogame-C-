using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    //public Transform tirador;
    public Color colorEstelaDeBala;
    public LayerMask colision;
    public float velocidadBala = 35;
    bool esJugador;
    string recibioDisparo;
    string ejecutoDisparo;
    float[] dañoDelJugador = new float[2] {0, 20}; //Daño del Jugador a 10, 100 Modo invencible.
    //float[] dañoDelEnemigo = new float[5] {0, 20, 10, 25, 50};
    float[] dañoDelTanque = new float[5] {5, 8, 10, 13, 16}; 
    float[] dañoDelDpsBajo = new float[5] {10, 15, 20, 25, 30};
    float[] dañoDelDpsAlto = new float[5] {10, 20, 30, 40, 50};
    float[] dañoDelDpsEspecial = new float[5] {5, 25, 35, 45, 55};  

    //public static float aumentarDañoDelJugador { get; set; }
    float daño;
    float duracionBala = .25f;//.25f;
    private float distanciaRecorrida;

    void Start()
    {
        Destroy(gameObject, duracionBala);   
        GetComponent<TrailRenderer>().material.SetColor("_TintColor", colorEstelaDeBala); 
    }

    void Update()
    {      
        if (gameObject != null)
        {
            distanciaRecorrida = velocidadBala * Time.deltaTime;
            VerificarColision(distanciaRecorrida);
            transform.Translate(Vector3.forward * distanciaRecorrida);           
        }
    }
    
    public void AsignarVelocidad(float nuevaVelocidadBala)
    {
        velocidadBala = nuevaVelocidadBala;
    }

    public void VerificarColision(float distanciaRecorrida) 
    {
        Ray rayo = new Ray(transform.position, transform.forward);
        RaycastHit objetivoGolpeado;

        if (Physics.Raycast(rayo, out objetivoGolpeado, distanciaRecorrida, colision, QueryTriggerInteraction.Collide))
        {
            GolperObjetivo(objetivoGolpeado);            
            //GolperObjetivo(objetivoGolpeado.collider, objetivoGolpeado.point);//Posible Refactorización
        }
    }

    //public void GolperObjetivo(Collider objetivo, Vector3 puntoGolpeado)//Posible Refactorización
    public void GolperObjetivo(RaycastHit objetivoGolpeado)
    {   
        IDaño objetoDañado = objetivoGolpeado.collider.GetComponent<IDaño>();     

        if (objetoDañado != null)
        {            
            if (ejecutoDisparo == "Player")
            {
                esJugador = true;
                daño = ObtenerValorDelDaño(esJugador);
            }

            else
            {
                esJugador = false;
                daño = ObtenerValorDelDaño(esJugador, recibioDisparo);
            }
            
            Debug.Log("Nombre - " + ejecutoDisparo + ", Funcion -" + gameObject.tag + ", disparo, " + esJugador + ", el daño es: " + daño);//Comentario - Mostrar Información de quien ejecuta el disparo.
            objetoDañado.RecibirDisparo(daño, objetivoGolpeado);
        }

        GameObject.Destroy(gameObject);        
    }

    public void MostrarQuienDisparo(string quienDisparo)
    {
        ejecutoDisparo = quienDisparo;
    }

    public float ObtenerValorDelDaño(bool esJugador)
    {
        if (esJugador == true)
        {
            daño = dañoDelJugador[1] + InterfazDeUsuario.aumentarDañoDelJugador;
        }   

        return daño;
    }

    public float ObtenerValorDelDaño(bool esJugador, string tipo)
    {
        if (esJugador == false)
        {         
            if (ejecutoDisparo == "Tipo1" && gameObject.tag == "Tanque")
            {
                daño = dañoDelTanque[0];
            }  

            else if (ejecutoDisparo == "Tipo1" && gameObject.tag == "DPS Rango Bajo")
            {
                daño = dañoDelDpsBajo[0];
            }  

            else if (ejecutoDisparo == "Tipo1" && gameObject.tag == "DPS Rango Alto")
            {
                daño = dañoDelDpsAlto[0];
            }  

            else if (ejecutoDisparo == "Tipo1" && gameObject.tag == "Especial")
            {
                daño = dañoDelDpsEspecial[0];
            }    

            else if (ejecutoDisparo == "Tipo2" && gameObject.tag == "Tanque")
            {
                daño = dañoDelTanque[1];
            }  

            else if (ejecutoDisparo == "Tipo2" && gameObject.tag == "DPS Rango Bajo")
            {
                daño = dañoDelDpsBajo[1];
            }  
            
            else if (ejecutoDisparo == "Tipo2" && gameObject.tag == "DPS Rango Alto")
            {
                daño = dañoDelDpsAlto[1];
            }  

            else if (ejecutoDisparo == "Tipo2" && gameObject.tag == "Especial")
            {
                daño = dañoDelDpsEspecial[1];
            } 

            else if (ejecutoDisparo == "Tipo3" && gameObject.tag == "Tanque")
            {
                daño = dañoDelTanque[2];
            }  

            else if (ejecutoDisparo == "Tipo3" && gameObject.tag == "DPS Rango Bajo")
            {
                daño = dañoDelDpsBajo[2];
            }  
            
            else if (ejecutoDisparo == "Tipo3" && gameObject.tag == "DPS Rango Alto")
            {
                daño = dañoDelDpsAlto[2];
            }  

            else if (ejecutoDisparo == "Tipo3" && gameObject.tag == "Especial")
            {
                daño = dañoDelDpsEspecial[2];
            } 

            else if (ejecutoDisparo == "Tipo4" && gameObject.tag == "Tanque")
            {
                daño = dañoDelTanque[3];
            }  

            else if (ejecutoDisparo == "Tipo4" && gameObject.tag == "DPS Rango Bajo")
            {
                daño = dañoDelDpsBajo[3];
            }  
            
            else if (ejecutoDisparo == "Tipo4" && gameObject.tag == "DPS Rango Alto")
            {
                daño = dañoDelDpsAlto[3];
            }   

            else if (ejecutoDisparo == "Tipo4" && gameObject.tag == "Especial")
            {
                daño = dañoDelDpsEspecial[3];
            } 

            else
            {
                daño = dañoDelTanque[0];
                Debug.LogWarning("Funcion o Tipo de enemigo indeterminado - "+ ejecutoDisparo+ "," + gameObject.tag);//Comentario - No se determina quien dispara.
            }           
        }        

        return daño;
    }
}
