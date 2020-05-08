using AdaptiveCards;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Reflection.Model;
using System;
using System.Collections.Generic;

namespace Reflection.Helper
{
    public class CardHelper
    {
        private readonly IConfiguration _configuration;
        public CardHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AdaptiveCard FeedBackCard(Dictionary<int,int> keyValues, Guid reflectionId)
        {
            for(int i=1;i<=5;i++)
            {
                if(!keyValues.ContainsKey(i))
                {
                    keyValues.Add(i, 0);
                }
            }

            return new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveImage() { Url = new Uri(_configuration["BaseUri"] + "/images/Firstresponsecolor.png") },
                    new AdaptiveColumnSet
                    {
                        Columns = new List<AdaptiveColumn>()
                        {
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage() { Size = AdaptiveImageSize.Small, Url = new Uri(_configuration["BaseUri"] + "/images/ref1.png"),
                                        Style = AdaptiveImageStyle.Default, Id = "1" }
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Stretch,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[1].ToString())
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage() { Size = AdaptiveImageSize.Small, Url = new Uri(_configuration["BaseUri"] + "/images/ref2.png"),
                                        Style = AdaptiveImageStyle.Default, Id = "2" }
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Stretch,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[2].ToString())
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage() { Size = AdaptiveImageSize.Small, Url = new Uri(_configuration["BaseUri"] + "/images/ref3.png"),
                                        Style = AdaptiveImageStyle.Default, Id = "3" }
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Stretch,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[3].ToString())
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage() { Size = AdaptiveImageSize.Small, Url = new Uri(_configuration["BaseUri"] + "/images/ref4.png"),
                                        Style = AdaptiveImageStyle.Default, Id = "4" }
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Stretch,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[4].ToString())
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage() { Size = AdaptiveImageSize.Small, Url = new Uri(_configuration["BaseUri"] + "/images/ref5.png"),
                                        Style = AdaptiveImageStyle.Default, Id = "5" }
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Stretch,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[5].ToString())
                                }

                            },
                        }
                    }

                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction()
                    {
                        Type = "Action.Submit",
                        Title = "View Reflections",
                        DataJson=@"{'type':'task/fetch','reflectionId':'" + reflectionId +"' }",
                        Data =
                        new TaskModuleActionHelper.AdaptiveCardValue<TaskModuleActionDetails>()
                        {
                            Data = new TaskModuleActionDetails()
                            {
                                type ="task/fetch",
                                URL ="https://bc5066ec.ngrok.io/OpenReflections",
                                Title="View Reflections"
                                
                            }
                        }
                    },
                },
            };
        }

        public  AdaptiveCard CreateNewPostCard(TaskInfo data)
        {
            
            return new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveColumnSet
                    {
                        Columns = new List<AdaptiveColumn>()
                        {
                            new AdaptiveColumn()
                            {
                                Width=AdaptiveColumnWidth.Stretch,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock("Posted by "+ $"{data.postCreateBy}") { Color = AdaptiveTextColor.Good, Size=AdaptiveTextSize.Medium, Spacing=AdaptiveSpacing.Medium},
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width=AdaptiveColumnWidth.Stretch,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock("| Responses are "+ $"{data.recurssionType}") { Color = AdaptiveTextColor.Good, Size=AdaptiveTextSize.Medium, Spacing=AdaptiveSpacing.Medium},
                                }

                            },

                        }
                    },
                    new AdaptiveTextBlock($"{data.question}") { Id = ($"{data.question }"), Weight = AdaptiveTextWeight.Bolder, Size=AdaptiveTextSize.Medium },
                    new AdaptiveColumnSet
                    {
                        Columns = new List<AdaptiveColumn>()
                        {
                             new AdaptiveColumn()
                            {
                                Width=AdaptiveColumnWidth.Auto,
                                Items=new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage(){Size=AdaptiveImageSize.Small,Url=new Uri(_configuration["BaseUri"] + "/images/1.png"),
                                        Style =AdaptiveImageStyle.Person, Id="1", SelectAction = new AdaptiveSubmitAction(){ DataJson = @"{'feedbackId':'1', 'type':'saveFeedback','reflectionId':'" + data.reflectionID +"'}" } }
                                }

                            },
                             new AdaptiveColumn()
                            {
                                Width=AdaptiveColumnWidth.Auto,
                                Items=new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage(){Size=AdaptiveImageSize.Small,Url=new Uri(_configuration["BaseUri"] + "/images/2.png"),
                                        Style =AdaptiveImageStyle.Person, Id="2", SelectAction = new AdaptiveSubmitAction() { DataJson = @"{'feedbackId':'2', 'type':'saveFeedback', 'reflectionId':'" + data.reflectionID +"'}" } }
                                }

                            },
                             new AdaptiveColumn()
                            {
                                Width=AdaptiveColumnWidth.Auto,
                                Items=new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage(){Size=AdaptiveImageSize.Small,Url=new Uri(_configuration["BaseUri"] + "/images/3.png"),
                                        Style =AdaptiveImageStyle.Person, Id="3", SelectAction = new AdaptiveSubmitAction(){ DataJson = @"{'feedbackId':'3', 'type':'saveFeedback', 'reflectionId':'" + data.reflectionID +"'}" } }
                                }

                            },
                             new AdaptiveColumn()
                            {
                                Width=AdaptiveColumnWidth.Auto,
                                Items=new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage(){Size=AdaptiveImageSize.Small,Url=new Uri(_configuration["BaseUri"] + "/images/4.png"),
                                        Style =AdaptiveImageStyle.Person, Id="4", SelectAction = new AdaptiveSubmitAction(){ DataJson = @"{'feedbackId':'4', 'type':'saveFeedback', 'reflectionId':'" + data.reflectionID +"'}" } }
                                }

                            },
                             new AdaptiveColumn()
                            {
                                Width=AdaptiveColumnWidth.Auto,
                                Items=new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage(){Size=AdaptiveImageSize.Small,Url=new Uri(_configuration["BaseUri"] + "/images/5.png"),
                                        Style =AdaptiveImageStyle.Person, Id="5", SelectAction = new AdaptiveSubmitAction(){ DataJson = @"{'feedbackId':'5', 'type':'saveFeedback', 'reflectionId':'" + data.reflectionID +"'}" } }
                               }

                            },
                        }
                    }
                }
            };

        }

    }
}
