using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Capas
{
    public class DL
    {
        //const string strcnn = @"Data Source=82.223.108.84;Initial Catalog=gamekeyspotdb; User ID=sa;Password=Bp9Ea51VGI; Packet Size=4096";
        const string strcnn = @"Data Source=localhost,1433;Initial Catalog=shopcartdb;User ID=sa;Password=Carloselias23.;Packet Size=4096";

        public static Dictionary<string, object> GetListProducts()
        {
            Dictionary<string, object> d = new Dictionary<string, object>();

            FuncionesSQL.ConeccionString = strcnn;

            string sql = "select  * from tblproductos";

            var dictList = FuncionesSQL.GetData(sql, null).Result;


            return dictList;
        }

        public static Dictionary<string, object> AgregarProducto(ProductosModel model)
        {
            Dictionary<string, object> d = new Dictionary<string, object>();

            FuncionesSQL.ConeccionString = strcnn;

            string sqlExiste = "select count(*) from tblproductos where id_codewhosale = @id_codewhosale and escodewhosale = 1";

            List<SqlParameter> s = new List<SqlParameter>();

            s.Add(new SqlParameter()
            {
                ParameterName = "@ProductosNombre",
                Value = model.productName,
                DbType = DbType.String,
                SqlValue = model.productName
            });

            s.Add(new SqlParameter()
            {
                ParameterName = "@id_codewhosale",
                Value = model.productCodewhosaleId,
                DbType = DbType.String,
                SqlValue = model.productCodewhosaleId
            });

            var resultExiste = FuncionesSQL.PostData(sqlExiste, s, 1).Result;

            if (resultExiste.ContainsKey("error"))
            {
                d.Add("error", resultExiste.ContainsKey("error"));

                return d;
            }

            if (Convert.ToInt32(resultExiste["valor"]) < 1)
            {
                Console.WriteLine("resultExiste: " + resultExiste["valor"]);

                string sqlbuscaCategoriaId = "select * from tblproductoscategorias where lower(CategoriasNombre) = lower(@CategoriasNombre)";

                List<SqlParameter> sc = new List<SqlParameter>();

                sc.Add(new SqlParameter()
                {
                    ParameterName = "@CategoriasNombre",
                    Value = model.categoriaNombre,
                    DbType = DbType.String,
                    SqlValue = model.categoriaNombre
                });

                Console.WriteLine("categoriaNombre: " + model.categoriaNombre + "_" + sqlbuscaCategoriaId);

                var resultsqlbuscaCategoriaId = FuncionesSQL.GetData(sqlbuscaCategoriaId, sc).Result;


                Console.WriteLine("*********************");


                if (resultsqlbuscaCategoriaId.ContainsKey("error"))
                {
                    Console.WriteLine("error: " + resultsqlbuscaCategoriaId["error"]);
                    Console.WriteLine("*********************");
                }

                string categoria = string.Empty;
                string categoriaId = string.Empty;

                if (resultsqlbuscaCategoriaId.ContainsKey("valor"))
                {
                    try
                    {
                        var dcategoria = resultsqlbuscaCategoriaId["valor"];
                        if (dcategoria != null)
                        {

                            DataRow dr = (DataRow)resultsqlbuscaCategoriaId["p_valor"];

                            Console.WriteLine("p_valor: " + dr["CategoriasNombre"]);

                            categoria = dr["CategoriasNombre"].ToString();
                            categoriaId = dr["CategoriasId"].ToString();

                            Console.WriteLine("* ********************");
                        }
                        else
                        {
                            Console.WriteLine("datos nulos * ********************");
                        }
                    }
                    catch (Exception ex)
                    {
                        categoria = "";
                        categoriaId = "0";

                        Console.WriteLine("No contiene error :" + ex.Message + "*********************");
                    }
                };


                Console.WriteLine("Entro en resultsqlbuscaCategoriaId: " + categoria + " - " + categoriaId);

                if (Convert.ToInt32(categoriaId) > 0)
                {
                    Console.WriteLine("Entro en tblproductos: " + model);

                    string sqlInsert = "INSERT INTO tblproductos(CategoriasId, ProductosFechaCreacion," +
                           "ProductosNombre," +
                           "ProductosCodigo," +
                           "ProductosImagen," +
                           "ProductosDescripcion," +
                           "ProductosDescripcionAmpliada," +
                           "ProductosPrecio," +
                           "ProductosPrecioVenta," +
                           "ProductosCantidad," +
                           "Visible," +
                           "regions,lenguajes," +
                           "escodewhosale," +
                           "id_codewhosale," +
                           "officialTitle," +
                           "category, developerName, developerHomepage" +
                           ")" +
                           "VALUES" +
                           "(@CategoriasId, GETDATE()," +
                           "@ProductosNombre," +
                           "@ProductosCodigo," +
                           "@ProductosImagen," +
                           "@ProductosDescripcion," +
                           "@ProductosDescripcionAmpliada," +
                           "@ProductosPrecio," +
                           "@ProductosPrecioVenta," +
                           "@ProductosCantidad," +
                           "@Visible,@regions," +
                           "@lenguajes," +
                           "@escodewhosale," +
                           "@id_codewhosale," +
                           "@officialTitle," +
                           "@category, @developerName, @developerHomepage); SELECT CAST(scope_identity() AS int)";

                    List<SqlParameter> sInsert = new List<SqlParameter>();

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "CategoriasId",
                        Value = Convert.ToInt32(categoriaId),
                        DbType = DbType.Int32,
                        SqlValue = Convert.ToInt32(categoriaId),
                        SqlDbType = SqlDbType.Int
                    });

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@ProductosNombre",
                        SqlValue = model.productName,
                        Value = model.productName,
                        DbType = DbType.String,
                        SqlDbType = SqlDbType.VarChar
                    });


                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@ProductosCodigo",
                        SqlValue = model.identifier,
                        Value = model.identifier,
                        DbType = DbType.String,
                        SqlDbType = SqlDbType.VarChar
                    });

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@ProductosImagen",
                        SqlValue = model.images,
                        Value = model.images,
                        DbType = DbType.String
                    });

                    Console.WriteLine("sInsert: parametros 1 - 4");

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@ProductosDescripcion",
                        SqlValue = model.description,
                        Value = model.description,
                        DbType = DbType.String
                    });

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@ProductosDescripcionAmpliada",
                        SqlValue = model.description,
                        Value = model.description,
                        DbType = DbType.String
                    });

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@ProductosPrecio",
                        SqlValue = model.prices,
                        Value = model.prices,
                        DbType = DbType.Decimal
                    });

                    decimal porcentaje = Convert.ToDecimal(.20);

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@ProductosPrecioVenta",
                        SqlValue = model.prices + ((model.prices * porcentaje) / 100),
                        Value = model.prices + ((model.prices * porcentaje) / 100),
                        DbType = DbType.Decimal
                    });
                    Console.WriteLine("sInsert: parametros 4 - 8");
                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@ProductosCantidad",
                        SqlValue = model.quantity,
                        Value = model.quantity,
                        DbType = DbType.String
                    });

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@Visible",
                        SqlValue = true,
                        Value = true,
                        DbType = DbType.Boolean
                    });

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@regions",
                        SqlValue = model.regions,
                        Value = model.regions,
                        DbType = DbType.String,
                        SqlDbType = SqlDbType.NVarChar
                    });

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@lenguajes",
                        SqlValue = model.languages,
                        Value = model.languages,
                        DbType = DbType.String
                    });
                    Console.WriteLine("sInsert: parametros 8 - 12");
                    //sInsert.Add(new SqlParameter()
                    //{
                    //    ParameterName = "@releaseDate",
                    //    SqlValue = model.releaseDate,
                    //    Value = model.releaseDate,
                    //    DbType = DbType.Date
                    //});

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@escodewhosale",
                        SqlValue = true,
                        Value = true,
                        DbType = DbType.Boolean
                    });

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@id_codewhosale",
                        SqlValue = model.productCodewhosaleId,
                        Value = model.productCodewhosaleId,
                        DbType = DbType.String
                    });

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@officialTitle",
                        SqlValue = model.officialTitle,
                        Value = model.officialTitle,
                        DbType = DbType.String
                    });

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@category",
                        SqlValue = model.category,
                        Value = model.category,
                        DbType = DbType.String
                    });

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@developerName",
                        SqlValue = model.developerName,
                        Value = model.developerName,
                        DbType = DbType.String
                    });

                    sInsert.Add(new SqlParameter()
                    {
                        ParameterName = "@developerHomepage",
                        SqlValue = model.developerHomepage,
                        Value = model.developerHomepage,
                        DbType = DbType.String,
                        SqlDbType = SqlDbType.NVarChar
                    });

                    Console.WriteLine("sInsert: parametros 8 - 12");

                    var r = FuncionesSQL.PostData(sqlInsert, sInsert, 1).Result;

                    Console.WriteLine("PostData: ");

                    if (r.ContainsKey("valor"))
                    {
                        d.Add("valor", r["valor"]);
                    }
                    else
                    {
                        dynamic e = r["error"];
                        Console.WriteLine("PostData: " + e);
                        d.Add("valor", e);
                    }

                    return d;
                }
                else
                {
                    d.Add("error", "No existe categoria: " + model.categoriaNombre);

                    return d;
                    ///errorr productos sin categoria//
                };
            }
            else
            {
                //edito producto// 
                string sqlbuscadatosProducto = "select * from tblproductoscategorias where productoId = @productoId or  id_codewhosale = @id_codewhosale";

                List<SqlParameter> p_buscaProducto = new List<SqlParameter>();

                p_buscaProducto.Add(new SqlParameter()
                {
                    ParameterName = "@productoId",
                    Value = model.productId,
                    DbType = DbType.Int32,
                    SqlValue = model.productId
                });

                p_buscaProducto.Add(new SqlParameter()
                {
                    ParameterName = "@id_codewhosale",
                    SqlValue = model.productCodewhosaleId,
                    Value = model.productCodewhosaleId,
                    DbType = DbType.String
                });

                var resultsqlbuscaCategoriaId = FuncionesSQL.GetData(sqlbuscadatosProducto, p_buscaProducto).Result;



            };

            return d;
        }

        public static bool AgregarGaleria(string productoId, string titulo, string ruta, string tipo)
        {
            Console.WriteLine("entro en AgregarGaleria");

            if (tipo == "fotos")
            {
                Console.WriteLine("Entro en Fotos");
                string sqlbuscarImagen = "select count(*) from tblProductosGaleria where productosId = @productoId AND lower(imagen) = lower(@imagen)";

                List<SqlParameter> p_buscarimagen = new List<SqlParameter>();

                p_buscarimagen.Add(new SqlParameter()
                {
                    ParameterName = "@productoId",
                    Value = Convert.ToInt32(productoId),
                    DbType = DbType.Int32,
                    SqlValue = Convert.ToInt32(productoId),
                    SqlDbType = SqlDbType.Int
                });

                p_buscarimagen.Add(new SqlParameter()
                {
                    ParameterName = "@imagen",
                    Value = ruta,
                    DbType = DbType.String,
                    SqlValue = ruta,
                    SqlDbType = SqlDbType.VarChar
                });

                try
                {
                    dynamic existe = FuncionesSQL.PostData(sqlbuscarImagen, p_buscarimagen, 1).Result;

                    Console.WriteLine("entro existe Imagen: " + existe);

                    if (existe.ContainsKey("error"))
                    {
                        Console.WriteLine(" ei existe Imagen: " + existe["error"]);
                    };

                    if (existe.ContainsKey("valor"))
                    {

                        if (Convert.ToInt32(existe["valor"]) < 1)
                        {
                            Console.WriteLine("Existe Imagen: " + existe["valor"]);

                            string sqlinsert = "INSERT INTO tblProductosGaleria(TITULO,IMAGEN,PRODUCTOSID)VALUES(@TITULO,@IMAGEN,@PRODUCTOID)";

                            List<SqlParameter> p_add = new List<SqlParameter>();

                            p_add.Add(new SqlParameter()
                            {
                                ParameterName = "@TITULO",
                                Value = titulo,
                                DbType = DbType.String,
                                SqlValue = titulo
                            });

                            p_add.Add(new SqlParameter()
                            {
                                ParameterName = "@IMAGEN",
                                Value = ruta,
                                DbType = DbType.String,
                                SqlValue = ruta
                            });

                            p_add.Add(new SqlParameter()
                            {
                                ParameterName = "@PRODUCTOID",
                                Value = Convert.ToInt32(productoId),
                                DbType = DbType.Int32,
                                SqlValue = Convert.ToInt32(productoId)
                            });

                            var r = FuncionesSQL.PostData(sqlinsert, p_add, 0).Result;

                            if (r.ContainsKey("valor"))
                            {
                                Console.WriteLine("AgregarGaleria: Foto Valor " + r.ContainsKey("valor"));
                                return true;
                            }

                            if (r.ContainsKey("error"))
                            {
                                Console.WriteLine("AgregarGaleria: Error " + r.ContainsKey("error"));
                                return false;
                            };


                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Existe Imagen: " + ex.InnerException);

                    return false;
                }
                return true;
            }
            else
            {
                Console.WriteLine("Entro en Videos");
                string sqlbuscarImagen = "select count(*) from tblProductosGaleria where productosId = @productoId and lower(video) = lower(@video)";

                List<SqlParameter> p_buscarimagen = new List<SqlParameter>();

                p_buscarimagen.Add(new SqlParameter()
                {
                    ParameterName = "@productoId",
                    Value = Convert.ToInt32(productoId),
                    DbType = DbType.Int32,
                    SqlValue = Convert.ToInt32(productoId)
                });

                p_buscarimagen.Add(new SqlParameter()
                {
                    ParameterName = "@video",
                    Value = ruta,
                    DbType = DbType.String,
                    SqlValue = ruta
                });

                dynamic existe = FuncionesSQL.PostData(sqlbuscarImagen, p_buscarimagen, 1).Result;

                if (Convert.ToInt32(existe["valor"]) < 1)
                {
                    try
                    {
                        string sqlinsert = "INSERT INTO tblProductosGaleria(TITULO,VIDEO,PRODUCTOSID)VALUES(@TITULO,@VIDEO,@PRODUCTOID)";

                        List<SqlParameter> p_add = new List<SqlParameter>();

                        p_add.Add(new SqlParameter()
                        {
                            ParameterName = "@TITULO",
                            Value = titulo,
                            DbType = DbType.String,
                            SqlValue = titulo
                        });

                        p_add.Add(new SqlParameter()
                        {
                            ParameterName = "@VIDEO",
                            Value = ruta,
                            DbType = DbType.String,
                            SqlValue = ruta
                        });

                        p_add.Add(new SqlParameter()
                        {
                            ParameterName = "@PRODUCTOID",
                            Value = Convert.ToInt32(productoId),
                            DbType = DbType.Int32,
                            SqlValue = Convert.ToInt32(productoId)
                        });

                        var r = FuncionesSQL.PostData(sqlinsert, p_add, 0).Result;

                        if (r.ContainsKey("valor"))
                        {

                            Console.WriteLine("AgregarGaleria: Videos Valor " + r.ContainsKey("valor"));
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("AgregarGaleria: Videos Error " + r.ContainsKey("error"));
                            return false;
                        };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("AgregarGaleria: Error " + ex.Message.ToString());

                        return false;
                    }
                }

                return true;
            }
        }


        public static bool AgregarCategorias(string nombre)
        {
            string sql = "select count(*) from tblproductoscategorias where lower(CategoriasNombre) = lower(@CategoriasNombre)";

            List<SqlParameter> s = new List<SqlParameter>();

            s.Add(new SqlParameter()
            {
                ParameterName = "@CategoriasNombre",
                DbType = DbType.String,
                Value = nombre,
                SqlValue = nombre
            });

            FuncionesSQL.ConeccionString = strcnn;

            var existe = FuncionesSQL.PostData(sql, s, 1).Result;

            int valor = 1;

            if (existe.ContainsKey("error"))
            {
                // error grave //
            }
            else
            {
                valor = Convert.ToInt32(existe["valor"].ToString());
            }

            if (valor < 1)
            {
                string sqlinsert = "INSERT INTO tblproductoscategorias(CategoriasNombre,visible)VALUES(@CategoriasNombre,1)";

                List<SqlParameter> sinsert = new List<SqlParameter>();

                sinsert.Add(new SqlParameter()
                {
                    ParameterName = "@CategoriasNombre",
                    DbType = DbType.String,
                    Value = nombre,
                    SqlValue = nombre
                });

                var insertaCategoria = FuncionesSQL.PostData(sqlinsert, sinsert, 0).Result;

                if (insertaCategoria.ContainsKey("error"))
                {
                    // error grave //
                }
                else
                {
                    valor = Convert.ToInt32(existe["valor"].ToString());
                }

                return true;
            };

            return false;
        }

        public static Dictionary<string, object> UpdateImage(string productoId, string imagen, string id)
        {
            List<SqlParameter> p_add = new List<SqlParameter>();

            p_add.Add(new SqlParameter()
            {
                ParameterName = "@id_codewhosale",
                Value = productoId,
                DbType = DbType.String,
                SqlValue = productoId
            });

            p_add.Add(new SqlParameter()
            {
                ParameterName = "@image",
                Value = imagen,
                DbType = DbType.String,
                SqlValue = imagen
            });

            p_add.Add(new SqlParameter()
            {
                ParameterName = "@Id",
                Value = id,
                DbType = DbType.Int64,
                SqlValue = id
            });

            string sqlbuscarImagen = "update tblProductos set productosImagen = @image, Id = @Id where id_codewhosale = @id_codewhosale ";

            var r = FuncionesSQL.PostData(sqlbuscarImagen, p_add, 0).Result;

            return r;
        }

    }
}
