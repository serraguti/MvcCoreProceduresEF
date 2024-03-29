﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System.Data;
using System.Data.Common;

#region PROCEDIMIENTOS ALMACENADOS
//create procedure SP_TODOS_ENFERMOS
//as
//	select * from ENFERMO
//go
//create procedure SP_FIND_ENFERMO
//(@inscripcion int)
//as
//	select * from ENFERMO
//	where INSCRIPCION=@inscripcion
//go
//create procedure SP_DELETE_ENFERMO
//(@inscripcion int)
//as
//	delete from ENFERMO
//	where INSCRIPCION=@inscripcion
//go
//create procedure SP_INSERT_ENFERMO
//(@apellido nvarchar(50), @direccion nvarchar(100)
//, @fechanacimiento datetime, @genero nvarchar(10))
//as
//	declare @maxinscripcion int
//	select @maxinscripcion = max(INSCRIPCION) + 1 from ENFERMO
//	insert into ENFERMO values (@maxinscripcion, @apellido
//    , @direccion, @fechanacimiento, @genero, 7888)
//go
#endregion

namespace MvcCoreProceduresEF.Repositories
{
    public class RepositoryEnfermos
    {
        private HospitalContext context;

        public RepositoryEnfermos(HospitalContext context)
        {
            this.context = context;
        }

        public List<Enfermo> GetEnfermos()
        {
            //PARA CONSULTAS DE SELECCION CON LOS PROCEDIMIENTOS 
            //ALMACENADOS, DEBEMOS MAPEAR MANUALMENTE LOS DATOS 
            //QUE RECIBIMOS
            using (DbCommand com =
                this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_TODOS_ENFERMOS";
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = sql;
                //ABRIMOS LA CONEXION A TRAVES DEL COMANDO
                com.Connection.Open();
                //EJECUTAMOS NUESTRO READER
                DbDataReader reader = com.ExecuteReader();
                List<Enfermo> enfermos = new List<Enfermo>();
                while (reader.Read())
                {
                    Enfermo enfermo = new Enfermo
                    {
                        Inscripcion = int.Parse(reader["INSCRIPCION"].ToString()),
                        Apellido = reader["APELLIDO"].ToString(),
                        Direccion = reader["DIRECCION"].ToString(),
                        FechaNacimiento = DateTime.Parse(reader["FECHA_NAC"].ToString()),
                        Genero = reader["S"].ToString()
                    };
                    enfermos.Add(enfermo);
                }
                reader.Close();
                com.Connection.Close();
                return enfermos;
            }
        }

        public Enfermo FindEnfermo(int inscripcion)
        {
            //PARA LLAMAR A PROCEDIMIENTOS CON PARAMETROS
            //LA LLAMADA SE REALIZA INCLUYENDO LOS PARAMETROS
            //Y TAMBIEN EL NOMBRE DEL PROCEDURE:
            // SP_NOMBREPROCEDIMIENTO @param1, @param2
            string sql = "SP_FIND_ENFERMO @inscripcion";
            //PARA DECLARAR PARAMETROS SE UTILIZA LA CLASE SqlParameter
            //DEBEMOS TENER CUIDADO CON EL NAMESPACE
            //EL NAMESPACE ES Microsoft.Data
            SqlParameter pamInscripcion =
                new SqlParameter("@inscripcion", inscripcion);
            //AL SER UN PROCEDIMIENTO SELECT, PUEDO UTILIZAR 
            //EL METODO FromSqlRaw PARA EXTRAER LOS DATOS
            //SI MI CONSULTA COINCIDE CON UN MODEL, PUEDO UTILIZAR 
            //LINQ PARA MAPEAR LOS DATOS.
            //CUANDO TENEMOS UN PROCEDURE SELECT, LAS PETICIONES SE 
            //DIVIDEN EN DOS.  NO PUEDO HACER LINQ Y DESPUES UN foreach
            //DEBEMOS EXTRAER LOS DATOS EN DOS ACCIONES
            var consulta = this.context.Enfermos.FromSqlRaw(sql, pamInscripcion);
            //EXTRAER LAS ENTIDADES DE LA CONSULTA (EJECUTAR)
            //PARA EJECUTAR, NECESITAMOS AsEnumerable()
            Enfermo enfermo = consulta.AsEnumerable().FirstOrDefault();
            return enfermo;
        }

        public void DeleteEnfermo(int inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO @inscripcion";
            SqlParameter pamInscripcion =
                new SqlParameter("@inscripcion", inscripcion);
            //EJECUTAR CONSULTAS DE ACCION SE REALIZA MEDIANTE 
            //EL METODO ExecuteSqlRaw() QUE SE ACCEDE DESDE 
            //Database DENTRO DEL DbContext
            this.context.Database.ExecuteSqlRaw(sql, pamInscripcion);
        }

        public void InsertEnfermo(string apellido, string direccion
            , DateTime fechaNacimiento, string genero)
        {
            string sql = "SP_INSERT_ENFERMO @apellido, @direccion, @fechanacimiento, "
                + "@genero";
            SqlParameter pamApellido = new SqlParameter("@apellido", apellido);
            SqlParameter pamDireccion = new SqlParameter("@direccion", direccion);
            SqlParameter pamFecha = new SqlParameter("@fechanacimiento", fechaNacimiento);
            SqlParameter pamGenero = new SqlParameter("@genero", genero);
            this.context.Database.ExecuteSqlRaw(sql, pamApellido, pamDireccion
                , pamFecha, pamGenero);
        }
    }
}
