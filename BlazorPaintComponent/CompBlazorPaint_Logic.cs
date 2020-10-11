using BlazorPaintComponent.classes;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Geometry;
using Blazor2PythonWebAPI_interfaces;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components.Web;
//using System.Xml;
//using System.Xml.Serialization;
//using System.Text.Json;
//using System.Text.Json.Serialization;
using System.IO;
using System.Reflection;
using System.Globalization;
//using JsonSerializer = System.Text.Json.JsonSerializer;


[assembly: AssemblyVersion("1.0.0.*")]
namespace BlazorPaintComponent
{
    public class CompBlazorPaint_Logic : ComponentBase
    {
        private Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        private DateTime buildDate = DateTime.UtcNow;
        protected string displayableVersion = "";
        
        bool IsCompLoadedAtLeastOnce = false;

        public OperationalMode CurrOperationalMode = OperationalMode.select;
        public BPaintMode CurrPaintMode = BPaintMode.idle;
        public SelectionMode CurrSelectionMode = SelectionMode.idle;

        //protected CompUsedColors_Logic Curr_CompUsedColors = new CompUsedColors_Logic();
        protected CompMySVG Curr_CompMySVG = new CompMySVG();


        public List<BPaintObject> ObjectsList = new List<BPaintObject>();
        public BPaintRectangle bpSelectionRectangle = null;
        public List<BPaintVertex> VerticesList = new List<BPaintVertex>();
        public List<BPaintVertex> SelectionVerticesList = new List<BPaintVertex>();
#if DEBUG
        protected string strDebugRelease = "DEBUG";
#else
        protected string strDebugRelease = "RELEASE";
#endif


        protected string Color1 = "#000000";//"#fc3807";

        protected int LineWidth1 = 3;

        protected int StepSize = 5;

        public BPaintVertex bpVertexUnderMousePointer = null;
        public BPaintVertex bpSelectionVertexUnderMousePointer = null;

        protected int FigureCode = 4;

        protected PointD currMouseLocation = PointD.nullPointD();
        protected string strCurrMouseLocation = "";

        protected string strCurrSVGareaLocation =>
            string.Format("x: {0}; y: {1}", LocalData.SVGPosition.X, LocalData.SVGPosition.Y);

        protected SizeD PaintAreaSize = new SizeD();


        protected string CurrentBackgroundImageURI = "";
        protected string CurrentImageBasename = "";
        private string guid = (Guid.NewGuid()).ToString().Replace("-", "");
        private readonly HttpClient http = new HttpClient();
        protected bool webAPIsessionStarted = false;

        protected string btnSelectMode_CSSclass = "btn btn-primary btn-with-icons";
        protected string btnDrawMode_CSSclass = "btn btn-outline-primary btn-with-icons";
        protected bool btnSelectAllDisabled { get; set; }
        protected bool btnDeleteSelectedDisabled { get; set; }

        [Inject]
        public NavigationManager MyNavigationManager { get; set; }

        //private string base_webAPI_uri = "http://127.0.0.1:2019/";
        public string base_webAPI_uri
        {
            get
            {
                return MyNavigationManager.Uri + "app/";
            }
        }



        protected override Task OnInitializedAsync()
        {
            
            BlazorWindowHelper.BlazorWindowHelper.Initialize();
            BlazorWindowHelper.BWHJsInterop.SetOnOrOff(true);
            BlazorWindowHelper.BlazorWindowHelper.OnScroll = OnScroll;
            BlazorWindowHelper.BlazorWindowHelper.OnResize = OnScroll;

            CurrentBackgroundImageURI = "";

            CurrOperationalMode = OperationalMode.select;
            OperationalModeHasChanged();

            //buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
            displayableVersion = $"{version}";


            return base.OnInitializedAsync();
        }




        protected void OnScroll()
        {
#if DEBUG
            Console.WriteLine("hit OnScroll");
#endif
            BPaintJsInterop.UpdateSVGPosition("PaintArea1", DotNetObjectReference.Create(this));
            StateHasChanged();
        }



        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender)
            {
                //if (Curr_CompUsedColors.ActionColorClicked == null)
                //{
                //    Curr_CompUsedColors.ActionColorClicked = ColorSelected;
                //}

                GetBoundingClientRect("PaintArea1");

                //IsCompLoadedAtLeastOnce = true;
            }
            
            base.OnAfterRender(firstRender);
        }



#region SDS buttons
        protected void btnSetSDS_nosun_onclick(MouseEventArgs eventArgs)
        {
            
        }

        protected void btnSetSDS_PartiallyCloudy_onclick(MouseEventArgs eventArgs)
        {

        }

        protected void btnSetSDS_Sun2_onclick(MouseEventArgs eventArgs)
        {

        }
#endregion



