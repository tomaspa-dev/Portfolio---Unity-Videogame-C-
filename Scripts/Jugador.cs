using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugador : Personaje
{
    public enum claseDelJugador {cortoAlcance, largoAlcance}

    protected override void Start()
    {
        base.Start(); 

        if (InterfazDeUsuario.nivelAmostrar >= 1)//Comentario - nivelAmostrar == 1
        {
            puntosDeVidaPersonaje = 1000;//Comentario - puntosDeVidaPersonaje = 100
        } 
   
        puntosDeVida = Personaje.puntosDeVidaPersonaje; 
    }  

    public float PuntosDeVidaIniciales 
    { 
        get => puntosDeVidaPersonaje; 
        private set => puntosDeVidaPersonaje = value; 
    }

    public float PuntosDeVida
    {
        get => puntosDeVida; 
        private set => puntosDeVida = value; 
    }

    public bool Muerto
    {
        get => muerto; 
        private set => muerto = value;
    }

    public override void AumentarPuntosDeVida(float puntosDeVida) 
    {
        if (Muerto == false && puntosDeVidaPersonaje < maximosPuntosDeVida)
        {
            if ((puntosDeVidaPersonaje + puntosDeVida) < maximosPuntosDeVida)
            {
                puntosDeVidaPersonaje += puntosDeVida;
            }            

            else
            {
                puntosDeVidaPersonaje = maximosPuntosDeVida;
            }
        }
    }
}
