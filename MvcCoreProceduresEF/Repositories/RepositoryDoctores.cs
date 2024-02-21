using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System.Data;
using System.Data.Common;

#region PROCEDIMIENTOS ALMACENADOS
//create procedure SP_TODOS_DOCTORES
//as
//	select * from DOCTOR
//go
//create procedure SP_ESPECIALIDADES
//as
//	select distinct ESPECIALIDAD from DOCTOR
//go
//create procedure SP_DOCTORES_ESPECIALIDAD
//(@especialidad nvarchar(50))
//as
//	select * from DOCTOR
//	where ESPECIALIDAD=@especialidad
//go
//create procedure SP_UPDATE_SALARIO_ESPECIALIDAD
//(@especialidad nvarchar(50), @incremento int)
//as
//	update DOCTOR set SALARIO = SALARIO + @incremento
//	where ESPECIALIDAD=@especialidad
//go


#endregion

namespace MvcCoreProceduresEF.Repositories
{
    public class RepositoryDoctores
    {
        private HospitalContext context;

        public RepositoryDoctores(HospitalContext context)
        {
            this.context = context;
        }

        public List<Doctor> GetDoctores()
        {
            string sql = "SP_TODOS_DOCTORES";
            var consulta = this.context.Doctores.FromSqlRaw(sql);
            List<Doctor> doctores = consulta.AsEnumerable().ToList();
            return doctores;
        }

        public List<Doctor> GetDoctoresEspecialidad(string especialidad)
        {
            string sql = "SP_DOCTORES_ESPECIALIDAD @especialidad";
            SqlParameter pamEspe = new SqlParameter("@especialidad", especialidad);
            var consulta = this.context.Doctores.FromSqlRaw(sql, pamEspe);
            List<Doctor> doctores = consulta.AsEnumerable().ToList();
            return doctores;
        }

        public List<string> GetEspecialidades()
        {
            string sql = "SP_ESPECIALIDADES";
            using (DbCommand com =
                this.context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = sql;
                com.Connection.Open();
                DbDataReader reader = com.ExecuteReader();
                List<string> especialidades = new List<string>();
                while (reader.Read())
                {
                    string espe = reader["ESPECIALIDAD"].ToString();
                    especialidades.Add(espe);
                }
                reader.Close();
                com.Connection.Close();
                return especialidades;
            }
        }

        public void IncrementarSalarioDoctoresEspecialidad
            (string especialidad, int incremento)
        {
            string sql = "SP_UPDATE_SALARIO_ESPECIALIDAD @especialidad, @incremento";
            SqlParameter pamEspe = new SqlParameter("@especialidad", especialidad);
            SqlParameter pamInc = new SqlParameter("@incremento", incremento);
            this.context.Database.ExecuteSqlRaw(sql, pamEspe, pamInc);
        }
    }
}
