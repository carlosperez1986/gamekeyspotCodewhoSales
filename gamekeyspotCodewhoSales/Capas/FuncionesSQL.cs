using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Capas
{
    public static class FuncionesSQL
    {
        /// <summary>
        /// const string strcnn = @"Data Source=localhost;Initial Catalog=gamekeyspotdb;User ID=sa;Password=Carloselias23.;Packet Size=4096";
        /// </summary>
        /// <value>The coneccion string.</value>
        public static string ConeccionString { get; set; }


        /// <summary>
        /// Parametro Tipo para identificar 0 = executenonquery para insert,update,delete; 1= scalar para select(*)
        /// </summary>
        public static async Task<Dictionary<string, object>> PostData(string CommandText, List<SqlParameter> SqlParametros, int tipo)
        {
            SqlConnectionStringBuilder builder =
            new SqlConnectionStringBuilder(ConeccionString);

            builder.AsynchronousProcessing = true;

            Dictionary<string, object> resultado = new Dictionary<string, object>();

            try
            {
                using (SqlConnection cnn = new SqlConnection(builder.ConnectionString))
                {
                    await cnn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand(CommandText, cnn))
                    {
                        foreach (SqlParameter s in SqlParametros)
                        {
                            SqlParameter sp = new SqlParameter()
                            {
                                Value = s.Value,
                                SqlValue = s.SqlValue,
                                ParameterName = s.ParameterName,
                                DbType = s.DbType,
                                SqlDbType = s.SqlDbType
                            };

                            cmd.Parameters.Add(s);
                        }

                        if (tipo == 0)
                        {
                            resultado.Add("valor", cmd.ExecuteNonQueryAsync().Result.ToString());
                        }
                        else
                        {
                            resultado.Add("valor", cmd.ExecuteScalarAsync().Result.ToString());
                        }
                    }
                    cnn.Close();
                }
            }
            catch (Exception ex)
            {
                resultado.Add("error", ex.Message + "|" + ex.InnerException);
                resultado.Add("commandtext", CommandText);
                resultado.Add("tipo", tipo);
                return resultado;
            }

            return resultado;
        }

        public static async Task<Dictionary<string, object>> GetData(string CommandText, List<SqlParameter> SqlParametros)
        {
            SqlConnectionStringBuilder builder =
                new SqlConnectionStringBuilder(ConeccionString);

            builder.AsynchronousProcessing = true;

            Dictionary<string, object> resultado = new Dictionary<string, object>();

            try
            {
                using (SqlConnection cnn = new SqlConnection(builder.ConnectionString))
                {
                    await cnn.OpenAsync();

                    string sql = CommandText;

                    using (SqlCommand cmd = new SqlCommand(CommandText, cnn))
                    {
                        if (SqlParametros != null)
                        {
                            foreach (SqlParameter s in SqlParametros)
                            {
                                SqlParameter sp = new SqlParameter()
                                {
                                    Value = s.Value,
                                    SqlValue = s.SqlValue,
                                    ParameterName = s.ParameterName,
                                    DbType = s.DbType
                                };

                                cmd.Parameters.Add(s);
                            }
                        }

                        IDataReader sr = cmd.ExecuteReader();

                        DataTable dt = new DataTable();

                        dt.Load(sr);


                        if (dt.Rows.Count > 0)
                        {
                            resultado.Add("valor", dt);
                            resultado.Add("count", dt.Rows.Count);
                            resultado.Add("p_valor", dt.Rows[0]);
                        }
                        else
                        {
                            resultado.Add("valor", "sf");
                            resultado.Add("count", dt.Rows.Count);
                            resultado.Add("p_valor", null);
                        }
                        return resultado;
                    }

                }
            }
            catch (Exception ex)
            {
                resultado.Add("error", ex.Message);
                resultado.Add("commandtext", CommandText);

                return resultado;
            }
        }



    }
}
