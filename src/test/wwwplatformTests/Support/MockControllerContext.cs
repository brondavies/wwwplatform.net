﻿using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Moq;

public class MockControllerContext
{
    public Mock<HttpContextBase> HttpContext { get; set; }
    public Mock<HttpRequestBase> Request { get; set; }
    public Mock<HttpResponseBase> Response { get; set; }
    public RouteData RouteData { get; set; }
    public StringBuilder responseBody = new StringBuilder();

    public MockControllerContext(Controller controller, string controllerName, string action)
    {
        //define context objects
        HttpContext = new Mock<HttpContextBase>();
        Request = new Mock<HttpRequestBase>();
        Response = new Mock<HttpResponseBase>();
        HttpContext.Setup(x => x.Request).Returns(Request.Object);
        HttpContext.Setup(x => x.Response).Returns(Response.Object);
        Response.Setup(x => x.Output).Returns(new StringWriter(responseBody));

        //apply context to controller
        RequestContext rc = new RequestContext(HttpContext.Object, new RouteData ());
        rc.RouteData.Values["controller"] = controllerName;
        rc.RouteData.Values["action"] = action;
        controller.ControllerContext = new ControllerContext(rc, controller);
    }
}