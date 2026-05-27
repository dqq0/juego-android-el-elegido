using UnityEngine;

namespace VNCreator
{
    /// <summary>
    /// Sistema de estadísticas del jugador.
    /// Persiste entre escenas gracias a DontDestroyOnLoad.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        // Instancia global (Singleton) para acceder desde cualquier script
        public static PlayerStats instance;

        [Header("Estadísticas del Jugador")]
        public int aptitud = 0;
        public int miedo   = 0;

        void Awake()
        {
            // Si ya existe una instancia, destruimos este duplicado
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject); // Sobrevive al cambiar de escena
        }

        // ─── Métodos para modificar aptitud ─────────────────────────────────
        public void AumentarAptitud(int cantidad)
        {
            aptitud += cantidad;
            Debug.Log($"[PlayerStats] Aptitud +{cantidad} → Total: {aptitud}");
        }

        public void DisminuirAptitud(int cantidad)
        {
            aptitud -= cantidad;
            Debug.Log($"[PlayerStats] Aptitud -{cantidad} → Total: {aptitud}");
        }

        // ─── Métodos para modificar miedo ────────────────────────────────────
        public void AumentarMiedo(int cantidad)
        {
            miedo += cantidad;
            Debug.Log($"[PlayerStats] Miedo +{cantidad} → Total: {miedo}");
        }

        public void DisminuirMiedo(int cantidad)
        {
            miedo -= cantidad;
            Debug.Log($"[PlayerStats] Miedo -{cantidad} → Total: {miedo}");
        }

        // ─── Reiniciar stats (para New Game) ────────────────────────────────
        public void Reiniciar()
        {
            aptitud = 0;
            miedo   = 0;
            Debug.Log("[PlayerStats] Estadísticas reiniciadas.");
        }
    }
}
