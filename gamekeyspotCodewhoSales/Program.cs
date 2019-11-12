using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Linq;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Capas;
using System.Runtime.CompilerServices;

namespace gamekeyspot
{
    public class MainClass
    {
        //string cnn = "Data Source=82.223.108.84;Initial Catalog=gamekeyspotdb; User ID=sa;Password=Bp9Ea51VGI; Packet Size=4096" providerName="System.Data.SqlClient";
        //const string strcnn = @"Data Source=82.223.108.84;Initial Catalog=gamekeyspotdb; User ID=sa;Password=Bp9Ea51VGI; Packet Size=4096";
        //const string strcnn = @"Data Source=localhost,1433;Initial Catalog=gamekeyspotdb;User ID=sa;Password=Carloselias23.;Packet Size=4096";

        const string client_id = "1051f676d039d085e4a62f9020b699b1";

        const string client_secret = "$2a$10$LprVXVd62l..QVc.YWP82epBEl3aovep0uqTUFnXUhJoFUkhSkBlm";

        public static string token;


        public static void Main(string[] args)
        {
            int preguntar = 0;

            while (preguntar == 0)
            {
                Console.WriteLine("Escoja la acción que desea hacer:\n a.- Guardar Categorias\n ei.- eliminar imagenes \n b.- Guardar Productos \n pi.- imágenes de productos ");

                string r = Console.ReadLine();

                //System.Timers.Timer timer = new System.Timers.Timer();

                //timer.Elapsed += OnTimedEvent;
                //timer.Interval = 5000;
                //timer.Start();
                //timer.AutoReset = true;

                //// Start the timer
                //timer.Enabled = true;

                Lectura(r);

            }
        }

        public static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            //Lectura("b");
        }

