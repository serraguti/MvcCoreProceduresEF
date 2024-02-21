using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;

#region VIEWS
//create view V_EMP_DEPT
//as
//	select CAST(
//	ISNULL(ROW_NUMBER() OVER (ORDER BY APELLIDO), 0) AS INT)
//	AS ID
//	, EMP.APELLIDO, EMP.OFICIO
//	, DEPT.DNOMBRE AS DEPARTAMENTO
//	, DEPT.LOC AS LOCALIDAD
//	from EMP
//	inner join DEPT
//	on EMP.DEPT_NO = DEPT.DEPT_NO
//go
#endregion

namespace MvcCoreProceduresEF.Repositories
{
    public class RepositoryViewEmpleados
    {
        private HospitalContext context;

        public RepositoryViewEmpleados(HospitalContext context)
        {
            this.context = context;
        }

        //REALIZAMOS LA PETICION A LA VISTA DE FORMA ASINCRONA.
        //TENEMOS UN METODO LLAMADO ToListAsync() DENTRO DE EF
        //QUE NOS DEVUELVE LAS LISTAS de var consulta DE FORMA ASINCRONA
        public async Task<List<ViewEmpleado>> GetEmpleadosAsync()
        {
            var consulta = from datos in this.context.ViewEmpleados
                           select datos;
            return await consulta.ToListAsync();
        }
    }
}
