using AdaptiveCards;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Reflection.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Reflection.Helper
{
    public class CardHelper
    {
        private readonly IConfiguration _configuration;
        public CardHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AdaptiveCard FeedBackCard(Dictionary<int, List<string>> keyValues, Guid reflectionId)
        {
            DirectoryInfo folderInfo = new DirectoryInfo(@"wwwroot/images/reflectimages");

            foreach (FileInfo file in folderInfo.GetFiles())
            {
                file.Delete();
            }
            for (int i = 1; i <= 5; i++)
            {
                if (!keyValues.ContainsKey(i))
                {
                    keyValues.Add(i, new List<string>());
                }
            }
            
            var totalcount = 0;
            for (int i = 1; i <= 5; i++)
            {
                if (keyValues.ContainsKey(i))
                    totalcount = totalcount + keyValues[i].Count;
            }
            Bitmap thumbBMP = new Bitmap(1000, 40);
            Graphics flagGraphics = Graphics.FromImage(thumbBMP);
            var color = Brushes.White;
            var width = 0;
            var previouswidth = 0;

           

            for (int i = 1; i <= 5; i++)
            {
                if (keyValues.ContainsKey(i))
                {
                    if (i == 1)
                    {
                        color = Brushes.MediumSeaGreen; 
                    }
                    if (i == 2)
                    {
                        color = Brushes.LightGreen;
                    }
                    if (i == 3)
                    {
                        color = Brushes.Gold;
                    }
                    if (i == 4)
                    {
                        color = Brushes.LightSalmon;
                    }
                    if (i == 5)
                    {
                        color = Brushes.Salmon;
                    }
                    width = (keyValues[i].Count *1000)/totalcount;
                    flagGraphics.FillRectangle(color, previouswidth, 0, width, 40);
                    previouswidth = previouswidth+width+1;
                }
            }
            var datastring = "/Images/reflectimages/" + Guid.NewGuid()+ ".png";
            string outputFileName = @"wwwroot"+ datastring;
            saveImage(thumbBMP, outputFileName);
            return new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveImage() { Url = new Uri(_configuration["BaseUri"] + datastring) },
                    new AdaptiveColumnSet
                    {
                        Columns = new List<AdaptiveColumn>()
                        {
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage() { Size = AdaptiveImageSize.Auto, Url = new Uri(_configuration["BaseUri"] + "/images/ref1.png"),Id = "1" }
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[1].Count.ToString())
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage() { Size = AdaptiveImageSize.Auto, Url = new Uri(_configuration["BaseUri"] + "/images/ref2.png"),Id = "2" }
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[2].Count.ToString())
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage() { Size = AdaptiveImageSize.Auto, Url = new Uri(_configuration["BaseUri"] + "/images/ref3.png"),
                                        Style = AdaptiveImageStyle.Default, Id = "3" }
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[3].Count.ToString())
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage() { Size = AdaptiveImageSize.Auto, Url = new Uri(_configuration["BaseUri"] + "/images/ref4.png"),
                                        Style = AdaptiveImageStyle.Default, Id = "4" }
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[4].Count.ToString())
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage() { Size = AdaptiveImageSize.Auto, Url = new Uri(_configuration["BaseUri"] + "/images/ref5.png"),
                                        Style = AdaptiveImageStyle.Default, Id = "5" }
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[5].Count.ToString())
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
                                URL =_configuration["BaseUri"] + "/openReflections/" + reflectionId,
                            }
                        }
                    },
                },
            };

        }

        public  Task<string> saveImage(Bitmap data, string Filepath)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(Filepath, FileMode.Create, FileAccess.ReadWrite))
                {
                    data.Save(memory, ImageFormat.Png);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return  null;
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
                                    new AdaptiveTextBlock("Posted by "+ $"{data.postCreateBy}") { Color = AdaptiveTextColor.Default, Size=AdaptiveTextSize.Medium, Spacing=AdaptiveSpacing.Medium },
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Width=AdaptiveColumnWidth.Stretch,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock("| Responses are "+ $"{data.privacy}") { Color = AdaptiveTextColor.Default, Size=AdaptiveTextSize.Medium, Spacing=AdaptiveSpacing.Medium},
                                }

                            },

                        }
                    },
                    new AdaptiveTextBlock($"{data.question}") { Id = ($"{data.question }"), Weight = AdaptiveTextWeight.Bolder, Size=AdaptiveTextSize.Large },
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
                                        Style =AdaptiveImageStyle.Person, Id="1", SelectAction = new AdaptiveSubmitAction(){ DataJson = @"{'feedbackId':'1', 'type':'saveFeedback','messageId':'" + data.messageID +"','reflectionId':'" + data.reflectionID +"'}" } }
                                }

                            },
                             new AdaptiveColumn()
                            {
                                Width=AdaptiveColumnWidth.Auto,
                                Items=new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage(){Size=AdaptiveImageSize.Small,Url=new Uri(_configuration["BaseUri"] + "/images/2.png"),
                                        Style =AdaptiveImageStyle.Person, Id="2", SelectAction = new AdaptiveSubmitAction() { DataJson = @"{'feedbackId':'2', 'type':'saveFeedback','messageId':'" + data.messageID +"','reflectionId':'" + data.reflectionID +"'}" } }
                                }

                            },
                             new AdaptiveColumn()
                            {
                                Width=AdaptiveColumnWidth.Auto,
                                Items=new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage(){Size=AdaptiveImageSize.Small,Url=new Uri(_configuration["BaseUri"] + "/images/3.png"),
                                        Style =AdaptiveImageStyle.Person, Id="3", SelectAction = new AdaptiveSubmitAction(){ DataJson = @"{'feedbackId':'3', 'type':'saveFeedback','messageId':'" + data.messageID +"','reflectionId':'" + data.reflectionID +"'}" } }
                                }

                            },
                             new AdaptiveColumn()
                            {
                                Width=AdaptiveColumnWidth.Auto,
                                Items=new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage(){Size=AdaptiveImageSize.Small,Url=new Uri(_configuration["BaseUri"] + "/images/4.png"),
                                        Style =AdaptiveImageStyle.Person, Id="4", SelectAction = new AdaptiveSubmitAction(){ DataJson = @"{'feedbackId':'4', 'type':'saveFeedback','messageId':'" + data.messageID +"','reflectionId':'" + data.reflectionID +"'}" } }
                                }

                            },
                             new AdaptiveColumn()
                            {
                                Width=AdaptiveColumnWidth.Auto,
                                Items=new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage(){Size=AdaptiveImageSize.Small,Url=new Uri(_configuration["BaseUri"] + "/images/5.png"),
                                        Style =AdaptiveImageStyle.Person, Id="5", SelectAction = new AdaptiveSubmitAction(){ DataJson = @"{'feedbackId':'5', 'type':'saveFeedback','messageId':'" + data.messageID +"','reflectionId':'" + data.reflectionID +"'}" } }
                               }

                            },
                        }
                    }
                }
            };

        }

    }
}
