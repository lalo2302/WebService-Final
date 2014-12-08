using System;
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
    public string HelloWorld()
    {
        return "Hello World";
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

                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    if (i > 2 && i < tabla.Rows.Count - 2)
                    {
                        if (tabla.Rows[i]["idLibro"].ToString() == tabla.Rows[i + 1]["idLibro"].ToString())
                        {
                            libro = new Libro();
                            libro.genero = tabla.Rows[i]["genero"].ToString();
                            libro.idLibro = int.Parse(tabla.Rows[i]["idLibro"].ToString());
                            libro.nombreLibro = tabla.Rows[i]["nombreLibro"].ToString();
                            libro.portada = tabla.Rows[i]["portada"].ToString();
                            libro.autorEnsayo = tabla.Rows[i]["autorEnsayo"].ToString();
                            libro.autorLibro = tabla.Rows[i]["nombreAutor"].ToString() + " " + tabla.Rows[i]["apellidoAutor"].ToString() + ", " + tabla.Rows[i + 1]["nombreAutor"].ToString() + " " + tabla.Rows[i + 1]["apellidoAutor"].ToString();

                            ListaDeLibros.Add(libro);
                        }
                        else if (tabla.Rows[i]["idLibro"].ToString() == tabla.Rows[i - 1]["idLibro"].ToString())
                        {
                            libro = null;
                        }
                        else
                        {
                            libro = new Libro();
                            libro.genero = tabla.Rows[i]["genero"].ToString();
                            libro.idLibro = int.Parse(tabla.Rows[i]["idLibro"].ToString());
                            libro.nombreLibro = tabla.Rows[i]["nombreLibro"].ToString();
                            libro.portada = tabla.Rows[i]["portada"].ToString();
                            libro.autorEnsayo = tabla.Rows[i]["autorEnsayo"].ToString();
                            libro.autorLibro = tabla.Rows[i]["nombreAutor"].ToString() + " " + tabla.Rows[i]["apellidoAutor"].ToString();

                            ListaDeLibros.Add(libro);
                        }
                    }
                    else
                    {
                        libro = new Libro();
                        libro.genero = tabla.Rows[i]["genero"].ToString();
                        libro.idLibro = int.Parse(tabla.Rows[i]["idLibro"].ToString());
                        libro.nombreLibro = tabla.Rows[i]["nombreLibro"].ToString();
                        libro.portada = tabla.Rows[i]["portada"].ToString();
                        libro.autorEnsayo = tabla.Rows[i]["autorEnsayo"].ToString();
                        libro.autorLibro = tabla.Rows[i]["nombreAutor"].ToString() + " " + tabla.Rows[i]["apellidoAutor"].ToString();

                        ListaDeLibros.Add(libro);
                    }
                }
            }
        }
        ListaDeLibros = ListaDeLibros.OrderBy(x => x.nombreLibro).ToList();
        string json = JsonConvert.SerializeObject(ListaDeLibros);

        return json;
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
                ("select libros.idLibro, libros.nombreLibro, autores.nombreAutor, autores.apellidoAutor, libros.genero, libros. editorial, libros.`añoPublicacion`, libros.sinopsis, libros.autorEnsayo, libros.ensayo, libros.portada from libros, autores, autorlibro where libros.idLibro = autorlibro.idLibro and autorlibro.idAutor = autores.idAutor and libros.idLibro =" + idLibro_set + ";", miConexionSQL);
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



}

