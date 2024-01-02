using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personaje : MonoBehaviour, IDaño
{
    private ControlDelSonido controlDelSonido;
    private Jugador jugador;
    protected float puntosDeVida;
    protected float maximosPuntosDeVida = 1000;  
    public float puntosDeVidaIniciales = 5;
    public static float puntosDeVidaPersonaje { get; set; }
    protected bool muerto;
    public GameObject efectoAlMorir;
    //public event System.Action CuandoMuere;

    void Awake()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Jugador>();    
    }

    protected virtual void Start()
    {
        puntosDeVida = puntosDeVidaIniciales;   
        controlDelSonido = GameObject.FindGameObjectWithTag("Sonidos").GetComponent<ControlDelSonido>();//Ojo importante, permite el Sonido        
    }

    public virtual void RecibirDaño(float daño)
    {
        //Aqui debo calcular el valor del daño, jugador o enemigo
        if (daño <= puntosDeVida)
        {
            puntosDeVida -= daño;
        }

        else
        {
            puntosDeVida = 0; 
        }
        
        if (puntosDeVida <= 0 && !muerto) 
        {  
            Morir();

            if (gameObject.tag.Equals("Player"))
            {
                controlDelSonido.ActivarPista("GameOver");//Sonido 
            }

            else
            {
                controlDelSonido.ActivarPista("Muerte");//Sonido del Enemigo al morir 
            }
        }
    }

    public virtual void AumentarPuntosDeVida(float puntosDeVida) 
    {
        if (jugador.Muerto == false && puntosDeVidaPersonaje < maximosPuntosDeVida)
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

  

    protected void Morir()
    {
        muerto = true;

        if (gameObject.tag.Equals("Player"))
        {
            GameObject.Destroy(gameObject);

            Destroy (Instantiate(efectoAlMorir, transform.position, Quaternion.identity) as GameObject, 0.30f);
        }

        else
        {
            StartCoroutine(EliminarPersonaje());
        }
        //muerto = true;
        //GameObject.Destroy(gameObject);
    }

    IEnumerator EliminarPersonaje()//24.11 Enemigo va hacia el jugador, esquivando obstaculos
    {
        yield return new WaitForSeconds(0.15f);//1.00f
        //muerto = true;
        GameObject.Destroy(gameObject);    
        Destroy (Instantiate(efectoAlMorir, transform.position, Quaternion.identity) as GameObject, 0.30f);
    }

    public virtual void RecibirDisparo(float daño, RaycastHit objetivoGolpeado)
    {
        if (puntosDeVida > 0) 
        {
            RecibirDaño(daño);
        }
    }

    public virtual void RecibirAtaqueDeTrampa(float daño)
    {
        if (puntosDeVida > 0) 
        {
            RecibirDaño(daño);
        }
    }

    public virtual void RecibirAtaqueDeOrbe(float daño)
    {
        if (puntosDeVida > 0) 
        {
            RecibirDaño(daño);
        }
    }

    public virtual void RecibirCuracion(float curacion)
    {    
        if (puntosDeVida > 0)
        {
            if ((puntosDeVida + curacion) <  puntosDeVidaPersonaje)
            {                              
                puntosDeVida += curacion;
            }
            
            else
            {                
                puntosDeVida = puntosDeVidaPersonaje;
            }
        }
    }
  



    /*
    public float sHealth=100;
    protected float health;
    protected bool dead;
    public event System.Action OnDeath;
    protected virtual void Start(){
        health = sHealth;    
    }

    
    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection){
        TakeDamage (damage);
        }
    public virtual void TakeTrapHit(float damage){
        TakeDamage (damage);
    }
    public virtual void TakeDamage(float damage){
        health -=damage;            
            if (health <= 0 && !dead) {             
                Die ();
            FindObjectOfType<AudioManager>().Play("Muerte");
            }
    }
   
    public virtual void TakeHeal(float heal){    
        if (!dead) {
            if ((health + heal) <   scoreKeeper.startingHealth)
             {                               health += heal;
            } else {                
                health =    scoreKeeper.startingHealth;
            }
        }
    }

    public virtual void  IncreaseLife(float Life) {
        if (!dead && scoreKeeper.startingHealth < utility.maxHealth) {
            if ((scoreKeeper.startingHealth+ Life) < utility.maxHealth) 
            {        
                scoreKeeper.startingHealth += Life;
            } else {                
                scoreKeeper.startingHealth = utility.maxHealth;
            }
        } 
       }
    [ContextMenu("Self Destruct")]
    protected void Die(){
        dead = true; 
        if (OnDeath != null) {
            OnDeath ();
        }
        GameObject.Destroy (gameObject);
    }

    */
}






