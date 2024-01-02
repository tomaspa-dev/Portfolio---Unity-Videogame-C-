using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class InterfazDeUsuario : MonoBehaviour
{
    private ControlDelSonido controlDelSonido;
    GeneradorDelNivel generadorDelNivel;
    private Bala bala;
    public static int nivelAmostrar { get ; private set; }
    private int nivel;//12.10 Debido a que el nivel no se puede cambiar
    public static int puntaje { get ; private set; }
    public static int puntosPorNivel { get ; private set; }
    int puntosAcumulados;
    private static int[] puntosPorObjeto = new int[] {5, 10, 20, 30, 40, 50, 0};//{5, 10, 20, 30, 40, 50, 0};
    private static int[] puntosPorEnemigo = new int[] {1, 2, 3, 4, 0};//{1, 2, 3, 4, 0};
     public static bool misionFallida { get ; private set; }//23.10 Declarar Ganador
    public static bool ganoPartida { get ; private set; }//23.10 Declarar Ganador
    public static bool juegoPausado { get ; private set; }//23.10 Evitar Sonidos en Pausa y Pantalla Ganador
    public static int tipoDeMision { get ; private set; }
    public static int enemigosAmatar { get ; private set; }
    public static int objetosDeMisionAConseguir { get ; private set; }    
    public static int tiempoTotal { get ; private set; }
    public static int tiempoRestante { get ; private set; }
    private int valorActual;
    public static int enemigoDestruido { get ; private set; }
    public static int objetoDeMisionConseguido { get ; private set; }
    public static float aumentarDañoDelJugador { get; set; }
    private Jugador jugador;
    private int PUNTOSDEVIDA = 50;
    private int DAÑODELARMA = 5;
    protected float maximoDañoDeArma = 100;//Es una Constante?
    //Interfaz Grafica
    public Image imgFondoGameOver;
    public GameObject interfazDeUsuario;
    public GameObject pantallaGanador;
    public GameObject pantallaGameOver;
    public GameObject pantallaPausa;
    public GameObject pantallaMision;
    public RectTransform cuadroMision;
    public Button btnAumentarVida;
    public Button btnMejorarArma;
    public Text txtFaseCompletada;
    public Text txtEligeOpcion;
    public Text txtPuntuacionAcumulada;
    public Text txtTiempo;
    public Text txtPuntuacion;
    public Text txtPuntos;
    public Text txtBajas;
    public Text txtNivel;
    public Text txtPuntosDeVida;
    public Text txtTiempoRestante;
    public Text txtMision;
    
    public void Awake()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Jugador>();  

        if (jugador!= null)       
        {
            misionFallida = false;
            GenerarTiempoTotal();
            GenerarMisiones(); 
            HabilitarPantallaMision();
            StartCoroutine(DeclararGanador(tiempoTotal));
         } 
    }   

    public void Start()
    {     
        if (ControlDelSonido.instanciaControlDelSonido != null)
        {
            controlDelSonido = GameObject.FindGameObjectWithTag("Sonidos").GetComponent<ControlDelSonido>();
        }

        StartCoroutine(Temporizador(tiempoRestante));
        StartCoroutine(AnimarPantallaMision());
    }

    public void Update()
    {   
        //Debug.Log("El juego se encuentra pausado -" + InterfazDeUsuario.juegoPausado);//23.10 Eliminar  
        //Debug.Log("Se gano la partida del juego -" + InterfazDeUsuario.ganoPartida);//23.10 Eliminar  
        //Debug.Log("El Jugador esta" + jugador.Muerto);//Mostrar Informacion - Saber si el jugador esta muerto  
        txtPuntuacion.text = puntaje.ToString("0 000 000");
        txtPuntos.text = puntosPorNivel.ToString("0 000 000");
        txtBajas.text = enemigoDestruido.ToString("000 000");
        txtNivel.text = nivelAmostrar.ToString("0 000");
        txtPuntosDeVida.text = jugador.PuntosDeVida.ToString("0 000");
        txtTiempoRestante.text = tiempoRestante.ToString("00");   

        if (jugador.Muerto == true)
        {
            Debug.Log("Vuelve a intentarlo, fallaste la mision - " + tipoDeMision + ", de" + tiempoTotal + " segundos");//Mostrar Informacion -
            //Debug.Log("Vuelve a intentarlo, fallaste la mision - " + tipoDeMision + ".Sobrevivir " + tiempoTotal + " segundos");//Mostrar Informacion -
            GameOver();                                         
        } 

        if (puntosPorNivel < 100)
        {
            DeshabilitarBotones();
        }

        verificarJuegoPausado(); 

        /*
        else if (puntosPorNivel >= 1)
        {
            HabilitarBotones();
        }
        */
    } 

    public void verificarJuegoPausado() 
    {
        if (Time.timeScale == 1.0f)
        {   
            juegoPausado = false;        
		}

        else if (Time.timeScale == 0)
        {   
            juegoPausado = true;
		}
    }

    public static void NivelAmostrar() 
    {        
        //nivelAmostrar = 1;//Comentario - El Videojuego inicia en el nivel 1 y va aumentando.
        nivelAmostrar = Utilidades.ObtenerNumeroAleatorio(20, 29);//Comentario - Para pruebas, variar niveles iniciales. 3
    }

    public void AumentarElNivel()
    {          
        nivelAmostrar += 1;
        ReiniciarNivel();
        interfazDeUsuario.SetActive(true);
        SceneManager.LoadScene("Escena Principal");        
        //Primero Aumenta el Nivel, luego muestra la escena
    }

    private void GenerarTiempoTotal()
    {
        if (nivelAmostrar >= 1 && nivelAmostrar < 20)
        {
            tiempoTotal = 30;            
        }
        
        else if (nivelAmostrar >= 20)
        {
            tiempoTotal = 45;
        }

        else
        {
            Debug.Log("Tiempo Inexistente debido a un nivel nulo o erroneo - " + nivelAmostrar);//Comentario - Mostrar Informacion - 
        } 

        tiempoRestante = tiempoTotal;//Comentario - Tiempo Restante que se mostrara en pantalla 
    }   

    public static void ReiniciarNivel()
    {
        ganoPartida = false;
        enemigosAmatar = 0;
        objetosDeMisionAConseguir = 0;
        enemigoDestruido = 0;  
    }

    public void EmpezarNuevoJuego()
    {
        pantallaGameOver.SetActive(false);
        interfazDeUsuario.SetActive(true);
        //Cargar la escena en cada nivel 
        SceneManager.LoadScene("Escena Principal");       
        //SceneManager.LoadScene("Pruebas");//Para Escenario de Pruebas.
    }

    public void CargarGanador()
    {
        controlDelSonido.DesactivarMusicaAleatoria();
        controlDelSonido.ActivarPista("Ganador");
        interfazDeUsuario.SetActive(false);
        pantallaGanador.SetActive(true);
        StartCoroutine(PantallaGanador(10));
        HabilitarBotones();
    }

    public void GameOver()
    {
        ReiniciarNivel();
        misionFallida = true;
        controlDelSonido.DesactivarMusicaAleatoria();
        StartCoroutine(MostrarPantallaGameOver(Color.clear, Color.black, 1));
        interfazDeUsuario.SetActive(false);
        pantallaGameOver.SetActive(true);       
    }

    public void AumentarVida()
    {
        if (puntosPorNivel >= 1)
        {
            //HabilitarBotones();
            jugador.AumentarPuntosDeVida(PUNTOSDEVIDA);
            DisminuirPuntos();
        }       

        /*
        else 
        {
            btnAumentarVida.GetComponent<Button>().interactable = false;
        } 
        */
    }    

    public void MejorarArma()
    {
        if (puntosPorNivel >= 1)
        {
            //HabilitarBotones();
            AumentarDañoDelJugador(DAÑODELARMA);        
            DisminuirPuntos();
        }       

        /*
        else
        {
            btnMejorarArma.GetComponent<Button>().interactable = false;
        }  
        */       
    }

    public void AumentarDañoDelJugador(float aumentarDaño)
    {
        if ((aumentarDañoDelJugador + aumentarDaño) < maximoDañoDeArma)
        {
            aumentarDañoDelJugador += aumentarDaño;
        }            

        else 
        {
            aumentarDañoDelJugador = maximoDañoDeArma;
        }
    }

    public void DisminuirPuntos()
    {
        puntosPorNivel -= 100;
        DeshabilitarBotones();
    }

    public void DeshabilitarBotones()
    {
        btnAumentarVida.GetComponent<Button>().interactable = false;
        btnMejorarArma.GetComponent<Button>().interactable = false;
    }

    public void HabilitarBotones()
    {
        btnAumentarVida.GetComponent<Button>().interactable = true;
        btnMejorarArma.GetComponent<Button>().interactable = true;
    }

    public void HabilitarPantallaMision()
    {
        pantallaMision.SetActive(true);
    }

    public void DeshabilitarPantallaMision()
    {
        pantallaMision.SetActive(false);
    }  

    public void PausarJuego()
    {
        if (Time.timeScale == 1)
        {   
            pantallaPausa.SetActive(true);
			Time.timeScale = 0;            
		}
    }  


    public void ReanudarJuego()
    {
        if (Time.timeScale == 0)
        {   
            pantallaPausa.SetActive(false); 
			Time.timeScale = 1;           
		}
    }

    public void SalirDePantalla()
    {
        pantallaPausa.SetActive(false); 
        Application.Quit();
    }    

    public void IrAlMenuPrincipal()
    {
        pantallaPausa.SetActive(false);
        Time.timeScale = 1;
        ReiniciarNivel();
        puntaje = 0;
        puntosPorNivel = 0;
        aumentarDañoDelJugador = 0;
        controlDelSonido.DesactivarMusicaAleatoria();
        SceneManager.LoadScene("Menu Principal");
    }    

    public void GenerarNivelAleatorio()
    {
        nivelAmostrar = Utilidades.ObtenerNumeroAleatorio(1, 10);//Generar Numero Aleatorio, desde funcion que maneja multihilo.
    }    

    public static void OtorgarPuntos(string tipoDeEnemigo)
    {
        switch (tipoDeEnemigo)
        {
            case "Tipo1":
                puntaje += puntosPorEnemigo[0];
                puntosPorNivel += puntosPorEnemigo[0];
                //Debug.Log("Matar Enemigo Tipo 1 - " + puntosPorEnemigo[0] + " puntos.");//Comentario - Mostrar Informacion puntos ganados por enemigo tipo 1.
                break;

            case "Tipo2":
                puntaje += puntosPorEnemigo[1];
                puntosPorNivel += puntosPorEnemigo[1];
                //Debug.Log("Matar Enemigo Tipo 2 - " + puntosPorEnemigo[1] + " puntos.");//Comentario - Mostrar Informacion puntos ganados por enemigo tipo 1.
                break;

            case "Tipo3":
                puntaje += puntosPorEnemigo[2];
                puntosPorNivel += puntosPorEnemigo[2];
                //Debug.Log("Matar Enemigo Tipo 3- " + puntosPorEnemigo[2] + " puntos.");//Comentario - Mostrar Informacion puntos ganados por enemigo tipo 3.
                break;

            case "Tipo4":
                puntaje += puntosPorEnemigo[3];
                puntosPorNivel += puntosPorEnemigo[3];
                //Debug.Log("Matar Enemigo Tipo 4 - " + puntosPorEnemigo[3] + " puntos.");//Comentario - Mostrar Informacion puntos ganados por enemigo tipo 4.
                break;

            default:
                puntaje += puntosPorEnemigo[4];
                puntosPorNivel += puntosPorEnemigo[4];
                //Debug.Log("Matar No Enemigo - " + puntosPorEnemigo[4] + " puntos.");//Comentario - Mostrar Informacion puntos ganados por enemigo tipo 5.
                break;
        }
        
        Debug.Log("Puntos Acumulados - " + puntaje);//Comentario - Mostrar Informacion puntos acumulados ganados por destruir enemigos.
    }

    public static void OtorgarPuntos()
    {
            switch (nivelAmostrar)
            {
                case int nivel when (nivel > 0 && nivel < 10):
                    puntaje += puntosPorObjeto[0];
                    puntosPorNivel += puntosPorObjeto[0];
                    Debug.Log("Ganaste - " + puntosPorObjeto[0] +" puntos.");//Comentario - Mostrar Informacion puntos ganados por objeto al estar entre el nivel 1 y 9.
                    break;

                case int nivel when (nivel >= 10 && nivel < 20):
                    puntaje += puntosPorObjeto[1];
                    puntosPorNivel += puntosPorObjeto[1];
                    Debug.Log("Ganaste - " + puntosPorObjeto[1] +" puntos.");//Comentario - Mostrar Informacion puntos ganados por objeto al estar entre el nivel 10 y 19.
                    break;

                case int nivel when (nivel >= 20 && nivel < 30):
                    puntaje += puntosPorObjeto[2];
                    puntosPorNivel += puntosPorObjeto[2];
                    Debug.Log("Ganaste - " + puntosPorObjeto[2] +" puntos.");//Comentario - Mostrar Informacion puntos ganados por objeto al estar entre el nivel 20 y 29.
                    break;

                case int nivel when (nivel >= 30 && nivel < 40):
                    puntaje += puntosPorObjeto[3];
                    puntosPorNivel += puntosPorObjeto[3];
                    Debug.Log("Ganaste - " + puntosPorObjeto[3] +" puntos.");//Comentario - Mostrar Informacion puntos ganados por objeto al estar entre el nivel 30 y 39.
                    break;

                case int nivel when (nivel >= 40 && nivel < 50):
                    puntaje +=puntosPorObjeto[4];
                    puntosPorNivel +=puntosPorObjeto[4];
                    Debug.Log("Ganaste - " + puntosPorObjeto[4] +" puntos.");//Comentario - Mostrar Informacion puntos ganados por objeto al estar entre el nivel 40 y 49.
                    break;

                case int nivel when (nivel >= 50):
                    puntaje += puntosPorObjeto[5];
                    puntosPorNivel += puntosPorObjeto[5];
                    Debug.Log("Ganaste - " + puntosPorObjeto[5] +" puntos.");//Comentario - Mostrar Informacion puntos ganados por objeto al estar entre el nivel 50 a más.
                    break;

                default:
                    puntaje += puntosPorObjeto[6];
                    puntosPorNivel += puntosPorObjeto[6];
                    Debug.Log("Ganaste - " + puntosPorObjeto[6] +" puntos.");//Comentario - Mostrar Informacion para nivel desconocido o invalido cero puntos.
                    break;                    
            }
            
            Debug.Log("Puntos Acumulados - " + puntaje);//Comentario - Mostrar Informacion puntos acumulados ganados por objetos.
    }

    public void GenerarMisiones()
    {
        if (nivelAmostrar > 0 && nivelAmostrar < 6)
        {
            tipoDeMision = Utilidades.ObtenerNumeroAleatorio(1, 2);
        }

        else if (nivelAmostrar >= 6 && nivelAmostrar < 10)       
        {
            tipoDeMision = Utilidades.ObtenerNumeroAleatorio(2, 3);
            
            if (tipoDeMision == 2)
            {
                enemigosAmatar = Random.Range (1, 3);   
            }
        }

        else if (nivelAmostrar >= 10 && nivelAmostrar < 20)       
        {
            tipoDeMision = Utilidades.ObtenerNumeroAleatorio(1, 3);

            if (tipoDeMision == 2)
            {
                enemigosAmatar = Random.Range (1, 5);   
            }
        }

        else if (nivelAmostrar >= 20 && nivelAmostrar < 30)       
        {
            tipoDeMision = Utilidades.ObtenerNumeroAleatorio(1, 4);

            if (tipoDeMision == 2)
            {
                enemigosAmatar = Random.Range (2, 6);
            }

            else if (tipoDeMision == 3)
            {
               objetosDeMisionAConseguir = Random.Range (3, 6);  
            }
        }

        else if (nivelAmostrar >= 30 && nivelAmostrar < 40)       
        {
            tipoDeMision = Utilidades.ObtenerNumeroAleatorio(2, 4);

            if (tipoDeMision == 2)
            {
                enemigosAmatar = Random.Range (3, 11);   
            }

            else if (tipoDeMision == 3)
            {
               objetosDeMisionAConseguir = Random.Range (4, 8);  
            }
        }

        else if (nivelAmostrar >= 40 && nivelAmostrar < 50)       
        {
            tipoDeMision = Utilidades.ObtenerNumeroAleatorio(2, 5);

            if (tipoDeMision == 2)
            {
                enemigosAmatar = Random.Range (4, 12);   
            }

            else if (tipoDeMision == 3 || tipoDeMision == 4)
            {
               objetosDeMisionAConseguir = Random.Range (5, 9);  
            }
        }
        
        else
        {
            tipoDeMision = Utilidades.ObtenerNumeroAleatorio(2, 5);//Hasta 4, la mision 5 no existe

            if (tipoDeMision == 2)
            {
                enemigosAmatar = Random.Range (5, 13);   
            }

            else if (tipoDeMision == 3 || tipoDeMision == 4)
            {
               objetosDeMisionAConseguir = Random.Range (6, 12);  
            }
        }

        Debug.Log("El tipo de Mision es: " + tipoDeMision);//Mostrar Informacion - 

        switch (tipoDeMision)
        {
            case 1:
                Debug.Log("Mision 1 - Sobrevivir, "+ tiempoTotal +" segundos");//Mostrar Informacion -
                txtMision.text = "Mision - Sobrevivir, "+ tiempoTotal +" segundos";
                break;

            case 2:
                Debug.Log("Mision 2 - Destruir, "+ enemigosAmatar + " enemigos");//Mostrar Informacion -
                txtMision.text = "Mision - Destruir, "+ enemigosAmatar + " enemigos";
                break;

            case 3:
                Debug.Log("Mision 3 - Robar, " + objetosDeMisionAConseguir +" planos");//Mostrar Informacion -
                txtMision.text = "Mision - Robar, " + objetosDeMisionAConseguir +" planos";
                break;

            case 4:
                Debug.Log("Mision 4 - Conseguir, " + objetosDeMisionAConseguir + "Armas");//Mostrar Informacion -
                txtMision.text = "Mision - Conseguir, " + objetosDeMisionAConseguir + "Armas";
                break;

            case 5:
                Debug.Log("Mision 5 - Hackear X Servidores");//Mostrar Informacion -
                txtMision.text = "Mision - Hackear X Servidores";
                break;
                
            default:
                Debug.Log("Error - Numero de Mision Invalido");//Mostrar Informacion -
                txtMision.text = "Error - Numero de Mision Invalido";
                break;
        }
    }

    IEnumerator Temporizador(int cuentaRegresiva)
    {
        valorActual = cuentaRegresiva; 
        
        while (tiempoRestante > 0)
        {
            Debug.Log("Temporizador, El tiempo Restante es: " + tiempoRestante);//Mostrar Informacion - Saber que valor tiene el tiempo restante
            
            yield return new WaitForSeconds(1);

            tiempoRestante--;
        }       
    }

    public static void CalcularObjetivosDeEnemigos()
    {
        if(enemigosAmatar > 0)
        {            
            enemigosAmatar -= 1;
        }
    }

    public static void CalcularBajaDeEnemigos()
    {     
        enemigoDestruido += 1;
    }

    public static void CalcularObjetivosDeMision()
    {
        if (objetosDeMisionAConseguir > 0)
        {            
            objetosDeMisionAConseguir -= 1;
        }

        objetoDeMisionConseguido += 1;
    } 

    IEnumerator DeclararGanador(float tiempo)
    {
        yield return new WaitForSeconds (tiempo);//Comentario - Si el tiempo trancurrido del nivel finalizo, se verificará el ganador por tipo de misión.
        
        if (jugador.Muerto == false || jugador != null) 
        {             
            switch (tipoDeMision)
            {
                case 1:

                    Debug.Log("Felicidades, pasaste la misión - " + tipoDeMision + ". Sobrevivir " + tiempoTotal + " segundos");//Comentario - Mostrar Informacion -
                    CargarGanador();//13.10 Aumentar el Nivel, si se gano   
                                        
                    break;

                case 2:

                    if (enemigosAmatar == 0)
                    {
                        Debug.Log("Felicidades, pasaste la misión - " + tipoDeMision + ". Destruir enemigos");//Comentario - Mostrar Informacion -
                        CargarGanador();//13.10 Aumentar el Nivel, si se gano
                    }

                    else
                    {
                        Debug.Log("Vuelve a intentarlo, fallaste la misión - " + tipoDeMision + ". Destruir enemigos");//Comentario - Mostrar Informacion -
                        GameOver();
                    }

                    break;
                
                case 3:

                    if (objetosDeMisionAConseguir == 0)
                    {
                        Debug.Log("Felicidades, pasaste la misión - " + tipoDeMision + ". Robar X planos");//Comentario - Mostrar Informacion -
                        CargarGanador();//13.10 Aumentar el Nivel, si se gano
                    }

                    else
                    {
                        Debug.Log("Vuelve a intentarlo, fallaste la misión - " + tipoDeMision + ". Robar X planos");//Comentario - Mostrar Informacion -
                        GameOver();
                    }            

                    break;

                case 4:

                    if (objetosDeMisionAConseguir == 0)
                    {
                        Debug.Log("Felicidades, pasaste la misión - " + tipoDeMision + ". Conseguir X Armas");//Comentario - Mostrar Informacion -
                        CargarGanador();//13.10 Aumentar el Nivel, si se gano
                    }

                    else
                    {
                        Debug.Log("Vuelve a intentarlo, fallaste la misión - " + tipoDeMision + ". Conseguir X Armas");//Comentario - Mostrar Informacion -
                        GameOver();
                    }    

                    break;

                default:

                    Debug.Log("Error - Numero de misión inválida");//Comentario - Mostrar Informacion -
                    break;
            }  
        }      
    }

    IEnumerator MostrarPantallaGameOver(Color desde, Color para, float tiempo)
    {
        float velocidad = 1 / tiempo;
        float porcentaje = 0;

        while(porcentaje < 1)
        {
            porcentaje += Time.deltaTime * velocidad; //
            imgFondoGameOver.color = Color.Lerp(desde, para, porcentaje);
            yield return null;
        }        
    }

    IEnumerator AnimarPantallaMision()
    {
        float tiempoDeRetardo = 2.5f;
        float velocidad = 3f;
        float porcentajeDeAnimacion = 0;
        int direccion = 1;     

        float finTiempoDeRetardo = Time.time + 1 / velocidad + tiempoDeRetardo;

        while (porcentajeDeAnimacion >= 0)
        {
            porcentajeDeAnimacion += Time.deltaTime * velocidad * direccion;

            if (porcentajeDeAnimacion >= 1)
            {
                porcentajeDeAnimacion = 1;

                if (Time.time > finTiempoDeRetardo)
                {
                    direccion = -1;
                }
            }
            
            cuadroMision.anchoredPosition = Vector2.up * Mathf.Lerp(-500, 8, porcentajeDeAnimacion);  
            yield return null;       
        }     
        DeshabilitarPantallaMision();
    }

    IEnumerator PantallaGanador(float tiempo)
    {   
        while (tiempo >= 0)
        {
            ganoPartida = true;
            txtFaseCompletada.text = "Fase " + (nivelAmostrar) + " Completada";
            txtPuntuacionAcumulada.text = "Puntos Acumulados: " + puntosPorNivel;
            txtTiempo.text = "Continuar en: " + tiempo.ToString("00");
            Debug.Log("Continuar, en: " + tiempo);//Mostrar Informacion - Saber que valor tomo el tiempo restante
            
            if (tiempo == 0)
            {
                AumentarElNivel();            
            }  

            yield return new WaitForSeconds(1);
            tiempo--;          
        }             
    }
}
