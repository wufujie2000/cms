﻿using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/error")]
    public class PagesErrorController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var logId = rest.GetQueryInt("logId");

                return Ok(new
                {
                    LogInfo = DataProvider.ErrorLog.GetErrorLogInfo(logId),
                    SystemManager.Version
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}