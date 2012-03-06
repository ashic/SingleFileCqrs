using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeManager.Domain;
using EmployeeManager.Framework;
using EmployeeManager.ReadModel;
using Raven.Client;
using EmployeeManager.Code;
using System.Web.Security;

namespace EmployeeManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICommandService _service;
        private readonly IDocumentStore _store;

        public HomeController():this(Configuration.CommandService, Configuration.DocumentStore)
        {
            
        }

        public HomeController(ICommandService service, IDocumentStore store)
        {
            _service = service;
            _store = store;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEmployee(CreateEmployeeCommand command)
        {
            try
            {
                command.EmployeeId = Guid.NewGuid();
                _service.Handle(command);
                return Json(new { Succeeded = true });
            }
            catch (Exception ex)
            {
                return
                    Json(
                        new
                        {
                            Succeeded = false,
                            ErrorMessage = string.Format("Something went bad...oops: {0}", ex.Message)
                        });
            }
        }

        [HttpPost]
        public ActionResult ChangeEmployeeName(ChangeEmployeeNameCommand command)
        {
            try
            {
                _service.Handle(command);
                return Json(new { Succeeded = true });
            }
            catch (Exception ex)
            {
                return
                    Json(
                        new
                        {
                            Succeeded = false,
                            ErrorMessage = string.Format("Something went bad...oops: {0}", ex.Message)
                        });
            }
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            using (var session = _store.OpenSession())
            {
                var items = session.Query<EmployeeListItem>().ToList();

                return Json(items, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetEmployee(string id)
        {
            using (var session = _store.OpenSession())
            {
                var item = session.Query<EmployeeDetails>().First(x=>x.EmployeeId == id);

                return Json(item, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
