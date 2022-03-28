using System;
using Microsoft.AspNetCore.Mvc;
using SMS.Web.Models;
using SMS.Data.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace SMS.Web.Controllers
{
    [Authorize]
    public class TicketController : BaseController
    {
        private readonly IStudentService svc;
        public TicketController()
        {
            svc = new StudentServiceDb();
        }

        // GET /ticket/index
        public IActionResult Index()
        {
            var tickets = svc.GetOpenTickets();
            return View(tickets);
        }
       
        //  POST /ticket/close/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="admin,manager")]
        public IActionResult Close(int id)
        {
            // close ticket via service
            var t = svc.CloseTicket(id);
            if (t == null)
            {
                Alert("No such ticket found", AlertType.warning);            
            }

            Alert($"Ticket {id } closed", AlertType.info);  
            // redirect to the index view
            return RedirectToAction(nameof(Index));
        }
       
        // GET /ticket/create
        [Authorize(Roles="admin,manager")]
        public IActionResult Create()
        {
            var students = svc.GetStudents();
            //populate the viewmodel with the student selectlist 
            var tvm = new TicketViewModel {
                Students = new SelectList(students,"Id","Name") 
            };
            
            // render blank form
            return View( tvm );
        }
       
        // POST /ticket/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="admin,manager")]
        public IActionResult Create([Bind("StudentId,Issue")] TicketViewModel tvm)
        {            
            if (ModelState.IsValid)
            {
                var ticket = svc.CreateTicket(tvm.StudentId, tvm.Issue);

                // example showing how to alert user when service returns error
                if (ticket != null) {
                    Alert($"Ticket Created", AlertType.info);  
                } else {
                    Alert($"Problem creating Ticket", AlertType.warning);
                }
                return RedirectToAction(nameof(Index));
            }

            // redisplay the form for editing
            return View(tvm);
        }
    }
}
