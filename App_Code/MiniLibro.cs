using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Libro
/// </summary>
public class Libro
{
	public Libro()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string nombreLibro { set; get; }
    public string portada { set; get; }
    public string autorEnsayo { set; get; }
    public string autorLibro { set; get; }
    public int idLibro { set; get; }
    public string genero { set; get; }
}