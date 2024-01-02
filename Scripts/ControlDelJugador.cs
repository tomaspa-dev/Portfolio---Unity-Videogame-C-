using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(ControlDelArma))]
public class ControlDelJugador : MonoBehaviour
{     
    public enum Estado {Detenerse, Moverse}
    Estado estadoActual;
    public Joystick joystick;
    public Animator animator;//15.03 Animacion de Movimiento del Jugador
    GameObject[] objetivoDelJugador;
    private enum Dispositivo {Teclado, Tactil}
    private float ejeHorizontal;
    private float ejeVertical;
    Vector3 direccion;
    private float anguloDeGiro;
    private float anguloDeGiroGradual;
    private float tiempoAlGirar = 0.25f;//0.1f;
    private float velocidadAlGirar = 0.1f;//0.1f;
    private Dispositivo dispositivo = Dispositivo.Teclado;//Activar el Input por Teclado;      
    //private Dispositivo dispositivo = Dispositivo.Tactil;
    public CharacterController controlJugador;
    private float velocidadDelJugador = 3.45f;//3.65f;
    Vector3 velocidadDeCaida;
    private float gravedad = -15.81f;
    public Transform verificarSuelo;
    private float radioEsfera = 0.15f;//0.09f
    private bool estaEnTierra;
    private bool verificarPresenciaDeEnemigos;
    //private bool seMueve;
    private bool seDetiene;
    private bool seAvanza;
    private bool seRetrocede;
    private bool conAutoapuntado;
    public LayerMask suelo;
    ControlDelArma controlDelArma;
    Arma arma;

    public GameObject posicionParaApuntar;
    //bool seEncontroObjetivo;//esPunteriaAsistida
    float distanciaDeDisparo;
    float rangoDeVision = 17.5f;
    private GameObject objetivoActual;

    GameObject[] objetivosDelJugador;

    private bool activarPunteriaAsistida;

    void Awake()
    {   
        controlDelArma = GetComponent<ControlDelArma>();
        //objetivoDelJugador = GameObject.FindGameObjectsWithTag("Tipo1");   
        //objetivosDelJugador = FindGameObjectsWithTags(new string[] {"Tipo1", "Tipo2", "Tipo3", "Tipo4"}); //21.12 No funca, usar con corutina
        GetComponent<BoxCollider>();   
        GetComponent<Rigidbody>();
        HabilitarVerificacionDeEnemigos();
        conAutoapuntado = true;
    } 

    void Start()
    {
        
    }

    void Update()
    {       
        ActualizarAnimacionDetenerse();   
        ActualizarAutoapuntado();
        
        //Debug.Log("Vector Dirección Magnitude" + direccion.magnitude);//21.03    
        Debug.Log("Se Mueve - " + direccion.magnitude + "; se Avanza - "+seAvanza + ", seDetiene - "+seDetiene);//21.03 
        Debug.Log("Estado" + estadoActual +"EjeH" + ejeHorizontal + "EjeV" + ejeVertical);//21.03      

        AgregarDispositivo();
        direccion = new Vector3(ejeHorizontal, 0f, ejeVertical).normalized;
        verificarAltura(); 

        switch (estadoActual)
        {
            case Estado.Detenerse:
                Detenerse();
                break;
            
            case Estado.Moverse:
                Moverse();
                break;

            default:
                Debug.Log("No se encuentra un estado activo, o su valor es erróneo");
                break;
        }

        if (estaEnTierra && velocidadDeCaida.y < 0)
        {          
            velocidadDeCaida.y = -3;

            if (direccion.magnitude >= 0.1f ) // (Mathf.Abs(ejeHorizontal) > 0 || Mathf.Abs(ejeVertical) > 0)
            {
                estadoActual = Estado.Moverse;
                
                if (ejeHorizontal < 0 || ejeVertical < 0)
                {
                    Retroceder();
                }

                else if (ejeHorizontal > 0 || ejeVertical > 0)
                {
                    Avanzar();                                     
                } 

                else
                {
                    Debug.Log("Valores incorrectos en los ejes");
                }
            } 

            else
            {             
                estadoActual = Estado.Detenerse;
            } 

            ActualizarAnimacionCaminar(); 
            ActualizarAnimacionRetroceder();                                         
        }            

        else
        {                         
            AcelerarCaidaDelJugador();
        }   

        velocidadDeCaida.y += gravedad * Time.deltaTime;    
    }

