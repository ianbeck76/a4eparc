using A4EPARC.Enums;
using A4EPARC.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace A4EPARC.Services
{
    public class ResultService : IResultService
    {
        public ResultViewModel CalculateDecision(ClientViewModel viewmodel, string answerString)
        {
            var actionScore = viewmodel.Questions.Where(r => r.ActionTypeId == 1).Select(r => r.Answer).Sum();
            var preContemplationScore = viewmodel.Questions.Where(r => r.ActionTypeId == 3).Select(r => r.Answer).Sum();
            var contemplationScore = viewmodel.Questions.Where(r => r.ActionTypeId == 2).Select(r => r.Answer).Sum();

            //Convert to total scores
            var ta = 50 + 10 * (actionScore - 13.2000) / 4.7460;
            var tp = 50 + 10 * (preContemplationScore - 7.4258) / 2.8306;
            var tc = 50 + 10 * (contemplationScore - 16.5484) / 2.5538;

            var reldist2 = Math.Pow((65.0558 - tp.GetValueOrDefault()), 2) + Math.Pow((41.7343 - tc.GetValueOrDefault()), 2) + Math.Pow((42.7966 - ta.GetValueOrDefault()), 2);
            var nradist2 = Math.Pow((60.0000 - tp.GetValueOrDefault()), 2) + Math.Pow((40.0000 - tc.GetValueOrDefault()), 2) + Math.Pow((60.0000 - ta.GetValueOrDefault()), 2);
            var refdist2 = Math.Pow((47.1980 - tp.GetValueOrDefault()), 2) + Math.Pow((50.1701 - tc.GetValueOrDefault()), 2) + Math.Pow((41.6665 - ta.GetValueOrDefault()), 2);
            var pardist2 = Math.Pow((45.3448 - tp.GetValueOrDefault()), 2) + Math.Pow((53.4616 - tc.GetValueOrDefault()), 2) + Math.Pow((58.6332 - ta.GetValueOrDefault()), 2);

            var distmin2a = Math.Min(reldist2, nradist2);
            var distmin2b = Math.Min(refdist2, pardist2);
            var distmin2 = Math.Min(distmin2a, distmin2b);

            var result = new ResultViewModel();
            result.ClientId = viewmodel.Id;
            result.ActionScore = actionScore.GetValueOrDefault();
            result.PreContemplationScore = preContemplationScore.GetValueOrDefault();
            result.ContemplationScore = contemplationScore.GetValueOrDefault();
            result.ActionScoreMatrix = (int)ta;
            result.PreContemplationScoreMatrix = (int)tp;
            result.ContemplationScoreMatrix = (int)tc;
            result.AnswerString = answerString;

            if (distmin2 == reldist2)
            {
                //stage 1 reluctant - precontemplation
                result.ActionIdToDisplay = ActionType.Precontemplation;
            }
            if (distmin2 == nradist2)
            {
                // stage 2 superficial action - Unauthentic Action
                result.ActionIdToDisplay = ActionType.UnauthenticAction;
            }
            if (distmin2 == refdist2)
            {
                // stage 3  Reflective = Contemplation
                result.ActionIdToDisplay = ActionType.Contemplation;
            }
            if (distmin2 == pardist2)
            {
                // stage 4 - if matrix score is greater than 44 Action else Preparation
                result.ActionIdToDisplay = result.ActionScoreMatrix > 55 ? ActionType.Action : ActionType.Preparation;
            }

            return result;
        }

        //Use this and remove above method if given to clients...
        public ResultViewModel GetDecision(ClientViewModel viewmodel, string answerString)
        {
            var url = string.Format("http://psychass.com/SOCActionService.svc/Get?key={0}&answers={1}", WebConfigurationManager.AppSettings["WebServiceKey"], answerString);

            var request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                var response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream, Encoding.UTF8);
                    var stream = reader.ReadToEnd();
                    var jsonObject = JObject.Parse(stream);

                    var jsonResult = JsonConvert.DeserializeObject<JsonResultModel>(jsonObject.ToString());

                    var result = new ResultViewModel
                    {
                        ClientId = viewmodel.Id,
                        ActionScore =
                            viewmodel.Questions.Where(r => r.ActionTypeId == 1)
                                     .Select(r => r.Answer.GetValueOrDefault())
                                     .Sum(),
                        PreContemplationScore =
                            viewmodel.Questions.Where(r => r.ActionTypeId == 3)
                                     .Select(r => r.Answer.GetValueOrDefault())
                                     .Sum(),
                        ContemplationScore =
                            viewmodel.Questions.Where(r => r.ActionTypeId == 2)
                                     .Select(r => r.Answer.GetValueOrDefault())
                                     .Sum(),
                        ActionScoreMatrix = Convert.ToInt32(jsonResult.GetResult.ActionScore),
                        PreContemplationScoreMatrix = Convert.ToInt32(jsonResult.GetResult.PreContemplationScore),
                        ContemplationScoreMatrix = Convert.ToInt32(jsonResult.GetResult.ContemplationScore),
                        AnswerString = answerString
                    };
                    switch (jsonResult.GetResult.Result)
                    {
                        case "PreContemplation":
                            {
                                result.ActionIdToDisplay = ActionType.Precontemplation;
                                break;
                            }
                        case "UnreflectiveAction":
                            {
                                result.ActionIdToDisplay = ActionType.UnauthenticAction;
                                break;
                            }
                        case "Contemplation":
                            {
                                result.ActionIdToDisplay = ActionType.Contemplation;
                                break;
                            }
                        case "Preparation":
                            {
                                result.ActionIdToDisplay = ActionType.Preparation;
                                break;
                            }
                        case "Action":
                            {
                                result.ActionIdToDisplay = ActionType.Action;
                                break;
                            }
                    }
                    return result;
                }
            }
            catch (WebException ex)
            {
                var errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    var errorText = reader.ReadToEnd();
                    // log errorText
                }
            }

            //action = 1, contemplation = 2, precontemplation = 3, preparer = 4, unauthenticaction = 5

            return new ResultViewModel { ActionIdToDisplay = ActionType.Undefined };
        }

    }

    public interface IResultService 
    {
        ResultViewModel CalculateDecision(ClientViewModel viewmodel, string answerString);

        ResultViewModel GetDecision(ClientViewModel viewmodel, string answerString);
        
    }
}