        public static void Lectura(string opcion)
        {
            int eAux = 0;

            int primerapasada = 0;

            if (opcion == "a")
            {
                while (eAux == 0)
                {
                    try
                    {
                        Console.WriteLine("************************************************** \n");

                        Console.WriteLine("Comienza lectura de Categorias - " + DateTime.Now);

                        WebRequest webCategoria = WebRequest.Create("https://api.codeswholesale.com/v2/platforms?access_token=" + token);

                        using (WebResponse respcategoria = webCategoria.GetResponse())
                        {
                            Stream catdataStream = respcategoria.GetResponseStream();

                            Console.WriteLine("Lectura correcta - categorias - " + DateTime.Now + "\n");

                            StreamReader catreader = new StreamReader(catdataStream);

                            string catresponseFromServer = catreader.ReadToEnd();

                            Console.WriteLine("Obteniendo Lista de Categorias - " + DateTime.Now);

                            dynamic catjson = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(catresponseFromServer);

                            Newtonsoft.Json.Linq.JArray listacategorias = catjson.platforms;

                            foreach (dynamic i in listacategorias)
                            {
                                string categoriaNombre = i.name;

                                Console.WriteLine("Leyendo Categoria - " + categoriaNombre + " _ " + DateTime.Now + "\n");

                                Boolean e = DL.AgregarCategorias(categoriaNombre);

                                if (e == true)
                                {
                                    Console.WriteLine("Categoria agregada - " + categoriaNombre + " _ " + DateTime.Now + "\n");
                                }

                                System.Threading.Thread.Sleep(1);
                            }

                            catreader.Close();

                            eAux = 1;
                        }
                    }
                    catch (WebException wex)
                    {
                        Console.WriteLine("Codewhosale Error - " + DateTime.Now + " - " + wex.Response.ToString());

                        if (wex.Response != null)
                        {
                            using (var errorResponse = (HttpWebResponse)wex.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    string error = reader.ReadToEnd();

                                    dynamic jsonDe = JsonConvert.DeserializeObject(error);

                                    Console.WriteLine(" Error web " + jsonDe + " - " + DateTime.Now);

                                    if (jsonDe.error == "invalid_token")
                                    {
                                        Console.WriteLine("Codewhosale Error : invalid_token " + DateTime.Now);

                                        #region token new
                                        try
                                        {
                                            WebRequest requesttoken = WebRequest.Create("https://api.codeswholesale.com/oauth/token?grant_type=client_credentials&client_id=" + client_id + "&client_secret=" + client_secret);

                                            requesttoken.ContentType = "application/json;charset=UTF-8";

                                            requesttoken.Method = "POST";

                                            var response = (HttpWebResponse)requesttoken.GetResponse();

                                            Console.WriteLine("Codewhosale Error Get token - " + DateTime.Now + " - " + response.StatusDescription);

                                            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                                            var items = JsonConvert.DeserializeObject<dynamic>(responseString);

                                            Console.WriteLine("Codewhosale Error Get token " + DateTime.Now + " - " + responseString);

                                            token = items.access_token;

                                            eAux = 0;

                                            #endregion
                                        }
                                        catch (WebException token_error)
                                        {
                                            eAux = 1;

                                            using (var token_error_str = (HttpWebResponse)token_error.Response)
                                            {
                                                var resp_error = token_error_str.GetResponseStream();

                                                using (StreamReader st_t_error = new StreamReader(resp_error))
                                                {
                                                    string string_resp = st_t_error.ReadToEnd();

                                                    dynamic jsonDe_resp = JsonConvert.DeserializeObject(string_resp);

                                                    Console.WriteLine("************************************************************************");

                                                    Console.WriteLine(" Error grave - generador de token " + jsonDe_resp + " - " + DateTime.Now);

                                                    Console.WriteLine("************************************************************************");
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("A la mierda todo " + ex.Message.ToString() + DateTime.Now);

                                            eAux = 1;
                                        }
                                    }
                                }
                                //   eAux = 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error obteniendo Lista de Categorias - " + ex.Message.ToString() + "_" + DateTime.Now);

                        eAux = 1;
                    }
                }
            }
            else if (opcion == "ei")
            {
                System.IO.DirectoryInfo d = new DirectoryInfo("imagenes");

                foreach (var f in d.GetFiles())
                {
                    f.Delete();

                    Console.WriteLine("eliminando Imagen : " + f.Name);
                }

            }
            else if (opcion == "pi")
            {
                Console.WriteLine("\n********** INICIO OBTENIENDO IMÁGENES DE PRODUCTOS ************\n");

                Dictionary<string, object> p = DL.GetListProducts();

                DataTable dt = (DataTable)p["valor"];

                foreach (DataRow r in dt.Rows)
                {
                    string id = r["productosId"].ToString();

                    string image = r["productosimagen"].ToString();

                    bool resp = gamekeyspotCodewhoSales.Services.GetImage.getImageApi(r["id_codewhosale"].ToString(), id);


                }

                //foreach (var i in (DataTable)dt)
                //{

                //}

            }
            else
            {
                while (eAux == 0)
                {
                    try
                    {
                        WebRequest request = WebRequest.Create("https://api.codeswholesale.com/v2/products?access_token=" + token);

                        request.Credentials = CredentialCache.DefaultCredentials;

                        Console.WriteLine("llamada: products - " + DateTime.Now + " - token: " + token);

                        WebResponse response = request.GetResponse();

                        Stream dataStream = response.GetResponseStream();

                        Console.WriteLine("Lectura correcta - products - " + DateTime.Now);

                        StreamReader reader = new StreamReader(dataStream);

                        string responseFromServer = reader.ReadToEnd();

                        JsonSerializer serializer = new JsonSerializer();

                        Console.WriteLine("Obteniendo Lista de productos - " + DateTime.Now);

                        dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseFromServer);

                        Newtonsoft.Json.Linq.JArray listaProductos = json.items;

                        foreach (dynamic i in listaProductos)
                        {
                            try
                            {
                                string region = "";

                                if (i.regions.Count > 0)
                                {
                                    region = i.regions[0];
                                }

                                decimal precio = 0;

                                if (i.prices.Count > 0)
                                {
                                    precio = i.prices[0].value;
                                }

                                string lenguajes = "";

                                if (i.languages.Count > 0)
                                {
                                    for (var il = 0; il < i.languages.Count; il++)
                                    {
                                        lenguajes += i.languages[il] + ",";
                                    }

                                    lenguajes = lenguajes.Substring(0, lenguajes.LastIndexOf(','));
                                }

                                string productoId = i.productId.ToString(), ProductoNombre = i.name, identifier = i.identifier, cantidad = i.quantity;
                                string releaseDate = i.releaseDate, plataform = i.platform;
                                string officialTitle = "", category = "", developerName = "", developerHomepage = "";

                                Console.WriteLine("********** Producto " + i.productId + " ************\n\n");

                                Console.WriteLine("Producto: " + productoId + " - " + ProductoNombre + " - cantidad:" + cantidad + " - release " + releaseDate + " - categoria: " + plataform + " - precio: " + precio + " - región: " + region + " - lenguajes: " + lenguajes + " _ " + DateTime.Now + "\n");

                                Console.WriteLine("---- Descripción " + i.productId + "---- \n");

                                Dictionary<string, object> dictdescripcion = new Dictionary<string, object>();

                                try
                                {
                                    //dictdescripcion = ObtenerDescripcion(i.productId.ToString());

                                    if (dictdescripcion.ContainsKey("error"))
                                    {
                                        if (dictdescripcion["error"].ToString() == "50042")
                                        {
                                            dictdescripcion["error"] = "No existe descripción";
                                        }

                                        Console.WriteLine("Error obteniendo descripción : " + i.productId + " : " + dictdescripcion["error"] + " - " + DateTime.Now);

                                        dictdescripcion.Clear();
                                    }
                                    else
                                    {
                                        Console.WriteLine("Datos de Descripción Finalizado : " + i.productId + " - " + DateTime.Now);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("************ Error descripción : programación**************" + ex.Message.ToString());
                                }

                                string imagenprincipal = string.Empty;

                                try
                                {
                                    if (i.images.Count > 0)
                                    {
                                        if (!File.Exists("imagenes/" + i.productId.ToString()))
                                        {
                                            Console.WriteLine("Guardando imagen _ " + i.productId.ToString() + "_" + DateTime.Now);

                                            string imageurl = "https://api.codeswholesale.com/v1/products/" + i.productId.ToString() + "/image?format=MEDIUM";

                                            imagenprincipal = SaveImageAsync(imageurl, i.productId.ToString());

                                            Console.WriteLine("Imagen guardada _ " + i.productId.ToString() + "_" + DateTime.Now);
                                        }
                                        else
                                        {
                                            imagenprincipal = i.productId.ToString();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }

                                Console.WriteLine("\n********** GUARDANDO DATABASE ************\n");

                                try
                                {
                                    ProductosModel m = new ProductosModel();

                                    string description = string.Empty;

                                    if (dictdescripcion.Count > 0)
                                    {
                                        officialTitle = dictdescripcion["officialTitle"].ToString();
                                        category = dictdescripcion["category"].ToString();
                                        developerName = dictdescripcion["developerName"].ToString();
                                        developerHomepage = dictdescripcion["developerHomepage"].ToString();

                                        if (dictdescripcion.ContainsKey("factSheets"))
                                        {
                                            try
                                            {
                                                description = dictdescripcion["factSheets"].ToString();

                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                    }

                                    m.productCodewhosaleId = productoId;

                                    m.productName = ProductoNombre;

                                    m.images = imagenprincipal;

                                    m.description = description;
                                    m.quantity = cantidad;
                                    m.regions = region;

                                    m.prices = precio;

                                    m.identifier = identifier;
                                    m.languages = lenguajes;
                                    m.escodewhosale = 1;
                                    m.platform = plataform;
                                    m.categoriaNombre = plataform;

                                    m.officialTitle = officialTitle;
                                    m.category = category;
                                    m.developerName = developerName;
                                    m.developerHomepage = developerHomepage;

                                    dynamic ProductoAlmacenado = DL.AgregarProducto(m);

                                    if (ProductoAlmacenado.ContainsKey("error"))
                                    {
                                        try
                                        {
                                            dynamic e = ProductoAlmacenado;

                                            Console.WriteLine("** ERROR GUARDANDO DATABASE ** ProductoAlmacenado ** : " + e["error"]);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("** ERROR GUARDANDO DATABASE ** ProductoAlmacenado ** : " + ex.Message.ToString());
                                        }

                                    }

                                    if (ProductoAlmacenado.ContainsKey("valor"))
                                    {
                                        string p = ProductoAlmacenado["valor"].ToString();

                                        Console.WriteLine("**** id: " + p + " ******");

                                        // para almacenar fotos //
                                        if (dictdescripcion.Count > 0)
                                        {
                                            if (dictdescripcion.ContainsKey("fotos"))
                                            {
                                                try
                                                {
                                                    dynamic df = dictdescripcion["fotos"];

                                                    if (df.Count > 0)
                                                    {
                                                        foreach (dynamic f in (List<string>)df)
                                                        {
                                                            string ext = ".jpg";

                                                            var fresult = DL.AgregarGaleria(p, "", f + ext, "fotos");

                                                            if (fresult == true)
                                                            {
                                                                Console.WriteLine("Foto guardada correctamente en db");
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Error guardando foto en db");
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Error guardando foto en db " + ex.Message.ToString());
                                                    ;
                                                }
                                            }

                                            if (dictdescripcion.ContainsKey("videos"))
                                            {
                                                try
                                                {
                                                    dynamic df = dictdescripcion["videos"];

                                                    if (df.Count > 0)
                                                    {
                                                        foreach (dynamic f in (List<string>)df)
                                                        {
                                                            string[] f_split = f.Split('#');

                                                            string url = string.Empty;
                                                            string titulo = string.Empty;

                                                            int fint = 0;

                                                            foreach (var sf in f_split)
                                                            {
                                                                if (fint == 0)
                                                                {
                                                                    url = sf;
                                                                }
                                                                else
                                                                {
                                                                    titulo = sf;
                                                                }
                                                                fint = fint + 1;
                                                            }

                                                            var fresult = DL.AgregarGaleria(p, titulo, f, "videos");

                                                            if (fresult == true)
                                                            {
                                                                Console.WriteLine("video guardada correctamente en db");
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Error guardando videos en db");
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    ex.Message.ToString();
                                                }
                                            }
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("** ERROR GUARDANDO DATABASE **" + ex.Message);
                                }


                                Console.WriteLine("\n\n********** Final Producto ************\n\n");

                                System.Threading.Thread.Sleep(50);
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();

                                Console.WriteLine(i);

                                Console.WriteLine("Leyendo Producto - " + i.name + " - categoria: " + i.platform + " - precio: " + i.prices[0].value + " - región: " + i.regions + " _ " + DateTime.Now + "\n");

                            }
                        }

                        reader.Close();
                        response.Close();

                        Console.WriteLine("Finalizo lectura de productos - " + DateTime.Now + "\n");

                        eAux = 0;

                        //PASA UNA VEZ AL DÍA//
                        //if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour <= 7)
                        //{


                        primerapasada = primerapasada + 1;
                    }
                    catch (WebException wex)
                    {
                        Console.WriteLine("Codewhosale Error - " + DateTime.Now + " - " + wex.Response.ToString());

                        if (wex.Response != null)
                        {
                            using (var errorResponse = (HttpWebResponse)wex.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    string error = reader.ReadToEnd();

                                    dynamic jsonDe = JsonConvert.DeserializeObject(error);

                                    Console.WriteLine(" Error web " + jsonDe + " - " + DateTime.Now);

                                    if (jsonDe.error == "invalid_token")
                                    {
                                        Console.WriteLine("Codewhosale Error : invalid_token " + DateTime.Now);

                                        #region token new
                                        try
                                        {
                                            WebRequest requesttoken = WebRequest.Create("https://api.codeswholesale.com/oauth/token?grant_type=client_credentials&client_id=" + client_id + "&client_secret=" + client_secret);

                                            requesttoken.ContentType = "application/json;charset=UTF-8";

                                            requesttoken.Method = "POST";

                                            var response = (HttpWebResponse)requesttoken.GetResponse();

                                            Console.WriteLine("Codewhosale Error Get token - " + DateTime.Now + " - " + response.StatusDescription);

                                            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                                            var items = JsonConvert.DeserializeObject<dynamic>(responseString);

                                            Console.WriteLine("Codewhosale Error Get token " + DateTime.Now + " - " + responseString);

                                            var a = items.access_token;

                                            token = items.access_token;

                                            eAux = 0;

                                            #endregion
                                        }
                                        catch (WebException token_error)
                                        {
                                            eAux = 1;

                                            using (var token_error_str = (HttpWebResponse)token_error.Response)
                                            {
                                                var resp_error = token_error_str.GetResponseStream();

                                                using (StreamReader st_t_error = new StreamReader(resp_error))
                                                {
                                                    string string_resp = st_t_error.ReadToEnd();

                                                    dynamic jsonDe_resp = JsonConvert.DeserializeObject(string_resp);

                                                    Console.WriteLine("************************************************************************");

                                                    Console.WriteLine(" Error grave - generador de token " + jsonDe_resp + " - " + DateTime.Now);

                                                    Console.WriteLine("************************************************************************");
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("A la mierda todo " + ex.Message.ToString() + DateTime.Now);

                                            eAux = 1;
                                        }
                                    }
                                }
                                //   eAux = 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error de programación - " + ex.Message.ToString() + " - " + DateTime.Now);

                        eAux = 1;
                    }
                    // finalizo while
                    if (primerapasada > 0)
                    {
                        Console.WriteLine("15 mint para la proxima pasada - " + DateTime.Now);
                        System.Threading.Thread.Sleep(7200000); // 2 h
                    }

                }
            }

        }

        public static Dictionary<string, object> ObtenerDescripcion(string productoid)
        {
            int eAux = 0;

            string idioma = "es";
            Dictionary<string, object> d = new Dictionary<string, object>();
            while (eAux == 0)
            {
                eAux = 1;
                try
                {


                    string url = "https://api.codeswholesale.com/v2/products/" + productoid + "/description?access_token=" + token;

                    WebRequest request = WebRequest.Create(url);

                    request.Credentials = CredentialCache.DefaultCredentials;

                    Console.WriteLine("llamada: producto descripción - " + DateTime.Now + " - token: " + token);

                    WebResponse response = request.GetResponse();

                    Stream dataStream = response.GetResponseStream();

                    Console.WriteLine("Lectura descripción - " + productoid + " - " + DateTime.Now);

                    StreamReader reader = new StreamReader(dataStream);

                    string responseFromServer = reader.ReadToEnd();

                    JsonSerializer serializer = new JsonSerializer();

                    Console.WriteLine("Obteniendo Descripción de producto - " + productoid + " _ " + DateTime.Now);

                    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseFromServer);

                    //Newtonsoft.Json.Linq.JArray datos = json;
                    string officialTitle = json.officialTitle;
                    string category = json.category;
                    string developerName = json.developerName;
                    string developerHomepage = json.developerHomepage;

                    Console.WriteLine("Datos - " + productoid + " _ " + officialTitle + " _ " + json.platform + "_" + category + "_" + developerName + "_" + developerHomepage + " - " + DateTime.Now + "\n");

                    d.Add("officialTitle", officialTitle);
                    d.Add("category", category);
                    d.Add("developerName", developerName);
                    d.Add("developerHomepage", developerHomepage);

                    if (json.factSheets != null)
                    {
                        try
                        {
                            for (var f = 0; f < json.factSheets.Count; f++)
                            {
                                if (json.factSheets[f].description != null)
                                {
                                    string descripcion = json.factSheets[f].description;
                                    string territorio = json.factSheets[f].territory.ToString().ToLower();

                                    if (territorio == "uk" || territorio == "es")
                                    {
                                        d.Add("factSheets", descripcion);
                                    }

                                    if (descripcion.Length > 25)
                                    {
                                        Console.WriteLine("descripción: " + descripcion.ToString().Substring(0, 25) + " _ " + territorio);
                                    }
                                    else
                                    {
                                        Console.WriteLine("descripción: " + descripcion.ToString() + " _ " + territorio);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }

                    if (json.videos != null)
                    {
                        try
                        {
                            List<string> dictVideos = new List<string>();

                            for (var f = 0; f < json.videos.Count; f++)
                            {
                                if (json.videos[f].url != null)
                                {
                                    string videoUrl = json.videos[f].url.ToString();

                                    string videoTitle = json.videos[f].title;

                                    dictVideos.Add(videoUrl + "#" + videoTitle);

                                    Console.WriteLine(videoUrl + "_" + videoTitle);
                                }
                            }

                            //guardo en el diccionario principal la lista de fotos
                            d.Add("videos", dictVideos);
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }

                    if (json.photos != null)
                    {
                        Console.WriteLine("Fotos - " + productoid + "-");

                        List<string> dictFotos = new List<string>();

                        for (var f = 0; f < json.photos.Count; f++)
                        {
                            try
                            {
                                string foto = json.photos[f].url;

                                string ext = Path.GetExtension(foto);

                                if (!File.Exists("imagenes/" + productoid.ToString() + "_" + f))
                                {
                                    Console.WriteLine("Guardando imagen _ " + productoid.ToString() + "_" + f + " - " + DateTime.Now);

                                    string imagen = SaveImageAsync(foto, productoid.ToString() + "_" + f).Result;

                                    dictFotos.Add(imagen);

                                    Console.WriteLine("Imagen guardada _ " + productoid.ToString() + "_" + f + " - " + DateTime.Now);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error Imagen Descripción _ " + productoid.ToString() + " - " + ex.Message + "_" + f + " - " + DateTime.Now);
                            }
                        }
                        //guardo en el diccionario principal la lista de fotos
                        d.Add("fotos", dictFotos);
                    }

                    System.Threading.Thread.Sleep(100);

                }
                catch (WebException wex)
                {
                    //Console.WriteLine("Codewhosale Error - " + DateTime.Now + " - " + wex.Response.ToString());
                    eAux = 1;
                    if (wex.Response != null)
                    {
                        using (var errorResponse = (HttpWebResponse)wex.Response)
                        {
                            using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                            {
                                string error = reader.ReadToEnd();

                                dynamic jsonDe = JsonConvert.DeserializeObject(error);

                                try
                                {
                                    Console.WriteLine("Error web " + jsonDe.message + " - " + DateTime.Now + "\n");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("wex.Response : descripción :" + ex.Message + " - " + DateTime.Now);
                                }

                                //paso el error/ de que no contiene descripción//
                                d.Add("error", jsonDe.code);

                                if (jsonDe.error == "invalid_token")
                                {

                                    eAux = 0;
                                    Console.WriteLine("Codewhosale Error : invalid_token " + DateTime.Now);

                                    #region token new
                                    try
                                    {
                                        WebRequest requesttoken = WebRequest.Create("https://api.codeswholesale.com/oauth/token?grant_type=client_credentials&client_id=" + client_id + "&client_secret=" + client_secret);

                                        requesttoken.ContentType = "application/json;charset=UTF-8";

                                        requesttoken.Method = "POST";

                                        var response = (HttpWebResponse)requesttoken.GetResponse();

                                        Console.WriteLine("Codewhosale Error Get token - " + DateTime.Now + " - " + response.StatusDescription);

                                        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                                        var items = JsonConvert.DeserializeObject<dynamic>(responseString);

                                        Console.WriteLine("Codewhosale Error Get token " + DateTime.Now + " - " + responseString);

                                        token = items.access_token;

                                        eAux = 0;

                                        #endregion
                                    }
                                    catch (WebException token_error)
                                    {
                                        eAux = 1;

                                        using (var token_error_str = (HttpWebResponse)token_error.Response)
                                        {
                                            var resp_error = token_error_str.GetResponseStream();

                                            using (StreamReader st_t_error = new StreamReader(resp_error))
                                            {
                                                string string_resp = st_t_error.ReadToEnd();

                                                dynamic jsonDe_resp = JsonConvert.DeserializeObject(string_resp);

                                                Console.WriteLine("************************************************************************");

                                                Console.WriteLine(" Error grave - generador de token " + jsonDe_resp + " - " + DateTime.Now);

                                                Console.WriteLine("************************************************************************");

                                                d.Add("error", jsonDe_resp);

                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("A la mierda todo " + ex.Message.ToString() + DateTime.Now);

                                        eAux = 1;
                                        d.Add("error", ex.Message);
                                    }
                                }
                            }
                            //   eAux = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    eAux = 1;
                    Console.WriteLine("Error - " + ex.Message + " _ " + DateTime.Now);
                    d.Add("error", ex.Message);
                }
            }
            return d;
        }

        public static async Task<string> SaveImageAsync(string imageUrl, string filename)
        {
            try
            {
                WebClient client = new WebClient();
                Stream stream = await client.OpenReadTaskAsync(imageUrl);
                Bitmap bitmap;

                filename = filename + ".jpg";

                bitmap = new Bitmap(stream);

                if (bitmap != null)
                    bitmap.Save("imagenes/" + filename, ImageFormat.Jpeg);

                stream.Flush();
                stream.Close();
                client.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error obteniendo imagen - " + ex.Message.ToString() + " _ " + DateTime.Now);
            }

            return filename;



        }

    }
}
