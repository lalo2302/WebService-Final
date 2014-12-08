using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Conexion
/// </summary>
public class Conexion
{
    public Conexion()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public static string ObtenerCadenaConexion()
    {
        return ConfigurationManager.ConnectionStrings["ConexionMySQL"].ToString();
    }
}