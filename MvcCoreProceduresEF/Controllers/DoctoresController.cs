using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repositories;

namespace MvcCoreProceduresEF.Controllers
{
    public class DoctoresController : Controller
    {
        private RepositoryDoctores repo;

        public DoctoresController(RepositoryDoctores repo)
        {
            this.repo = repo;
        }

        public IActionResult DoctoresEspecialidad()
        {
            List<string> especialidades = this.repo.GetEspecialidades();
            List<Doctor> doctores = this.repo.GetDoctores();
            ViewData["ESPECIALIDADES"] = especialidades;
            return View(doctores);
        }

        [HttpPost]
        public IActionResult DoctoresEspecialidad
            (string especialidad, int incremento)
        {
            List<string> especialidades = this.repo.GetEspecialidades();
            ViewData["ESPECIALIDADES"] = especialidades;
            this.repo.IncrementarSalarioDoctoresEspecialidad(especialidad, incremento);
            List<Doctor> doctores = this.repo.GetDoctoresEspecialidad(especialidad);
            return View(doctores);
        }
    }
}
