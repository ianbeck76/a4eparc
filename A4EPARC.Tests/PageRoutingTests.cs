using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using A4EPARC.Services;
using Moq;
using A4EPARC.ViewModels;
using System.Collections.Generic;
using A4EPARC.Controllers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using A4EPARC.Repositories;
using System.Web.Hosting;
using System.IO;
using System.Security.Principal;
using A4EPARC.Models;

namespace A4EPARC.Tests
{
    [TestClass]
    public class PageRoutingTests
    {
        private ClientViewModel model;

        [TestInitialize]
        public void SetUp() 
        {
            model = new ClientViewModel { Result = new ResultViewModel(), Questions = new List<QuestionViewModel> 
            { 
                new QuestionViewModel { Code = "1", ActionTypeId = 1, Answer = 1 }, 
                new QuestionViewModel { Code = "2", ActionTypeId = 2, Answer = 2 }, 
                new QuestionViewModel { Code = "3", ActionTypeId = 3, Answer = 3 }, 
                new QuestionViewModel { Code = "4", ActionTypeId = 1, Answer = 1 }, 
                new QuestionViewModel { Code = "5", ActionTypeId = 2, Answer = 2 }, 
                new QuestionViewModel { Code = "6", ActionTypeId = 3, Answer = 3 }, 
                new QuestionViewModel { Code = "7", ActionTypeId = 1, Answer = 1 }, 
                new QuestionViewModel { Code = "8", ActionTypeId = 2, Answer = 2 }, 
                new QuestionViewModel { Code = "9", ActionTypeId = 3, Answer = 3 }, 
                new QuestionViewModel { Code = "10", ActionTypeId = 1, Answer = 1 }, 
                new QuestionViewModel { Code = "11", ActionTypeId = 2, Answer = 2 }, 
                new QuestionViewModel { Code = "12", ActionTypeId = 3, Answer = 3 } 
            } };
        }

        [TestMethod]
        public void PageOne_PageTwo()
        {
            var model = new ClientViewModel();
            // Set some properties here

            var sc = new SurveyController(new ClientRepository(), 
                new QuestionsService(new QuestionsRepository()), 
                new SiteLabelsRepository(), 
                new CompanyRepository(), 
                new ResultService());

            SimpleWorkerRequest request = new SimpleWorkerRequest("","","", null, new StringWriter());
            HttpContext context = new HttpContext(request);

            var fakeIdentity = new GenericIdentity("ianbeck76@gmail.com;ian;1;en-GB");
            var principal = new GenericPrincipal(fakeIdentity, null);

            context.User = principal;

            HttpContext.Current = context;

            var result = sc.PageOne(model);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var viewResult = result as RedirectToRouteResult;
            Assert.IsFalse(sc.ModelState.IsValid);
            Assert.AreEqual("PageTwo", viewResult.RouteValues["action"]);
        }

        [TestMethod]
        public void PageOne_PageTwo_Mock()
        {
            var model = new ClientViewModel();
            // Set some properties here

            SimpleWorkerRequest request = new SimpleWorkerRequest("", "", "", null, new StringWriter());
            HttpContext context = new HttpContext(request);

            var clientRepository = new Mock<IClientRepository>();
            var questionsService = new Mock<IQuestionsService>();
            var siteLabelsRepository = new Mock<ISiteLabelsRepository>();
            var companyRepository = new Mock<ICompanyRepository>();
            var resultService = new Mock<IResultService>();
            
            companyRepository.Setup(c => c.GetSchemes()).Returns(new List<CompanySchemeViewModel> { new CompanySchemeViewModel { SchemeId = 1, CompanyId = 1, SchemeName = "Esher House" }});

            companyRepository.Setup(c => c.GetPageItems()).Returns(new List<CompanyPageItemViewModel> { new CompanyPageItemViewModel { CompanyId = 1, IsDisplay = true, IsRequired = false, Id = 1, Name = "Test" } });

            
            var controller = new SurveyController(
                clientRepository.Object, 
                questionsService.Object, 
                siteLabelsRepository.Object, 
                companyRepository.Object, 
                resultService.Object);

            var fakeIdentity = new GenericIdentity("ianbeck76@gmail.com;1;ian;en-GB");
            var principal = new GenericPrincipal(fakeIdentity, null);

            context.User = principal;

            HttpContext.Current = context;

            var userRepository = new Mock<IRepository<User>>().Object;

            controller.ControllerContext = new ControllerContext(context.Request.RequestContext, controller);

            var result = controller.PageOne(model);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var viewResult = result as RedirectToRouteResult;
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.AreEqual("PageTwo", viewResult.RouteValues["action"]);
        }
    }
}
