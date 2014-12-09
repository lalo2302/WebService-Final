using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ContenidoLibro
/// </summary>
public class ContenidoLibro
{
	public ContenidoLibro()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string IDLibro { set; get; }
    public string Titulo { set; get; }
    public string Autor { set; get; }
    public string Genero { set; get; }
    public string Editorial { set; get; }
    public string Publicacion { set; get; }
    public string Sinopsis { set; get; }
    public string AutorEnsayo { set; get; }
    public string Ensayo { set; get; }
    public string Portada { set; get; }
    public string Compra { set; get; }
}