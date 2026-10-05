using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;

namespace RestauranteApp.Idiomas
{
    public class LanguageManager
    {
        private const string SESSION_IDIOMA = "IdiomaActual";
        private const string ITEM_MANAGER = "LanguageManager";

        private readonly List<ILanguageObserver> observadores;
        private Dictionary<string, string> traducciones;

        public string IdiomaActual { get; private set; }

        private LanguageManager()
        {
            observadores = new List<ILanguageObserver>();
            IdiomaActual = ObtenerIdiomaDeSesion();
            CargarIdioma();
        }

        public static LanguageManager ObtenerInstancia()
        {
            if (HttpContext.Current.Items[ITEM_MANAGER] == null)
            {
                HttpContext.Current.Items[ITEM_MANAGER] = new LanguageManager();
            }

            return (LanguageManager)HttpContext.Current.Items[ITEM_MANAGER];
        }

        public void RegistrarObservador(ILanguageObserver observador)
        {
            if (observador != null && !observadores.Contains(observador))
            {
                observadores.Add(observador);
            }
        }

        public void EliminarObservador(ILanguageObserver observador)
        {
            if (observador != null)
            {
                observadores.Remove(observador);
            }
        }

        public void CambiarIdioma(string nuevoIdioma)
        {
            if (string.IsNullOrWhiteSpace(nuevoIdioma))
                return;

            if (nuevoIdioma != "es" && nuevoIdioma != "en")
                return;

            if (IdiomaActual == nuevoIdioma)
                return;

            IdiomaActual = nuevoIdioma;

            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[SESSION_IDIOMA] = nuevoIdioma;
            }

            CargarIdioma();
            NotificarObservadores();
        }

        public string ObtenerTexto(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                return clave;

            if (traducciones.ContainsKey(clave))
                return traducciones[clave];

            return clave;
        }

        private string ObtenerIdiomaDeSesion()
        {
            if (HttpContext.Current.Session != null)
            {
                object idioma = HttpContext.Current.Session[SESSION_IDIOMA];

                if (idioma != null)
                    return idioma.ToString();
            }

            return "es";
        }

        private void CargarIdioma()
        {
            string nombreArchivo = IdiomaActual + ".json";

            string ruta = HttpContext.Current.Server.MapPath(
                "~/App_Data/Idiomas/" + nombreArchivo
            );

            if (!File.Exists(ruta))
            {
                traducciones = new Dictionary<string, string>();
                return;
            }

            string json = File.ReadAllText(ruta);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            traducciones = serializer.Deserialize<Dictionary<string, string>>(json);

            if (traducciones == null)
            {
                traducciones = new Dictionary<string, string>();
            }
        }

        private void NotificarObservadores()
        {
            foreach (ILanguageObserver observador in observadores)
            {
                observador.ActualizarIdioma();
            }
        }
    }
}