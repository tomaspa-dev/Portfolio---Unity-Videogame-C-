using UnityEngine;
public interface IDaño {
    void RecibirDisparo(float daño, RaycastHit objetivoGolpeado);
    void RecibirDaño(float daño);
    void RecibirAtaqueDeTrampa(float daño);
    void RecibirAtaqueDeOrbe(float daño);
    void RecibirCuracion(float curacion); 
    void AumentarPuntosDeVida(float puntosDeVida);
}