#region operational modes

        protected void btnSelectMode_onClick(MouseEventArgs eventArgs)
        {
            OperationalMode prevMode = CurrOperationalMode;
            CurrOperationalMode = OperationalMode.select;
            if (CurrOperationalMode != prevMode)
            {
                OperationalModeHasChanged();
            }
        }


        protected void btnDrawMode_onClick(MouseEventArgs eventArgs)
        {
            OperationalMode prevMode = CurrOperationalMode;
            CurrOperationalMode = OperationalMode.draw;
            if (CurrOperationalMode != prevMode)
            {
                OperationalModeHasChanged();
            }
        }


        protected void OperationalModeHasChanged()
        {
            btnDrawMode_CSSclass = (CurrOperationalMode == OperationalMode.draw)?("btn btn-primary btn-with-icons") :("btn btn-outline-primary btn-with-icons");
            btnSelectMode_CSSclass = (CurrOperationalMode == OperationalMode.select) ? ("btn btn-primary btn-with-icons") : ("btn btn-outline-primary btn-with-icons");

            if (CurrOperationalMode == OperationalMode.draw)
            {
                CurrPaintMode = BPaintMode.idle;
                btnDeleteSelectedDisabled = true;
                btnSelectAllDisabled = true;
            }
            else
            {
                CurrSelectionMode = SelectionMode.idle;
                btnDeleteSelectedDisabled = false;
                btnSelectAllDisabled = false;
            }
            
            StateHasChanged();
        }

#endregion






