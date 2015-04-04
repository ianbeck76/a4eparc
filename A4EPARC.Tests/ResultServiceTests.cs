using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using A4EPARC.Services;
using Moq;
using A4EPARC.ViewModels;
using System.Collections.Generic;

namespace A4EPARC.Tests
{
    [TestClass]
    public class ResultServiceTests
    {
        private ClientViewModel model;
        private string answerstring;

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
        public void ContemplationTest()
        {
            answerstring = "5,1,5,1,5,1,5,1,5,1,5,1";
            var resultService = new ResultService();
            var result = resultService.CalculateDecision(model, answerstring);
            Assert.IsTrue(result.ActionIdToDisplay == Enums.ActionType.Contemplation);
        }

        [TestMethod]
        public void PreContemplationTest()
        {
            answerstring = "5,4,1,4,5,1,5,1,5,4,1,4";
            var resultService = new ResultService();
            var result = resultService.CalculateDecision(model, answerstring);
            Assert.IsTrue(result.ActionIdToDisplay == Enums.ActionType.UnauthenticAction);
        }

        [TestMethod]
        public void UnauthenticActionTest()
        {
            answerstring = "5,5,5,5,5,5,5,5,5,5,5,5";
            var resultService = new ResultService();
            var result = resultService.CalculateDecision(model, answerstring);
            Assert.IsTrue(result.ActionIdToDisplay == Enums.ActionType.UnauthenticAction);
        }
    }
}
