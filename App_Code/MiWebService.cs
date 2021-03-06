﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;

/// <summary>
/// Summary description for MiWebService
/// </summary>
[WebService]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class MiWebService : System.Web.Services.WebService
{

    //public MiWebService() {

    //Uncomment the following line if using designed components 
    //InitializeComponent(); 
    //}

    [WebMethod]
    public string ObtenerComentarios(string idLibro_comentario)
    {
        List<Comentario> ListaDeComentarios = new List<Comentario>();
        MySqlConnection miConexionSQL = new MySqlConnection(Conexion.ObtenerCadenaConexion());
        if (miConexionSQL.State == System.Data.ConnectionState.Closed)
            miConexionSQL.Open();
        if (miConexionSQL.State == System.Data.ConnectionState.Open)
        {
            MySqlCommand query = new MySqlCommand
                ("select * from comentarios where idLibro = '" + idLibro_comentario + "';", miConexionSQL);
            query.CommandType = System.Data.CommandType.Text;
            query.CommandTimeout = 120;
            MySqlDataReader reader = null;
            DataTable tabla = new DataTable();
            query.Prepare();
            reader = query.ExecuteReader();
            tabla.Load(reader);
            reader.Close();
            query.Dispose();
            miConexionSQL.Close();

            if (tabla.Rows.Count > 0)
            {
                Comentario comentario = null;
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    comentario = new Comentario();
                    comentario.idLibro = tabla.Rows[i]["idLibro"].ToString();
                    comentario.Com = tabla.Rows[i]["comentario"].ToString();
                    ListaDeComentarios.Add(comentario);
                }
                ListaDeComentarios = ListaDeComentarios.OrderBy(x => x.idLibro).ToList();
                string json = JsonConvert.SerializeObject(ListaDeComentarios);

                return json;
            }
            
        }
        return null;
    }
    [WebMethod]
    public string ObtenerLibros()
    {
        List<Libro> ListaDeLibros = new List<Libro>();
        MySqlConnection miConexionSQL = new MySqlConnection(Conexion.ObtenerCadenaConexion());

        if (miConexionSQL.State == System.Data.ConnectionState.Closed)
            miConexionSQL.Open();
        if (miConexionSQL.State == System.Data.ConnectionState.Open)
        {

            MySqlCommand query = new MySqlCommand("select libros.idLibro, libros.nombreLibro, libros.portada, libros.autorEnsayo, libros.genero, autores.nombreAutor, autores.apellidoAutor FROM libros, autores, autorlibro WHERE autorlibro.idLibro = libros.idLibro and autorlibro.idAutor = autores.idAutor order by libros.idLibro", miConexionSQL);
            query.CommandType = System.Data.CommandType.Text;
            query.CommandTimeout = 120; //120 segundos
            MySqlDataReader reader = null;
            DataTable tabla = new DataTable(); //Tabla para almacenar los datos temporales

            /*DataTable toma la primera columna de la tabla como primary key por defecto, asi que no van a salir libros
            repetidos porque el idLibro no se puede repetir. El libro que tiene dos autores necesitamos que salga dos veces
             para capturar ambos autores, asi que se agrega una nueva columna autoincrementable y a esa columna se le asigna
             el primary key*/

            tabla.Columns.Add(new System.Data.DataColumn("PK", typeof(System.Int32))); //agregar una nueva columna llamada PK
            tabla.PrimaryKey = new DataColumn[1] { tabla.Columns["PK"] }; //asignar nueva columna como primary key
            tabla.Columns["PK"].AutoIncrement = true; //autoincrement en nueva columna

            query.Prepare();//Inicializamos el comando
            reader = query.ExecuteReader();//Alamcenamos los datos en el lector para despues pasarlos al DataTable
            tabla.Load(reader);//Cargamos los datos del lector de la tabla
            reader.Close();//Siempre cerrar el lector
            query.Dispose();//Destruir siempre comando MySQl una vez usado
            miConexionSQL.Close();



            if (tabla.Rows.Count > 0)
            {
                Libro libro = null;
                var trc = tabla.Rows.Count;
                var tr = tabla.Rows;
                bool flag = false;
                string idtemp = "idtemp";
                for (int j = 1; j <= trc; j++)
                {
                    for (int i = 0; i < trc - j; i++)
                    {

                        if (tr[i]["idLibro"].ToString() == tr[trc - j]["idLibro"].ToString())
                        {
                            libro = new Libro();
                            libro.genero = tr[trc - j]["genero"].ToString();
                            libro.idLibro = int.Parse(tr[trc - j]["idLibro"].ToString());
                            libro.nombreLibro = tr[trc - j]["nombreLibro"].ToString();
                            libro.portada = tr[trc - j]["portada"].ToString();
                            libro.autorEnsayo = tr[trc - j]["autorEnsayo"].ToString();
                            libro.autorLibro = tr[trc - j]["nombreAutor"].ToString() + " " + tr[trc - j]["apellidoAutor"].ToString() + ", " + tr[i]["nombreAutor"].ToString() + " " + tr[i]["apellidoAutor"].ToString();

                            ListaDeLibros.Add(libro);
                            flag = true;
                            idtemp = tr[i]["idLibro"].ToString();
                        }
                        else { flag = false; }

                    }

                    if (!flag)
                    {
                        if (tr[trc - j]["idLibro"].ToString() != idtemp)
                        {
                            libro = new Libro();
                            libro.genero = tr[trc - j]["genero"].ToString();
                            libro.idLibro = int.Parse(tr[trc - j]["idLibro"].ToString());
                            libro.nombreLibro = tr[trc - j]["nombreLibro"].ToString();
                            libro.portada = tr[trc - j]["portada"].ToString();
                            libro.autorEnsayo = tr[trc - j]["autorEnsayo"].ToString();
                            libro.autorLibro = tr[trc - j]["nombreAutor"].ToString() + " " + tr[trc - j]["apellidoAutor"].ToString();

                            ListaDeLibros.Add(libro);
                        }
                    }

                }
                ListaDeLibros = ListaDeLibros.OrderBy(x => x.nombreLibro).ToList();
                string json = JsonConvert.SerializeObject(ListaDeLibros);

                return json;
            }
        }
        return null;
    }

    [WebMethod]
    public ContenidoLibro SetLibro(string idLibro_set)
    {

        MySqlConnection miConexionSQL = new MySqlConnection(Conexion.ObtenerCadenaConexion());
        if (miConexionSQL.State == System.Data.ConnectionState.Closed)
            miConexionSQL.Open();
        if (miConexionSQL.State == System.Data.ConnectionState.Open)
        {
            MySqlCommand query = new MySqlCommand
                ("select libros.idLibro, libros.nombreLibro, libros.linkCompra, autores.nombreAutor, autores.apellidoAutor, libros.genero, libros. editorial, libros.`añoPublicacion`, libros.sinopsis, libros.autorEnsayo, libros.ensayo, libros.portada from libros, autores, autorlibro where libros.idLibro = autorlibro.idLibro and autorlibro.idAutor = autores.idAutor and libros.idLibro =" + idLibro_set + ";", miConexionSQL);
            query.CommandType = System.Data.CommandType.Text;
            query.CommandTimeout = 120;
            MySqlDataReader reader = null;
            DataTable tabla = new DataTable();

            /*DataTable toma la primera columna de la tabla como primary key por defecto, asi que no van a salir libros
            repetidos porque el idLibro no se puede repetir. El libro que tiene dos autores necesitamos que salga dos veces
             para capturar ambos autores, asi que se agrega una nueva columna autoincrementable y a esa columna se le asigna
             el primary key*/

            tabla.Columns.Add(new System.Data.DataColumn("PK", typeof(System.Int32))); //agregar una nueva columna llamada PK
            tabla.PrimaryKey = new DataColumn[1] { tabla.Columns["PK"] }; //asignar nueva columna como primary key
            tabla.Columns["PK"].AutoIncrement = true; //autoincrement en nueva columna


            query.Prepare();
            reader = query.ExecuteReader();
            tabla.Load(reader);
            reader.Close();
            query.Dispose();
            miConexionSQL.Close();

            if (tabla.Rows.Count > 0)
            {
                ContenidoLibro respuesta = new ContenidoLibro();
                respuesta.IDLibro = tabla.Rows[0]["idLibro"].ToString();
                respuesta.Titulo = tabla.Rows[0]["nombreLibro"].ToString();
                respuesta.Autor = tabla.Rows[0]["nombreAutor"].ToString() + " " + tabla.Rows[0]["apellidoAutor"].ToString();
                respuesta.Genero = tabla.Rows[0]["genero"].ToString();
                respuesta.Editorial = tabla.Rows[0]["editorial"].ToString();
                respuesta.Publicacion = tabla.Rows[0]["añoPublicacion"].ToString();
                respuesta.Sinopsis = tabla.Rows[0]["sinopsis"].ToString();
                respuesta.AutorEnsayo = tabla.Rows[0]["autorEnsayo"].ToString();
                respuesta.Ensayo = tabla.Rows[0]["ensayo"].ToString();
                respuesta.Portada = tabla.Rows[0]["portada"].ToString();
                respuesta.Compra = tabla.Rows[0]["linkCompra"].ToString();

                //for para agregar mas autores a un libro en el caso de que este tenga mas de un autor.
                if (tabla.Rows.Count > 1)
                {
                    for (int i = 1; i < tabla.Rows.Count; i++)
                    {
                        respuesta.Autor += ", " + tabla.Rows[i]["nombreAutor"].ToString() + " " + tabla.Rows[i]["apellidoAutor"].ToString();
                    }
                }

                return respuesta;
            }
        }

        return null;
    }

    [WebMethod]

    public string BuscarLibro(string busqueda)
    {
        List<Libro> ListaDeLibros = new List<Libro>();
        MySqlConnection miConexionSQL = new MySqlConnection(Conexion.ObtenerCadenaConexion());

        if (miConexionSQL.State == System.Data.ConnectionState.Closed)
            miConexionSQL.Open();
        if (miConexionSQL.State == System.Data.ConnectionState.Open)
        {

            MySqlCommand query = new MySqlCommand("SELECT * from (select libros.idLibro, libros.nombreLibro, libros.portada, libros.autorEnsayo, libros.genero, autores.nombreAutor, autores.apellidoAutor from libros, autores, autorlibro where libros.nombreLibro LIKE '%" + busqueda + "%' and autorlibro.idLibro = libros.idLibro and autorlibro.idAutor = autores.idAutor UNION select libros.idLibro, libros.nombreLibro, libros.portada, libros.autorEnsayo,  libros.genero, autores.nombreAutor, autores.apellidoAutor from libros, autores, autorlibro where autores.nombreAutor  LIKE '%" + busqueda + "%' and autorlibro.idLibro = libros.idLibro and autorlibro.idAutor = autores.idAutor  UNION select libros.idLibro, libros.nombreLibro, libros.portada, libros.autorEnsayo, libros.genero, autores.nombreAutor,  autores.apellidoAutor from libros, autores, autorlibro where autores.apellidoAutor LIKE '%" + busqueda + "%' and autorlibro.idLibro = libros.idLibro  and autorlibro.idAutor = autores.idAutor UNION select libros.idLibro, libros.nombreLibro, libros.portada, libros.autorEnsayo, libros.genero, autores.nombreAutor,  autores.apellidoAutor from libros, autores, autorlibro where libros.autorEnsayo LIKE '%" + busqueda + "%' and autorlibro.idLibro = libros.idLibro  and autorlibro.idAutor = autores.idAutor) a order by idLibro", miConexionSQL);
            query.CommandType = System.Data.CommandType.Text;
            query.CommandTimeout = 120; //120 segundos
            MySqlDataReader reader = null;
            DataTable tabla = new DataTable(); //Tabla para almacenar los datos temporales

            /*DataTable toma la primera columna de la tabla como primary key por defecto, asi que no van a salir libros
            repetidos porque el idLibro no se puede repetir. El libro que tiene dos autores necesitamos que salga dos veces
             para capturar ambos autores, asi que se agrega una nueva columna autoincrementable y a esa columna se le asigna
             el primary key*/

            tabla.Columns.Add(new System.Data.DataColumn("PK", typeof(System.Int32))); //agregar una nueva columna llamada PK
            tabla.PrimaryKey = new DataColumn[1] { tabla.Columns["PK"] }; //asignar nueva columna como primary key
            tabla.Columns["PK"].AutoIncrement = true; //autoincrement en nueva columna

            query.Prepare();//Inicializamos el comando
            reader = query.ExecuteReader();//Alamcenamos los datos en el lector para despues pasarlos al DataTable
            tabla.Load(reader);//Cargamos los datos del lector de la tabla
            reader.Close();//Siempre cerrar el lector
            query.Dispose();//Destruir siempre comando MySQl una vez usado
            miConexionSQL.Close();



            if (tabla.Rows.Count > 0)
            {
                Libro libro = null;
                var trc = tabla.Rows.Count;
                var tr = tabla.Rows;
                bool flag = false;
                string idtemp = "idtemp";
                for (int j = 1; j <= trc; j++)
                {
                    for (int i = 0; i < trc - j; i++)
                    {
                     
                        if (tr[i]["idLibro"].ToString() == tr[trc - j]["idLibro"].ToString())
                        {
                            libro = new Libro();
                            libro.genero = tr[trc - j]["genero"].ToString();
                            libro.idLibro = int.Parse(tr[trc - j]["idLibro"].ToString());
                            libro.nombreLibro = tr[trc - j]["nombreLibro"].ToString();
                            libro.portada = tr[trc - j]["portada"].ToString();
                            libro.autorEnsayo = tr[trc - j]["autorEnsayo"].ToString();
                            libro.autorLibro = tr[trc - j]["nombreAutor"].ToString() + " " + tr[trc - j]["apellidoAutor"].ToString() + ", " + tr[i]["nombreAutor"].ToString() + " " + tr[i]["apellidoAutor"].ToString();

                            ListaDeLibros.Add(libro);
                            flag = true;
                            idtemp = tr[i]["idLibro"].ToString();
                        }
                    else {flag = false;}

                    }

                    if (!flag)
                    {
                        if(tr[trc - j]["idLibro"].ToString() != idtemp)
                        {
                            libro = new Libro();
                            libro.genero = tr[trc - j]["genero"].ToString();
                            libro.idLibro = int.Parse(tr[trc - j]["idLibro"].ToString());
                            libro.nombreLibro = tr[trc - j]["nombreLibro"].ToString();
                            libro.portada = tr[trc - j]["portada"].ToString();
                            libro.autorEnsayo = tr[trc - j]["autorEnsayo"].ToString();
                            libro.autorLibro = tr[trc - j]["nombreAutor"].ToString() + " " + tr[trc - j]["apellidoAutor"].ToString();

                            ListaDeLibros.Add(libro);
                        }
                    }

                }
                ListaDeLibros = ListaDeLibros.OrderBy(x => x.nombreLibro).ToList();
                string json = JsonConvert.SerializeObject(ListaDeLibros);

                return json;
            }
        }

        return null;
    }



    [WebMethod]
    public bool GuardarComentario(string ObjetoJSONComentario)
    {
        var resultado = JsonConvert.DeserializeObject<Comentario>(ObjetoJSONComentario);

        MySqlConnection miConexionSQL = new MySqlConnection(Conexion.ObtenerCadenaConexion());
        if (miConexionSQL.State == System.Data.ConnectionState.Closed)
            miConexionSQL.Open();
        if (miConexionSQL.State == System.Data.ConnectionState.Open)
        {
            MySqlCommand query = new MySqlCommand
                ("INSERT INTO " +
                "comentarios(idLibro,comentario)" +
                "VALUES ('" + resultado.idLibro + "', " + "'" + resultado.Com + "');",
            miConexionSQL);
            query.CommandType = System.Data.CommandType.Text;
            query.CommandTimeout = 120;
            query.ExecuteNonQuery();
            query.Dispose();
            miConexionSQL.Close();
        }

        return true;
    }



}