#region web API functions


        protected async void btnPrevExample_onClick()
        {
            bool webAPIsessionStarted = await EnsureWebAPIsessionStarted();
            if (webAPIsessionStarted)
            {
                RequestPreviousExample();
            }
            ClearAllObjects();
        }


        protected async void btnNextExample_onClick()
        {
            bool webAPIsessionStarted = await EnsureWebAPIsessionStarted();
            if (webAPIsessionStarted)
            {
                RequestNextExample();
            }

            ClearAllObjects();
        }



        protected async Task<bool> EnsureWebAPIsessionStarted()
        {
            if (webAPIsessionStarted)
            {
                return true;
            }

            string url = new Uri(base_webAPI_uri).Append("exec?command=start&webapi_client_id=" + guid).AbsoluteUri;
#if DEBUG
            Console.WriteLine("btnStartWebAPIsession_onClick: HTTP GET");
            Console.WriteLine("URL = " + url);
#endif
            HttpClient http = new HttpClient();
            string strRepl = await http.GetStringAsync(url);

            WebAPI_response resp = null;
            try
            {
                resp = JsonConvert.DeserializeObject<WebAPI_response>(strRepl, new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.All});
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
                throw;
            }

            if (resp == null)
            {
#if DEBUG
                Console.WriteLine("ERROR: failed deserializing WebAPI response");
#endif
                return false;
            }

            if (resp.ResponseCode == ResponseCodes.Error)
            {
#if DEBUG
                Console.WriteLine("ERROR : failed starting a WebAPI session:");
                Console.WriteLine("Error code: " + resp.Error.ErrorCode);
                Console.WriteLine("Error message: " + resp.Error.ErrorDescription);
#endif
                return false;
            }
            else if (resp.ResponseCode == ResponseCodes.OK)
            {
                Console.WriteLine("OK : WebAPI session started successfully!");
                webAPIsessionStarted = true;
                return true;
            }

            return false;
        }




        protected async void btnSaveCurrExampleLabels_onClick()
        {
            bool webAPIsessionStarted = await EnsureWebAPIsessionStarted();
            if (!webAPIsessionStarted)
            {
                return;
            }

            
            string url = new Uri(base_webAPI_uri).Append("labels?command=post_current_example_labels&webapi_client_id=" + guid).AbsoluteUri;
#if DEBUG
            Console.WriteLine("URL = " + url);
#endif

            ExampleLabels labelsPackage = new ExampleLabels(CurrentImageBasename, ObjectsList, PaintAreaSize);

            string currExampleLabelsJSONstring = JsonConvert.SerializeObject(labelsPackage, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

#if DEBUG
            Console.WriteLine("currExampleLabelsJSONstring: ");
            Console.WriteLine(currExampleLabelsJSONstring);
#endif
            HttpResponseMessage response = await http.PostAsync(url, new StringContent(currExampleLabelsJSONstring), CancellationToken.None);
            HttpStatusCode respCode = response.StatusCode;
#if DEBUG
            Console.WriteLine("response status code: " + respCode);
#endif
            if (respCode != HttpStatusCode.OK)
            {
                
            }

            StateHasChanged();
        }



        



        protected async Task<bool> RequestImageURL(string request_type = "get_next_image")
        {
            string url = new Uri(base_webAPI_uri).Append("images?command=" + request_type + "&webapi_client_id=" + guid).AbsoluteUri;
#if DEBUG
            Console.WriteLine("URL = " + url);
#endif

            string strRepl = await http.GetStringAsync(url);

            WebAPI_response resp = null;
            try
            {
                resp = JsonConvert.DeserializeObject<WebAPI_response>(strRepl, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
                throw;
            }

            if (resp == null)
            {
#if DEBUG
                Console.WriteLine("ERROR: failed deserializing WebAPI response");
#endif
                return false;
            }


            if (resp.ResponseCode == ResponseCodes.Error)
            {
                WebAPI_error currError = resp.Error;
#if DEBUG
                Console.WriteLine("WebAPI ERROR code : " + currError.ErrorCode);
                Console.WriteLine("WebAPI ERROR description : " + currError.ErrorDescription);
                Console.WriteLine("Response JSON: ");
                Console.Write(resp.ToJSON());
#endif
            }
            else
            {
                try
                {
                    CurrentBackgroundImageURI = resp.StringAttributes["imageURL"];
#if DEBUG
                    Console.WriteLine("got CurrentBackgroundImageURI value: " + CurrentBackgroundImageURI);
#endif
                    CurrentBackgroundImageURI = new Uri(MyNavigationManager.Uri).Append(CurrentBackgroundImageURI).AbsoluteUri;
#if DEBUG
                    Console.WriteLine("now CurrentBackgroundImageURI: " + CurrentBackgroundImageURI);
#endif
                    CurrentImageBasename = resp.StringAttributes["imgBaseName"];

                    StateHasChanged();
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("ERROR : failed extracting CurrentBackgroundImageURI from JSON response.");
                    Console.WriteLine("got JSON response:");
                    Console.Write(strRepl);
                    Console.WriteLine("converted it to the WebAPI_response instance:");
                    Console.Write(resp.ToJSON());
                    Console.WriteLine(e);
#endif
                    return false;
                }
            }

            return true;
        }



        protected async Task<bool> RequestExistingLabels()
        {
            WebAPI_response resp = null;
            string url = new Uri(base_webAPI_uri).Append("labels?command=get_current_example_labels&webapi_client_id=" + guid + "&img_basename=" + CurrentImageBasename).AbsoluteUri;

#if DEBUG
            Console.WriteLine("URL = " + url);
#endif
            string strRepl = await http.GetStringAsync(url);

            try
            {
                resp = JsonConvert.DeserializeObject<WebAPI_response>(strRepl, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
                Console.WriteLine("ERROR: failed deserializing WebAPI response");
                return false;
#endif
            }


            if (resp != null)
            {
                if (resp.ResponseCode == ResponseCodes.Error)
                {
#if DEBUG
                    WebAPI_error currError = resp.Error;
                    Console.WriteLine("WebAPI ERROR code : " + currError.ErrorCode);
                    Console.WriteLine("WebAPI ERROR description : " + currError.ErrorDescription);
                    Console.WriteLine("Response JSON: ");
                    return false;
#endif
                }
                else
                {
                    try
                    {
                        ExampleLabels received_labels = JsonConvert.DeserializeObject<ExampleLabels>(resp.StringAttributes["found_example_labels"], new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
#if DEBUG
                        Console.WriteLine("got received_labels: " + received_labels.ToJSON());
#endif
                        foreach (BPaintObject bpobj in received_labels.LabelsList)
                        {
                            try
                            {
#if DEBUG
                                Console.WriteLine("post-processing BPaintObject");
#endif
                                bpobj.PostJsonDeserializationCleaning();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                            
                            ObjectsList.Add(bpobj);

                            if (ObjectsList.Any())
                            {
                                bpobj.ObjectID = ObjectsList.Max(x => x.ObjectID) + 1;
                                cmd_Clear_Selection();
                                cmd_Clear_Editing();
                            }
                            else
                            {
                                bpobj.ObjectID = 1;
                            }

                            foreach (BPaintVertex v in bpobj.VerticesList)
                            {
#if DEBUG
                                Console.WriteLine("Adding " + bpobj.VerticesList.Count.ToString() + " vertices to this frame");
#endif
                                VerticesList.Add(v);
                            }
                        }

                        cmd_RefreshSVG();

                        return true;
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Console.WriteLine("ERROR : failed extracting found_example_labels from JSON response.");
                        Console.WriteLine("got JSON response:");
                        Console.Write(strRepl);
                        Console.WriteLine("converted it to the WebAPI_response instance:");
                        Console.Write(resp.ToJSON());
                        Console.WriteLine(e);
#endif
                        return false;
                    }
                }
            }
            return true;
        }



        protected async void RequestNextExample()
        {
            bool requestedImage = await RequestImageURL(request_type: "get_next_image");
            bool requestedLabels = await RequestExistingLabels();
            StateHasChanged();
        }



        protected async void RequestPreviousExample()
        {
            bool requestedImage = await RequestImageURL(request_type: "get_previous_image");
            bool requestedLabels = await RequestExistingLabels();
            StateHasChanged();
        }


        #endregion


//        private void ColorSelected(string a)
//        {
//#if DEBUG
//            Console.WriteLine("CompBlazorPaint.ColorSelected() hit; string a = " + a);
//#endif
//            Color1 = a;
            
//            if (ObjectsList.Any())
//            {
//                foreach (var item in ObjectsList.Where(x => x.Selected))
//                {
//                    item.Color = Color1;
//                    foreach(BPaintVertex v in item.VerticesList)
//                    {
//                        v.Color = Color1;
//                    }
//                }
//            }
            
//            StateHasChanged();
//        }

                

        public void cmd_Clear_Selection()
        {
            foreach (var item in ObjectsList.Where(x => x.Selected))
            {
                item.Selected = false;
            }

            foreach (BPaintVertex vertex in VerticesList.Where(v => v.Selected))
            {
                vertex.Selected = false;
            }
        }



        public void cmd_Clear_Editing()
        {
            foreach (var item in ObjectsList.Where(x => x.EditMode))
            {
                item.EditMode = false;
            }
        }



        public string strCurrentlyEditingObject()
        {
            if (!ObjectsList.Any())
            {
                return "none";
            }

            if ((ObjectsList.Any()) & (!ObjectsList.Any(o => o.EditMode)))
            {
                return String.Format("none of {0}", ObjectsList.Count);
            }

            if ((ObjectsList.Any()) & (ObjectsList.Any(o => o.EditMode)))
            {
                return ObjectsList.Single(o => o.EditMode).ToString();
            }

            return "none";
        }



        public void startLine(MouseEventArgs e)
        {
            PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);

            BPaintLine new_BPaintLine = new BPaintLine();


            if (ObjectsList.Any())
            {
                new_BPaintLine.ObjectID = ObjectsList.Max(x => x.ObjectID) + 1;
                cmd_Clear_Selection();
                cmd_Clear_Editing();
            }
            else
            {
                new_BPaintLine.ObjectID = 1;
            }

            BPaintVertex startVertex = new BPaintVertex(CurrPosition, Color1);
            VerticesList.Add(startVertex);
            
            new_BPaintLine.Selected = true;
            new_BPaintLine.EditMode = true;
            new_BPaintLine.Color = Color1;
            new_BPaintLine.LineWidth = LineWidth1;
            new_BPaintLine.Position = startVertex;
            BPaintVertex endVertex = new BPaintVertex(CurrPosition, Color1);
            new_BPaintLine.end = endVertex;
            VerticesList.Add(endVertex);
            ObjectsList.Add(new_BPaintLine);

            CurrPaintMode = BPaintMode.movingAnElement;
            bpVertexUnderMousePointer = endVertex;
        }




        public void startCircle(MouseEventArgs e)
        {
            PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);

            BPaintCircle new_BPaintCircle = new BPaintCircle();


            if (ObjectsList.Any())
            {
                new_BPaintCircle.ObjectID = ObjectsList.Max(x => x.ObjectID) + 1;
                cmd_Clear_Selection();
                cmd_Clear_Editing();
            }
            else
            {
                new_BPaintCircle.ObjectID = 1;
            }

            BPaintVertex startVertex = new BPaintVertex(CurrPosition, Color1);
            VerticesList.Add(startVertex);

            new_BPaintCircle.Selected = true;
            new_BPaintCircle.EditMode = true;
            new_BPaintCircle.Color = Color1;
            new_BPaintCircle.LineWidth = LineWidth1;
            new_BPaintCircle.Position = startVertex;
            BPaintVertex endVertex = new BPaintVertex(CurrPosition, Color1);
            new_BPaintCircle.end = endVertex;
            VerticesList.Add(endVertex);
            CurrPaintMode = BPaintMode.movingAnElement;
            bpVertexUnderMousePointer = endVertex;
            ObjectsList.Add(new_BPaintCircle);
        }




        public void startEllipse(MouseEventArgs e)
        {
            PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);

            BPaintEllipse new_BPaintEllipse = new BPaintEllipse();


            if (ObjectsList.Any())
            {
                new_BPaintEllipse.ObjectID = ObjectsList.Max(x => x.ObjectID) + 1;
                cmd_Clear_Selection();
                cmd_Clear_Editing();
            }
            else
            {
                new_BPaintEllipse.ObjectID = 1;
            }

            BPaintVertex startVertex = new BPaintVertex(CurrPosition, Color1);
            VerticesList.Add(startVertex);

            new_BPaintEllipse.Selected = true;
            new_BPaintEllipse.EditMode = true;
            new_BPaintEllipse.Color = Color1;
            new_BPaintEllipse.LineWidth = LineWidth1;
            new_BPaintEllipse.Position = startVertex;
            //new_BPaintEllipse.VerticesList.Add(startVertex);
            CurrPaintMode = BPaintMode.drawing;
            ObjectsList.Add(new_BPaintEllipse);
        }




        public void continueEllipse(MouseEventArgs e)
        {
            PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);

            BPaintEllipse currBPaintEllipse =
                ObjectsList.Single(o => ((o.EditMode) & (o.ObjectType == BPaintOpbjectType.Ellipse))) as BPaintEllipse;

            BPaintVertex currVertex = new BPaintVertex(CurrPosition, Color1);
            VerticesList.Add(currVertex);

            switch (currBPaintEllipse.VerticesList.Count)
            {
                case 1:
                    currBPaintEllipse.pt2 = currVertex;
                    break;
                case 2:
                    currBPaintEllipse.pt3 = currVertex;
                    currBPaintEllipse.EditMode = false;
                    CurrPaintMode = BPaintMode.idle;
                    break;
                default:
                    break;
            }
        }




        public BPaintVertex makeVertex(MouseEventArgs e)
        {
            PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);

            BPaintVertex currBPaintVertex = new BPaintVertex();


            if (ObjectsList.Any())
            {
                cmd_Clear_Selection();
            }
            
            currBPaintVertex.Selected = true;
            currBPaintVertex.Color = Color1;
            currBPaintVertex.PtD = new PointD(CurrPosition);
            VerticesList.Add(currBPaintVertex);
            return currBPaintVertex;
        }




        public void onMouseMove(MouseEventArgs e)
        {
            //this.OnScroll();

            PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);
            currMouseLocation = CurrPosition;
            strCurrMouseLocation = "x: " + currMouseLocation.X.ToString("F2") + "; y: " + currMouseLocation.Y.ToString("F2");

            if (CurrOperationalMode == OperationalMode.select)
            {
                switch (CurrSelectionMode)
                {
                    case SelectionMode.idle:
                        if (SelectionVerticesList.Any(x => (CurrPosition.DistanceTo(x.PtD) <= 10)))
                        {
                            BPaintVertex currVertex =
                                SelectionVerticesList.Single(x => (CurrPosition.DistanceTo(x.PtD) <= 10));

                            bpSelectionVertexUnderMousePointer = currVertex;
                            bpSelectionVertexUnderMousePointer.Selected = true;
                            CurrSelectionMode = SelectionMode.hoveredAVertex;
                        }
                        else
                        {
                            bpSelectionVertexUnderMousePointer = null;
                            CurrSelectionMode = SelectionMode.idle;
                        }

                        break;
                    case SelectionMode.movingAnElement:
                        BPaintVertex vertex2Move = bpSelectionVertexUnderMousePointer as BPaintVertex;
                        vertex2Move.PtD = CurrPosition;
                        break;
                    case SelectionMode.hoveredAVertex:
                        if (SelectionVerticesList.Any(x => (CurrPosition.DistanceTo(x.PtD) <= 10)))
                        {
                            BPaintVertex currVertex =
                                SelectionVerticesList.Single(x => (CurrPosition.DistanceTo(x.PtD) <= 10));

                            bpSelectionVertexUnderMousePointer = currVertex;
                            CurrSelectionMode = SelectionMode.hoveredAVertex;
                        }
                        else
                        {
                            bpSelectionVertexUnderMousePointer = null;

                            foreach (BPaintVertex vertex in SelectionVerticesList)
                            {
                                vertex.Selected = false;
                            }

                            CurrSelectionMode = SelectionMode.idle;
                        }

                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (CurrPaintMode)
                {
                    // TODO: Fix vertices highlighting
                    // category=Appearance issue=none estimate=2h
                    // when there is no selection rectangle, vertices under the mouse cursor are not highlighted

                    case BPaintMode.idle:
                        if (VerticesList.Any(x => (CurrPosition.DistanceTo(x.PtD) <= 10)))
                        {
                            BPaintVertex currVertex = VerticesList.Single(x => (CurrPosition.DistanceTo(x.PtD) <= 10));

                            bpVertexUnderMousePointer = currVertex;
                            bpVertexUnderMousePointer.Selected = true;
                            CurrPaintMode = BPaintMode.hoveredAVertex;
                        }
                        else
                        {
                            bpVertexUnderMousePointer = null;
                            CurrPaintMode = BPaintMode.idle;
                        }

                        break;
                    case BPaintMode.drawing:
                        break;
                    case BPaintMode.movingAnElement:
                        BPaintVertex vertex2Move = bpVertexUnderMousePointer as BPaintVertex;
                        vertex2Move.PtD = CurrPosition;
                        break;
                    case BPaintMode.hoveredAVertex:
                        if (VerticesList.Any(x => (CurrPosition.DistanceTo(x.PtD) <= 10)))
                        {
                            BPaintVertex currVertex = VerticesList.Single(x => (CurrPosition.DistanceTo(x.PtD) <= 10));

                            bpVertexUnderMousePointer = currVertex;
                            CurrPaintMode = BPaintMode.hoveredAVertex;
                        }
                        else
                        {
                            bpVertexUnderMousePointer = null;
                            foreach (BPaintObject bpObject in ObjectsList)
                            {
                                //bpObject.Selected = false;
                                bpObject.EditMode = false;
                            }

                            foreach (BPaintVertex vertex in VerticesList)
                            {
                                vertex.Selected = false;
                            }

                            CurrPaintMode = BPaintMode.idle;
                        }

                        break;
                    default:
                        break;
                }
            }

            ProcessSelection();
            StateHasChanged();
        }




        public void onMouseDown(MouseEventArgs e)
        {
            if (CurrOperationalMode == OperationalMode.select)
            {
                if (CurrSelectionMode == SelectionMode.idle)
                {
                    if (bpSelectionRectangle != null)
                    {
                        foreach (BPaintVertex vertex in bpSelectionRectangle.VerticesList)
                        {
                            SelectionVerticesList.RemoveAll(x => true);
                        }
                        bpSelectionRectangle = null;
                    }
                    
                    PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);

                    bpSelectionRectangle = new BPaintRectangle()
                    {
                        ObjectID = 999
                    };

                    BPaintVertex startVertex = new BPaintVertex(CurrPosition, "magenta");
                    SelectionVerticesList.Add(startVertex);

                    bpSelectionRectangle.Selected = false;
                    bpSelectionRectangle.EditMode = true;
                    bpSelectionRectangle.Color = "red";
                    bpSelectionRectangle.LineWidth = 2;
                    bpSelectionRectangle.Position = startVertex;
                    BPaintVertex endVertex = new BPaintVertex(CurrPosition, "magenta");
                    bpSelectionRectangle.end = endVertex;
                    SelectionVerticesList.Add(endVertex);
                    CurrSelectionMode = SelectionMode.movingAnElement;
                    bpSelectionVertexUnderMousePointer = endVertex;

                }
                else if (CurrSelectionMode == SelectionMode.hoveredAVertex)
                {
                    if (bpSelectionVertexUnderMousePointer != null)
                    {
                        BPaintVertex currVertex = bpSelectionVertexUnderMousePointer as BPaintVertex;
                        currVertex.Selected = true;
                        CurrSelectionMode = SelectionMode.movingAnElement;
                    }
                }
                StateHasChanged();
            }
            else
            {
                if (CurrPaintMode == BPaintMode.idle)
                {
                    if (!ObjectsList.Any(o => o.EditMode))
                    {
                        switch (FigureCode)
                        {
                            case 2:
                                startLine(e);
                                break;
                            case 3:
                                BPaintVertex newVertex = makeVertex(e);
                                CurrPaintMode = BPaintMode.idle;
                                break;
                            case 4:
                                startCircle(e);
                                break;
                            case 5:
                                //cmd_prepareEllipse(e);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        BPaintObject currBPaintObject = ObjectsList.Single(o => o.EditMode);
                        BPaintVertex newVertex = makeVertex(e);

                        switch (currBPaintObject.ObjectType)
                        {
                            case BPaintOpbjectType.Ellipse:
                                BPaintEllipse currEllipse = currBPaintObject as BPaintEllipse;
                                currEllipse.pt3 = newVertex;
                                currEllipse.EditMode = false;
                                break;
                            default:
                                break;
                        }

                        CurrPaintMode = BPaintMode.idle;
                    }
                }
                else if (CurrPaintMode == BPaintMode.hoveredAVertex)
                {
                    if (bpVertexUnderMousePointer != null)
                    {
                        BPaintVertex currVertex = bpVertexUnderMousePointer as BPaintVertex;
                        currVertex.Selected = true;
                        CurrPaintMode = BPaintMode.movingAnElement;
                    }

                }

                StateHasChanged();
            }
        }





        public void onMouseUp(MouseEventArgs e)
        {
            if (CurrOperationalMode == OperationalMode.select)
            {
                switch (CurrSelectionMode)
                {
                    case SelectionMode.movingAnElement:
                        CurrSelectionMode = SelectionMode.idle;

                        if (!bpSelectionRectangle.IsValid())
                        {
                            bpSelectionRectangle = null;
                            SelectionVerticesList.RemoveAll(x => true);
                        }
                        
                        break;
                    default:
                        break;
                }

                ProcessSelection();
                cmd_RefreshSVG();
                StateHasChanged();
            }
            else
            {
                switch (CurrPaintMode)
                {
                    case BPaintMode.idle:
                        // this can be if the object to create need more than 2 vertices
                        switch (FigureCode)
                        {
                            case 5:
                                startEllipse(e);
                                break;
                            default:
                                break;
                        }

                        break;
                    case BPaintMode.movingAnElement:
                        if (ObjectsList.Any(o => o.EditMode))
                        {
                            BPaintObject currObjectEditing = ObjectsList.Single(o => o.EditMode);
                            if (currObjectEditing.MandatoryVerticesCount > 2)
                            {
                                CurrPaintMode = BPaintMode.idle;
                            }
                            else
                            {
                                CurrPaintMode = BPaintMode.idle;
                            }
                        }
                        else
                        {
                            CurrPaintMode = BPaintMode.hoveredAVertex;
                            cmd_Clear_Editing();
                        }

                        ProcessSelection();
                        cmd_RefreshSVG();
                        break;
                    case BPaintMode.drawing:
                        switch (FigureCode)
                        {
                            case 5:
                                continueEllipse(e);
                                break;
                            default:
                                break;
                        }

                        break;
                    default:
                        break;
                }

                StateHasChanged();
            }
        }




        protected void ProcessSelection()
        {
            if (bpSelectionRectangle is null)
            {
                cmd_Clear_Selection();
                return;
            }

            RectD SelectionRect = bpSelectionRectangle.BoundingRectD(padding: false);
            foreach (BPaintObject obj in ObjectsList)
            {
                RectD currObjBoundingRect = obj.BoundingRectD(padding:false);
                if (currObjBoundingRect.IsInRect(SelectionRect))
                {
                    obj.Selected = true;
                }
                else
                {
                    obj.Selected = false;
                }
            }
        }




        protected void btnSelectAll_onClick()
        {
            //ChangeObjectsSelection(true);
            RectD unionRect = ObjectsList.First().BoundingRectD();
            foreach (BPaintObject obj in ObjectsList)
            {
                unionRect = unionRect.UnionWith(obj.BoundingRectD());
            }

            bpSelectionRectangle = new BPaintRectangle()
            {
                ObjectID = 999
            };

            BPaintVertex startVertex = new BPaintVertex(new PointD(unionRect.x-10, unionRect.y-10), "magenta");
            SelectionVerticesList.Add(startVertex);

            bpSelectionRectangle.Selected = false;
            bpSelectionRectangle.EditMode = true;
            bpSelectionRectangle.Color = "red";
            bpSelectionRectangle.LineWidth = 2;
            bpSelectionRectangle.Position = startVertex;
            BPaintVertex endVertex = new BPaintVertex(new PointD(unionRect.x + unionRect.width+10, unionRect.y + unionRect.height+10), "magenta");
            bpSelectionRectangle.end = endVertex;
            SelectionVerticesList.Add(endVertex);
            bpSelectionVertexUnderMousePointer = endVertex;
            
            ProcessSelection();
            cmd_RefreshSVG();
            StateHasChanged();
        }



        protected void cmd_delete_object()
        {
            if (ObjectsList.Any())
            {
                if (ObjectsList.Any(x => x.Selected))
                {
                    BPaintObject bpObjectToDelete = ObjectsList.Where(x => x.Selected).First();
                    foreach (BPaintVertex vertex in bpObjectToDelete.VerticesList)
                    {
                        VerticesList.Remove(vertex);
                    }

                    ObjectsList.Remove(bpObjectToDelete);
                    BPaintObject b = ObjectsList.Single(i => i.ObjectID == ObjectsList.Min(x => x.ObjectID));
                    b.Selected = true;

                    cmd_RefreshSVG();
                }

            }
        }




        protected void btnDeleteSelectedObjects_onClick()
        {
            if (ObjectsList.Any())
            {
                if (ObjectsList.Any(x => x.Selected))
                {
                    List<BPaintObject> objectsToDelete = ObjectsList.Where(x => x.Selected).ToList();
                    foreach (BPaintObject bpObjectToDelete in objectsToDelete)
                    {
                        foreach (BPaintVertex vertex in bpObjectToDelete.VerticesList)
                        {
                            VerticesList.Remove(vertex);
                        }
                        ObjectsList.Remove(bpObjectToDelete);
                    }
                    
                    cmd_RefreshSVG();
                }
            }
        }



        protected void ClearAllObjects()
        {
            if (VerticesList.Any())
            {
                VerticesList.Clear();
            }


            if (ObjectsList.Any())
            {
                ObjectsList.Clear();
                //List<BPaintObject> objectsToDelete = ObjectsList;
                //foreach (BPaintObject bpObjectToDelete in objectsToDelete)
                //{
                //    foreach (BPaintVertex vertex in bpObjectToDelete.VerticesList)
                //    {
                //        VerticesList.Remove(vertex);
                //    }
                //    ObjectsList.Remove(bpObjectToDelete);
                //}

                //cmd_RefreshSVG();
            }

            cmd_RefreshSVG();
        }






        public void cmd_RefreshSVG()
        {
            Curr_CompMySVG.Refresh();
            StateHasChanged();
        }