    void LateUpdate()
    {
        BuscarEnemigoCercano();//22.03 Utilizar en Pruebas

        /*
        if (Menu.PunteriaAsistida && verificarPresenciaDeEnemigos == true)//&& objetivoDelJugador != null
        {
           BuscarEnemigoCercano();
        }

        if (Menu.PunteriaAsistida == true)//&& objetivoDelJugador != null
        {
           BuscarEnemigoCercano();
        }
        */
    }
   
    public void Detenerse()
    {
        seRetrocede = false;
        seAvanza = false;
        seDetiene = true;     
        //ActualizarAnimacionDetenerse();
        //seDetiene = true;
        //Al no llamar al método el jugador no se mueve.
        //La animación que representa detenerse, esta activada por defecto en el inspector.
    }

    public void Moverse()
    {
        seDetiene = false;
        MoverJugadorEnTierra();
        ActualizarAnimacionMoverse();   
        //Debug.LogError("No se muestra la animación, por valor erróneo de la punteria asistida");     
    }

    public void Avanzar()
    {
        seRetrocede = false;
        seAvanza = true;   
    }

    public void Retroceder()
    {
        seRetrocede = true;
        seAvanza = false;

    }

    void AgregarDispositivo()
    {
        switch (dispositivo)
        {              
            case Dispositivo.Tactil:
                ejeHorizontal = joystick.Horizontal;
                ejeVertical = joystick.Vertical;  
                break;

            case Dispositivo.Teclado:
                ejeHorizontal = Input.GetAxisRaw("Horizontal");
                ejeVertical = Input.GetAxisRaw("Vertical");   
                break;

            default:
                Debug.LogError("No se reconoce el Dispositivo");//Mostrar Informacion -
                break;
        }
    }

    void verificarAltura()
    {
        estaEnTierra = Physics.CheckSphere(verificarSuelo.position, radioEsfera, suelo); 
        Debug.Log("El Jugador/Player esta en tierra - "+ estaEnTierra);//02.01     
    }

    void HabilitarVerificacionDeEnemigos()
    {
        verificarPresenciaDeEnemigos = true;   
    }

    void DeshabilitarVerificacionDeEnemigos()
    {
        verificarPresenciaDeEnemigos = false;   
    }

    void AcelerarCaidaDelJugador()
    {
        controlJugador.Move(velocidadDeCaida * Time.deltaTime);
    }

    GameObject[] FindGameObjectsWithTags(params string[] tags)
    {
        var todosLosTags = new List<GameObject>();

        foreach (string tag in tags)
        {
            todosLosTags.AddRange(GameObject.FindGameObjectsWithTag(tag).ToList());
        }

        return todosLosTags.ToArray();
    }

    void MoverJugadorEnTierra()
    {
        anguloDeGiro = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg;
        anguloDeGiroGradual = Mathf.SmoothDampAngle(transform.eulerAngles.y, anguloDeGiro, ref velocidadAlGirar, tiempoAlGirar);
        transform.rotation = Quaternion.Euler(0f, anguloDeGiroGradual, 0f);
        controlJugador.Move(direccion * velocidadDelJugador * Time.deltaTime);
    } 

    public void Apuntar(Transform objetivo)
    {
        Vector3 objetivoEnAlturaCorrecta = new Vector3 (objetivo.position.x, transform.position.y, objetivo.position.z);
        transform.LookAt(objetivoEnAlturaCorrecta);
    }

    //28.11 Buscar enemigo mas cercano
    void BuscarEnemigoCercano()
    {
        objetivosDelJugador = FindGameObjectsWithTags(new string[] {"Tipo1", "Tipo2", "Tipo3", "Tipo4"});//22.03 Debido a esto se fija en el Arma y no en el Enemigo.

        GameObject enemigoCercano = null;
        float distancia = Mathf.Infinity;
        Vector3 posicion = transform.position;

        foreach (GameObject enemigoPotencial in objetivosDelJugador)
        {
            Vector3 diff = enemigoPotencial.transform.position - posicion;
            //float distanciaActual = diff.sqrMagnitude;
            float distanciaActual = diff.sqrMagnitude;

            if (distanciaActual < distancia && distanciaActual >= 1)
            {
                enemigoCercano = enemigoPotencial;
                distancia = distanciaActual;
            }   
        }

        if (enemigoCercano != null)
        {
            //Debug.DrawLine(this.transform.position, enemigoCercano.transform.position, Color.red);//this.posicionParaApuntar
            Debug.DrawLine(this.transform.position, enemigoCercano.transform.position, Color.red);
            Apuntar(enemigoCercano.transform);
        }        
    }

