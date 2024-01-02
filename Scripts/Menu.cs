using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Menu : MonoBehaviour
{
    private ControlDelSonido controlDelSonido;
    private InterfazDeUsuario interfazDeUsuario;
    public GameObject menuPrincipal;
    public GameObject opcionCreditos;
    public GameObject opciones;
    public GameObject pantallaDeCarga;
    public Slider barraDeProgreso;
    public Slider[] sliderCambiarVolumen;
    public Text txtPorcentajeAvance;
    public Text txtConsejos;
    public Toggle tgPunteriaAsistida;
    public string consejosRecomendados;   
    public CanvasGroup canvasConsejos;
    private int cuentaConsejos;
    
    [SerializeField] public AudioMixerGroup masterAudioMixer;
    [SerializeField] public AudioMixerGroup musicaAudioMixer;
    [SerializeField] public AudioMixerGroup efectosAudioMixer;

    public static float volumenGeneral { get ; private set; }
    public static float volumenDeLosEfectos { get ; private set; }
    public static float volumenMusical { get ; private set; }

    float audioMixerVolumenGeneral; 
    float audioMixerVolumenDeLosEfectos; 
    float audioMixerVolumenMusical;

    private float volumenGeneralporDefecto = 1f;
    private float volumenDelosEfectosporDefecto = 1f;
    private float volumenMusicalporDefecto = 0.5f;
    
    private bool punteriaAsistidaporDefecto = false;
    private static bool punteriaAsistida;

    public static bool PunteriaAsistida 
    { 
        get => punteriaAsistida; 
        private set => punteriaAsistida = value; 
    }

    public enum Escenas 
    {
        Menu = 0,
        Escena1 = 1
    }

    void Awake()
    {
        AbrirPreferenciasPunteriaAsistida();

        controlDelSonido = GameObject.FindGameObjectWithTag("Sonidos").GetComponent<ControlDelSonido>();//Comentario - importante, permite el sonido.
         
        if (ControlDelSonido.instanciaControlDelSonido != null)
        {            
            controlDelSonido.ActivarPistaMusical("MenuPrincipal");
        }
    }

    void Start()
    {
        AbrirPreferenciasDelVolumen();//Comentario - Muy importante, el audiomixer no abre valores guardados del método Awake, no cambiarlo.
    }     

    public void Jugar()
    {  
        if (ControlDelSonido.instanciaControlDelSonido != null)
        {  
            HabilitarSonidoJugar();  
            controlDelSonido.DesactivarPistaMusical("MenuPrincipal"); 
        }

        CargarEscena (); //Comentario - Abrir la pantalla de carga antes de activar la escena principal
        InterfazDeUsuario.NivelAmostrar();//Comentario - Generar el Primer nivel desde la Interfaz de usuario.       
    }

    public void MostrarOpciones()
    {
        HabilitarSonidoDeBoton();
        menuPrincipal.SetActive(false);
        opciones.SetActive(true);
    }

    public void Regresar()
    {
        HabilitarSonidoDeBoton();
        GuardarPreferenciasDelVolumen();
        menuPrincipal.SetActive(true);
    }

    public void MostarMenuPrincipal()
    { 
        HabilitarSonidoDeBoton();       
        opciones.SetActive(false);
        menuPrincipal.SetActive(true);
    }

    public void MostrarCreditos()
    {
        HabilitarSonidoDeBoton();
        menuPrincipal.SetActive(false);
        opcionCreditos.SetActive(true);
    }

    public void Personalizar()
    {
        HabilitarSonidoDeBoton();
        Debug.Log("Muy Pronto...");
    }    

    public void Salir()
    {
        HabilitarSonidoDeBoton();
        Debug.Log("Cerrando Aplicacion");
        Application.Quit();
    }   

    public void HabilitarSonidoDeBoton()
    {
        controlDelSonido.ActivarPista("Boton");
    }

    public void HabilitarSonidoJugar()
    {
        controlDelSonido.ActivarPista("Jugar");
    }

    void CargarEscena ()
    {
        menuPrincipal.SetActive(false);
        pantallaDeCarga.SetActive(true);
        StartCoroutine(this.MostrarConsejos());
        StartCoroutine(this.MostrarPantallaDeCarga("Escena Principal"));        
    }

    IEnumerator MostrarConsejos()
    {     
        cuentaConsejos = Utilidades.ObtenerNumeroAleatorio(0, 6);
        txtConsejos.text = ElegirConsejo(cuentaConsejos);

        while (pantallaDeCarga.activeInHierarchy)
        {
            yield return new WaitForSeconds (2f);

            canvasConsejos.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1f, 1);

            yield return new WaitForSeconds (2f);

            cuentaConsejos++;
            
            if (cuentaConsejos >= 6)
            {
                cuentaConsejos = 0;
            }

            txtConsejos.text = ElegirConsejo(cuentaConsejos);
            canvasConsejos.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1, 0f, 1);
        }             
    }

    IEnumerator MostrarPantallaDeCarga(string nombreDelNivel)
    {        
        //yield return new WaitForSeconds (6f); //Comentario - Pruebas Solo para ver los comentarios en la pantalla, porque aparecen muy rapido.

        AsyncOperation operacion = SceneManager.LoadSceneAsync((int)Escenas.Escena1, LoadSceneMode.Single);

        while (!operacion.isDone)
        {
            float progreso = Mathf.Clamp01(operacion.progress /.9f);
            barraDeProgreso.value = progreso;
            txtPorcentajeAvance.text = string.Format("Generando Mapa Aleatorio : {0}%", progreso * 100f);
            //Debug.Log("Progreso de carga - " + progreso);
            yield return null;
        }         
    }

    public void AbrirValoresDelSliderVolumenGeneral()
    {
        sliderCambiarVolumen[0].value = PlayerPrefs.GetFloat("MasterVolume", volumenGeneralporDefecto);
    }

    public void AbrirValoresDelSliderVolumenEfectos()
    {
        sliderCambiarVolumen[1].value = PlayerPrefs.GetFloat("EfectosVolume", volumenDelosEfectosporDefecto);
    }

    public void AbrirValoresDelSliderVolumenMusical()
    {
        sliderCambiarVolumen[2].value = PlayerPrefs.GetFloat("MusicVolume", volumenMusicalporDefecto);
    }

    public void GuardarValoresDelSliderVolumenGeneral()
    {
        sliderCambiarVolumen[0].value = volumenGeneral;
    }

    public void GuardarValoresDelSliderVolumenEfectos()
    {
        sliderCambiarVolumen[1].value = volumenDeLosEfectos;
    }

    public void GuardarValoresDelSliderVolumenMusical()
    {
        sliderCambiarVolumen[2].value = volumenMusical;
    }

    public void GuardarPreferenciasVolumenGeneral(float value)
    {
        volumenGeneral = value;
        audioMixerVolumenGeneral = Mathf.Log10(volumenGeneral) * 20;
        masterAudioMixer.audioMixer.SetFloat("AudioMixerMasterVolume", audioMixerVolumenGeneral);
        GuardarValoresDelSliderVolumenGeneral();
        GuardarPreferenciasDelVolumenGeneral();   
    }

    public void GuardarPreferenciasVolumenDeLosEfectos(float value)
    {
        volumenDeLosEfectos = value;
        efectosAudioMixer.audioMixer.SetFloat("AudioMixerEfectosVolume", Mathf.Log10(volumenDeLosEfectos) * 20);
        GuardarValoresDelSliderVolumenEfectos();
        GuardarPreferenciasDelVolumenDeEfectos();
    }

     public void GuardarPreferenciasVolumenMusical(float value)
    {
        volumenMusical = value;
        musicaAudioMixer.audioMixer.SetFloat("AudioMixerMusicVolume", Mathf.Log10(volumenMusical) * 20);
        GuardarValoresDelSliderVolumenMusical();
        GuardarPreferenciasDelVolumenMusical();
    }

    public void AbrirPreferenciasDelVolumen()
    {
        masterAudioMixer.audioMixer.GetFloat("AudioMixerMasterVolume", out audioMixerVolumenGeneral);
        efectosAudioMixer.audioMixer.GetFloat("AudioMixerEfectosVolume", out audioMixerVolumenDeLosEfectos);
        musicaAudioMixer.audioMixer.GetFloat("AudioMixerMusicVolume", out audioMixerVolumenMusical);

        AbrirValoresDelSliderVolumenGeneral();
        AbrirValoresDelSliderVolumenEfectos();
        AbrirValoresDelSliderVolumenMusical();
    }

    public void AbrirPreferenciasPunteriaAsistida()
    {
        punteriaAsistida = ((PlayerPrefs.GetInt("PunteriaAsistida", (punteriaAsistidaporDefecto ? 1 : 0)) == 1 ? true : false));
        tgPunteriaAsistida.isOn = punteriaAsistida;
    }

    public void GuardarPreferenciasDelVolumenGeneral()
    {
        PlayerPrefs.SetFloat("MasterVolume", volumenGeneral);
    }

    public void GuardarPreferenciasDelVolumenDeEfectos()
    {
        PlayerPrefs.SetFloat("EfectosVolume", volumenDeLosEfectos);
    }

    public void GuardarPreferenciasDelVolumenMusical()
    {
        PlayerPrefs.SetFloat("MusicVolume", volumenMusical);
    }

    public void GuardarPreferenciasPunteriaAsistida(bool value)
    {
        punteriaAsistida = value;
        PlayerPrefs.SetInt("PunteriaAsistida", (value ? 1 : 0));
        Debug.Log("Punteria Asistida" + punteriaAsistida);
    }

    public void GuardarPreferenciasDelVolumen()
    {
        PlayerPrefs.Save();
    }

    public void MostrarInformacion()
    {
        Debug.Log("Valor del Volumen General " + volumenGeneral);
        Debug.Log("SliderVolumenGeneral " + sliderCambiarVolumen[0].value); 
        Debug.Log("AudioMixerGeneral " + audioMixerVolumenGeneral); 
    }

    public string ElegirConsejo(int semilla)
    {
        switch (semilla)
        {
            case 0:
                consejosRecomendados ="Puedes Acumular 100 Puntos y Obtener una Mejor Arma o Aumentar tus Puntos de Vida, tu decides.";
                break;
            
            case 1:
                consejosRecomendados = "Al matar Enemigos ganas puntos, que puedes canjear por un arma o fortalecer tus puntos de vida, tu decides.";
                break;

            case 2:
                consejosRecomendados = "Existen objetos que otorgan puntos y aparecen cada cierto tiempo en posiciones aleatorias.";
                break;
                
            case 3:
                consejosRecomendados = "Debes cumplir misiones en un determinado tiempo, si fallas perderas y moriras.";
                break;

            case 4:
                consejosRecomendados = "Los enemigos, sus posiciones, y ataques cambian en cada nivel.";
                break;

            case 5:
                consejosRecomendados = "Cada nivel es diferente, desde el Mapa hasta los enemigos.";
                break;

            default:
                consejosRecomendados = "Error - Consejo no disponible. Valor por defecto";
                break;
        }
 
        return consejosRecomendados;
    }
}
