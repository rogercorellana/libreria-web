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
        private const string COOKIE_IDIOMA = "Idioma";

        private readonly List<ILanguageObserver> observadores;
        private Dictionary<string, string> traducciones;

        public string IdiomaActual { get; private set; }

        private LanguageManager()
        {
            observadores = new List<ILanguageObserver>();
            IdiomaActual = ObtenerIdiomaGuardado();
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

            if (!EsIdiomaValido(nuevoIdioma))
                return;

            if (IdiomaActual == nuevoIdioma)
                return;

            IdiomaActual = nuevoIdioma;

            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[SESSION_IDIOMA] = nuevoIdioma;
            }

            // La cookie sobrevive al cierre de sesión (Session.Abandon),
            // así el Login se muestra en el último idioma elegido
            HttpCookie cookie = new HttpCookie(COOKIE_IDIOMA, nuevoIdioma);
            cookie.Expires = DateTime.Now.AddYears(1);
            cookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Set(cookie);

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

        private string ObtenerIdiomaGuardado()
        {
            // 1) Sesión actual
            if (HttpContext.Current.Session != null)
            {
                object idioma = HttpContext.Current.Session[SESSION_IDIOMA];

                if (idioma != null && EsIdiomaValido(idioma.ToString()))
                    return idioma.ToString();
            }

            // 2) Cookie persistente (por ejemplo, después de cerrar sesión)
            HttpCookie cookie = HttpContext.Current.Request.Cookies[COOKIE_IDIOMA];

            if (cookie != null && EsIdiomaValido(cookie.Value))
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SESSION_IDIOMA] = cookie.Value;
                }

                return cookie.Value;
            }

            // 3) Idioma por defecto
            return "es";
        }

        private static bool EsIdiomaValido(string idioma)
        {
            return idioma == "es" || idioma == "en";
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