//        protected void cmd_ColorChange(ChangeEventArgs e)
//        {
//#if DEBUG
//            Console.WriteLine("Hit cmd_ColorChange(), e = " + e.ToString());
//#endif
//            if (e?.Value != null)
//            {
//                Color1 = e.Value as string;

//                if (Curr_CompUsedColors.UsedColors_List.Any(x => x == Color1))
//                {
//                    Curr_CompUsedColors.UsedColors_List.Remove(Curr_CompUsedColors.UsedColors_List.Single(x => x == Color1));
//                }

//                if (Curr_CompUsedColors.UsedColors_List.Count > 9)
//                {
//                    Curr_CompUsedColors.UsedColors_List.RemoveAt(0);
//                }

//                Curr_CompUsedColors.UsedColors_List.Add(Color1);

//                Cmd_RefreshUsedColorsSVG();
//            }
//        }



        //public void Cmd_RefreshUsedColorsSVG()
        //{
        //    Curr_CompUsedColors.Refresh();
        //    StateHasChanged();
        //}



        public void Dispose()
        {

        }



        protected void cmd_Move(BPaintMoveDirection Par_Direction)
        {
            if (ObjectsList.Any())
            {
                foreach (var item in ObjectsList.Where(x => x.Selected))
                {
                    switch (Par_Direction)
                    {
                        case BPaintMoveDirection.left:
                            item.Position.PtD += new SizeD(-StepSize, 0);
                            break;
                        case BPaintMoveDirection.right:
                            item.Position.PtD += new SizeD(StepSize, 0);
                            break;
                        case BPaintMoveDirection.up:
                            item.Position.PtD += new SizeD(0, -StepSize);
                            break;
                        case BPaintMoveDirection.down:
                            item.Position.PtD += new SizeD(0, StepSize);
                            break;
                        default:
                            break;
                    }
                }
                cmd_RefreshSVG();
            }
        }


        public void GetBoundingClientRect(string ElementID)
        {
            BPaintJsInterop.GetElementBoundingClientRect(ElementID, DotNetObjectReference.Create(this));
        }


        
        [JSInvokable]
        public void invokeFromjs(string id, string rect_left, string rect_top, string rect_width, string rect_height, string window_scrollX, string window_scrollY)
        {
#if DEBUG
            //Console.WriteLine("invokeFromjs hit. Got strings: ");
            //List<string> args = new List<string>() { rect_left, rect_top, rect_width, rect_height, window_scrollX, window_scrollY };
            //List<string> names = new List<string>() { "rect_left", "rect_top", "rect_width", "rect_height", "window_scrollX", "window_scrollY" };
            //var zipped = args.Zip(names, (a,n) => new Tuple<string, string>(a, n));
            //foreach (Tuple<string, string> z in zipped)
            //{
            //    Console.WriteLine(z.Item2 + ": " + z.Item1);
            //}
#endif

            double d_rect_left = double.NaN;
            double d_rect_top = double.NaN;
            double d_rect_width = double.NaN;
            double d_rect_height = double.NaN;
            double d_window_scrollX = double.NaN;
            double d_window_scrollY = double.NaN;
            bool successfully_converted_coordinates = false;

            try
            {
                d_rect_left = Convert.ToDouble(rect_left.Replace(',', '.'), CultureInfo.InvariantCulture);
                d_rect_top = Convert.ToDouble(rect_top.Replace(',', '.'), CultureInfo.InvariantCulture);
                d_rect_width = Convert.ToDouble(rect_width.Replace(',', '.'), CultureInfo.InvariantCulture);
                d_rect_height = Convert.ToDouble(rect_height.Replace(',', '.'), CultureInfo.InvariantCulture);
                d_window_scrollX = Convert.ToDouble(window_scrollX.Replace(',', '.'), CultureInfo.InvariantCulture);
                d_window_scrollY = Convert.ToDouble(window_scrollY.Replace(',', '.'), CultureInfo.InvariantCulture);
                successfully_converted_coordinates = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            if (successfully_converted_coordinates)
            {
                try
                {
                    LocalData.SVGPosition = new PointD(d_rect_left, d_rect_top);
                    PaintAreaSize.Width = d_rect_width;
                    PaintAreaSize.Height = d_rect_height;
                    //StateHasChanged();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }


        }
    }
}
