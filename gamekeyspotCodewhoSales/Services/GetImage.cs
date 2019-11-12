using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using gamekeyspot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace gamekeyspotCodewhoSales.Services
{
    public static class GetImage
    {
        const string client_id = "1051f676d039d085e4a62f9020b699b1";

        const string client_secret = "$2a$10$LprVXVd62l..QVc.YWP82epBEl3aovep0uqTUFnXUhJoFUkhSkBlm";

        public static string token;

        public static bool getImageApi(string i, string id)
        {
            bool p = true;

            while (p)
            {
                try
                {
                    Console.WriteLine("Guardando imagen _ " + i.ToString() + "_" + DateTime.Now);

                    string imageurl = "https://api.codeswholesale.com/v1/products/" + i.ToString() + "/image?format=MEDIUM";

                    string imagenprincipal = MainClass.SaveImageAsync(imageurl, i.ToString()).Result;

                    var dr = Capas.DL.UpdateImage(i, imagenprincipal, id);

                    //WebRequest request = WebRequest.Create($"https://api.codeswholesale.com/v2/products/{id}?access_token=" + token);

                    //request.Credentials = CredentialCache.DefaultCredentials;

                    //Console.WriteLine("llamada: imagenes - " + DateTime.Now + " - token: " + token);

                    //WebResponse response = request.GetResponse();

                    //Stream dataStream = response.GetResponseStream();

                    //Console.WriteLine("Lectura correcta - imagenes - " + DateTime.Now);

                    //StreamReader reader = new StreamReader(dataStream);

                    //string responseFromServer = reader.ReadToEnd();

                    //JsonSerializer serializer = new JsonSerializer();

                    //Console.WriteLine("Obteniendo Lista de productos - " + DateTime.Now);

                    //Dictionary<string, object> json = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseFromServer);

                    //foreach (JArray i in (JArray)json["images"])
                    //{


                    //}




                    //if (i.images.Count > 0)
                    //{
                    //    if (!File.Exists("imagenes/" + i.productId.ToString()))
                    //    {
                    //        Console.WriteLine("Guardando imagen _ " + i.productId.ToString() + "_" + DateTime.Now);

                    //        string imageurl = "https://api.codeswholesale.com/v1/products/" + i.productId.ToString() + "/image?format=MEDIUM";

                    //        imagenprincipal = SaveImageAsync(imageurl, i.productId.ToString());

                    //        Console.WriteLine("Imagen guardada _ " + i.productId.ToString() + "_" + DateTime.Now);
                    //    }
                    //    else
                    //    {
                    //        imagenprincipal = i.productId.ToString();
                    //    }
                    //}

                    p = false;

                    return true;
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



                                        #endregion
                                    }
                                    catch (WebException token_error)
                                    {

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


                                    }
                                }
                            }
                            //   eAux = 0;
                        }
                    }
                }
            }
            return false;
        }
    }
}
