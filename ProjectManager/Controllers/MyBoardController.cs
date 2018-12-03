﻿
using Newtonsoft.Json;
using ProjectManager.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ProjectManager.Controllers
{
    public class MyBoardController : Controller
    {
        // GET: MyBoard
        Repository<ProjectManager.Models.TaskDetail> td = new Repository<ProjectManager.Models.TaskDetail>();
        Repository<ProjectManager.Models.TaskStatus> s = new Repository<ProjectManager.Models.TaskStatus>();
        Repository<ProjectManager.Models.Tasks> t = new Repository<ProjectManager.Models.Tasks>();
        Repository<ProjectManager.Models.Project> p = new Repository<ProjectManager.Models.Project>();
        public ActionResult Index(Guid id)
        {
            BoardVM VM = new BoardVM();
            var PID = Request.Cookies["PID"].Value;
            var q = from parentTask in t.GetCollections()
                    join childrenTask in t.GetCollections() on parentTask.TaskGUID equals childrenTask.ParentTaskGUID
                    select childrenTask;
            VM.TaskStatus = s.GetCollections();
            VM.Tasks = q.Where(x => x.ProjectGUID.ToString() == PID && x.EmployeeGUID == id).ToList();
            VM.Project = p.GetCollections().Where(x => x.ProjectGUID.ToString() == PID).ToList();
            VM.TaskDetail = td.GetCollections();
            q.ToList();
            return View(VM);
        }
        public ActionResult GetDetailTask(Guid id)
        {
            BoardVM VM = new BoardVM();
            VM.TaskDetail = td.GetCollections().Where(x => x.TaskGUID == id && x.TaskDetailStatusID == 2).ToList();
            return Json(VM.TaskDetail.Count());

        }
        public ActionResult GetCard(Guid id)
        {
            BoardVM VM = new BoardVM();
            VM.Task = t.Find(id);
            return Content(JsonConvert.SerializeObject(VM),"application/Json");

        }
        public ActionResult GetDetail(Guid id)
        {

            BoardVM VM = new BoardVM();
            VM.TaskDetail = td.GetCollections().Where(n => n.TaskGUID == id).ToList();
            return Json(VM.TaskDetail.Select(n => new { n.TaskDetailName, n.TaskDetailGUID, n.TaskDetailStatusID }).ToList());
        }

        //public ActionResult DeleteTaskDatail(Guid id)
        //{
        //    BoardVM VM = new BoardVM();
        //    VM.TaskDetails = td.Find(id);
        //    td.Delete(VM.TaskDetails);
        //    return Json(true);
        //}
        public ActionResult EditTaskStatusID(Guid id, int TaskStatusID)
        {
            BoardVM VM = new BoardVM();
            VM.Task = t.Find(id);
            VM.Task.TaskStatusID = TaskStatusID;
            t.Update(VM.Task);
            return Json(true);
        }
        public ActionResult InsertDetailTask(TaskDetail taskDetail)
        {
            if (taskDetail != null)
            {
                taskDetail.TaskDetailGUID = Guid.NewGuid();
                taskDetail.StartDate = DateTime.Now;
                taskDetail.EndDate = DateTime.Now;
                td.Add(taskDetail);
            }
            return Json(td.GetCollections().Where(x => x.TaskGUID == new Guid(Request.Form["TaskGUID"])).Select(x=>x.TaskDetailGUID).ToList());
        }
        public ActionResult EditDetailStatus(Guid id, int TaskDetailStatusID)
        {
            BoardVM VM = new BoardVM();
            VM.TaskDetails = td.Find(id);
            VM.TaskDetails.TaskDetailStatusID = TaskDetailStatusID;
            td.Update(VM.TaskDetails);
            return Json(true);
        }

        public ActionResult EditTaskDatail(Guid id, string TaskDetailName)
        {
            BoardVM VM = new BoardVM();
            VM.TaskDetails = td.Find(id);
            VM.TaskDetails.TaskDetailName = TaskDetailName;
            td.Update(VM.TaskDetails);
            return Json(true);
        }

        public ActionResult DeleteTaskDatail(Guid id)
        {
            BoardVM VM = new BoardVM();
            VM.TaskDetails = td.Find(id);
            td.Delete(VM.TaskDetails);
            return Json(td.GetCollections().Where(x => x.TaskGUID == new Guid(Request.Form["TaskGUID"])).Count());
        }

        
    }
}