using UnityEngine;

namespace VNCreator
{
    /// <summary>
    /// Sistema de estadisticas del jugador.
    /// Se crea solo en la PlayScene y resetea al iniciar nueva partida.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        // Instancia global (Singleton)
        public static PlayerStats instance;

        [Header("Estadisticas del Jugador")]
        public int aptitud = 0;
        public int miedo   = 0;

        void Awake()
        {
            // Si ya existe una instancia en escena, destruimos este duplicado
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        // --- Aptitud ---
        public void AumentarAptitud(int cantidad)
        {
            aptitud += cantidad;
            Debug.Log("[PlayerStats] Aptitud +" + cantidad + " -> Total: " + aptitud);
        }

        public void DisminuirAptitud(int cantidad)
        {
            aptitud -= cantidad;
            Debug.Log("[PlayerStats] Aptitud -" + cantidad + " -> Total: " + aptitud);
        }

        // --- Miedo ---
        public void AumentarMiedo(int cantidad)
        {
            miedo += cantidad;
            Debug.Log("[PlayerStats] Miedo +" + cantidad + " -> Total: " + miedo);
        }

        public void DisminuirMiedo(int cantidad)
        {
            miedo -= cantidad;
            Debug.Log("[PlayerStats] Miedo -" + cantidad + " -> Total: " + miedo);
        }

        // --- Reiniciar stats (New Game) ---
        public void Reiniciar()
        {
            aptitud = 0;
            miedo   = 0;
            Debug.Log("[PlayerStats] Estadisticas reiniciadas.");
        }
    }
}
