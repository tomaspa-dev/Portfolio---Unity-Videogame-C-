using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlDelArma : MonoBehaviour
{
    private ControlDelSonido controlDelSonido;
    public Transform posicionDelArma;
    public Arma armaInicial;
    Arma armaEquipada;   
    bool esElEnemigo = true;  

    void Start()
    {
        if (armaInicial != null)
        {
            equiparArma(armaInicial);
            controlDelSonido = GameObject.FindGameObjectWithTag("Sonidos").GetComponent<ControlDelSonido>();//Ojo importante, permite el Sonido
        }        
    }      

    public void equiparArma(Arma armaParaEquipar)
    {
        if (armaEquipada != null)
        {
            Destroy(armaEquipada.gameObject);            
        }

        armaEquipada = Instantiate(armaParaEquipar, posicionDelArma.position, posicionDelArma.rotation) as Arma;
        armaEquipada.transform.parent = posicionDelArma;    
    }  

    public void Disparar()
    {
        if (armaEquipada != null)
        {            
            armaEquipada.Disparar(esElEnemigo);
        }
    }

    public void AlPresionarGatillo()
    {
        if (armaEquipada != null)
        {            
            armaEquipada.AlPresionarGatillo();          
        }
    }

    public void AlSoltarGatillo()
    {
        if (armaEquipada != null)
        {
            armaEquipada.AlSoltarGatillo();

            /*
            if (ControlDelSonido.instanciaControlDelSonido != null)
            {
                if (InterfazDeUsuario.juegoPausado == false && InterfazDeUsuario.ganoPartida == false)
                {
                    controlDelSonido.ActivarPista("Disparo");
                }                
            }
            */
        }
    }
}