    void ActualizarAnimacionMoverse()
    {
        animator.SetFloat("seMueve", (Mathf.Abs(ejeHorizontal) + Mathf.Abs(ejeVertical)));//15.03 - Permite la animación segun el valor del movimiento
    }

    void ActualizarAnimacionCaminar()
    {
        animator.SetBool("seAvanza", seAvanza);
    }

    void ActualizarAnimacionRetroceder()
    {
        //animator.SetFloat("seRetrocede", (Mathf.Abs(ejeHorizontal) + Mathf.Abs(ejeVertical)));//15.03 - Permite la animación segun el valor del movimiento
        animator.SetBool("seRetrocede", seRetrocede);
    }

    void ActualizarAnimacionDetenerse()
    {
         animator.SetBool("seDetiene", seDetiene);
    }

    void ActualizarAutoapuntado()
    {
        animator.SetBool("conAutoapuntado", conAutoapuntado);
    }


    /*

    public void ApuntarAutomaticamente()
    {
        //transform.rotation = Quaternion.Euler(0f, anguloDeGiroGradual, 0f);
        if (objetivoActual != null)//objetivoActual.layer != LayerMask.NameToLayer("Obstaculo")
        {
            transform.LookAt(objetivoActual.transform);
        }        
    }

    void VerificarObjetivo()//28.11 Podria cambiar al utilizar buscar el enemigo mas cercano
    {
        Ray rayo = new Ray(posicionParaApuntar.transform.position, transform.forward);
        RaycastHit objetivoGolpeado;
        //LayerMask mascara;

        if (Physics.Raycast(rayo, out objetivoGolpeado, rangoDeVision, LayerMask.GetMask("Enemigo")))
        {
            Debug.Log(objetivoGolpeado.collider.gameObject.layer);
            distanciaDeDisparo = Vector3.Distance(posicionParaApuntar.transform.position, objetivoGolpeado.transform.position);
            //seEncontroObjetivo = true;
            objetivoActual = objetivoGolpeado.collider.gameObject;
            Debug.DrawLine(rayo.origin, objetivoGolpeado.point, Color.red);
            Debug.Log("Actual - " + objetivoGolpeado.collider.gameObject.layer);
        }

        else if (Physics.Raycast(rayo, out objetivoGolpeado, rangoDeVision, LayerMask.GetMask("Obstaculo")))
        {
            Debug.Log(objetivoGolpeado.collider.gameObject.layer);
            distanciaDeDisparo = Vector3.Distance(posicionParaApuntar.transform.position, objetivoGolpeado.transform.position);
            //seEncontroObjetivo = false;
            objetivoActual = null;
            Debug.DrawLine(rayo.origin, objetivoGolpeado.point, Color.yellow);
            Debug.Log("Actual - " + objetivoGolpeado.collider.gameObject.layer);
            //Debug.DrawLine(rayo.origin, rayo.origin + rayo.direction * 10, Color.yellow);
        }

        else
        {
            //seEncontroObjetivo = false;
            objetivoActual = null;
            Debug.DrawLine(rayo.origin, rayo.origin + rayo.direction * 10, Color.green);
        }
    }

    */

    /*
    //objetivoDelJugador.layer != LayerMask.NameToLayer("Enemigo"));

    void LateUpdate()//28.11 Podria eliminarse al buscar el enemigo mas cercano
    {
        if (Menu.PunteriaAsistida) 
        {
            if (seEncontroObjetivo == true)
            {
                ApuntarAutomaticamente();
            }
        }
    }
    */
}

/*
 //28.11 Buscar enemigo mas cercano
    void FindGetClosestEnemy()
    {
        float closestDistanceSqr = Mathf.Infinity;//Mathf.Infinity;
        Enemigo bestTarget = null;
        Enemigo[] allEnemies = GameObject.FindObjectsOfType<Enemigo>();
        
        Vector3 currentPosition = transform.position;

        foreach(Enemigo potentialTarget in allEnemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        Debug.DrawLine(this.transform.position, bestTarget.transform.position, Color.red);
        Apuntar(bestTarget.transform);
    } 
*/

/* 01.11 //Al exportar comentar, para disparar solo con boton
        if (Input.GetMouseButton(0))//Disparar con Mouse       
        {
            controlDelArma.AlPresionarGatillo();
        }

        if (Input.GetMouseButtonUp(0))  
        {
            controlDelArma.AlSoltarGatillo();
        }
*/
