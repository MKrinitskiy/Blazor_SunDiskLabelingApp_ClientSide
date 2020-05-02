using BlazorPaintComponent.classes;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using BlazorSvgHelper.Classes.SubClasses;
using Geometry;
using Blazor2PythonWebAPI_interfaces;
using CommonInterfaces;
using Newtonsoft.Json;
using CommonLibs;
using Microsoft.AspNetCore.Components.Web;



namespace BlazorPaintComponent
{
    public class CompBlazorPaint_Logic : ComponentBase
    {
        // TODO: Implement saving/storing(serverside)/restoring labels.
        // category=functionality issue=none priority=5 estimate=12h
        // While saving labels the app should send it to server and perhaps give a user an opportunity to download it

        // DONE: Implement selection. It doesn't work at the moment
        // category=functionality issue=none priority=3 estimate=12h

        bool IsCompLoadedAtLeastOnce = false;


        public OperationalMode CurrOperationalMode = OperationalMode.select;
        public BPaintMode CurrPaintMode = BPaintMode.idle;
        public SelectionMode CurrSelectionMode = SelectionMode.idle;

        protected CompUsedColors_Logic Curr_CompUsedColors = new CompUsedColors_Logic();
        protected CompMySVG Curr_CompMySVG = new CompMySVG();


        public List<BPaintObject> ObjectsList = new List<BPaintObject>();
        public BPaintRectangle bpSelectionRectangle = null;
        public List<BPaintVertex> VerticesList = new List<BPaintVertex>();
        public List<BPaintVertex> SelectionVerticesList = new List<BPaintVertex>();


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


        protected string CurrentBackgroundImageURI = "";
        private string guid = (Guid.NewGuid()).ToString().Replace("-", "");
        private readonly HttpClient http = new HttpClient();
        private string base_webAPI_uri = "http://127.0.0.1:2019/";
        protected bool webAPIsessionStarted = false;

        protected string btnSelectMode_CSSclass = "btn btn-primary btn-with-icons";
        protected string btnDrawMode_CSSclass = "btn btn-outline-primary btn-with-icons";
        protected bool btnSelectAllDisabled { get; set; }
        protected bool btnDeleteSelectedDisabled { get; set; }

        //protected string logText { get; set; } = "";



        protected override Task OnInitializedAsync()
        {
            
            BlazorWindowHelper.BlazorWindowHelper.Initialize();
            BlazorWindowHelper.BWHJsInterop.SetOnOrOff(true);
            BlazorWindowHelper.BlazorWindowHelper.OnScroll = OnScroll;
            BlazorWindowHelper.BlazorWindowHelper.OnResize = OnScroll;

            CurrentBackgroundImageURI = "";

            CurrOperationalMode = OperationalMode.select;
            OperationalModeHasChanged(); 
            
            return base.OnInitializedAsync();
        }




        protected void OnScroll()
        {
            //Console.WriteLine("hit OnScroll");
            BPaintJsInterop.UpdateSVGPosition("PaintArea1", DotNetObjectReference.Create(this));
            StateHasChanged();
        }



        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender)
            {
                if (Curr_CompUsedColors.ActionColorClicked == null)
                {
                    Curr_CompUsedColors.ActionColorClicked = ColorSelected;
                }

                GetBoundingClientRect("PaintArea1");

                //IsCompLoadedAtLeastOnce = true;
            }
            
            base.OnAfterRender(firstRender);
        }




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


        
        protected async Task<bool> EnsureWebAPIsessionStarted()
        {
            if (webAPIsessionStarted)
            {
                return true;
            }

            string url = new Uri(base_webAPI_uri).Append("exec?command=start&webapi_client_id=" + guid).AbsoluteUri;
            Console.WriteLine("btnStartWebAPIsession_onClick: HTTP GET");
            Console.WriteLine("URL = " + url);
            //logText += "btnStartWebAPIsession_onClick: HTTP GET" + Environment.NewLine;
            //logText += "URL = " + url + Environment.NewLine;
            HttpClient http = new HttpClient();
            string strRepl = await http.GetStringAsync(url);

            WebAPI_response resp = null;
            try
            {
                resp = JsonConvert.DeserializeObject<WebAPI_response>(strRepl);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //logText += e.ToString() + Environment.NewLine;
                throw;
            }

            if (resp == null)
            {
                Console.WriteLine("ERROR: failed deserializing WebAPI response");
                //logText += "ERROR: failed deserializing WebAPI response" + Environment.NewLine;
                return false;
            }

            if (resp.ResponseCode == ResponseCodes.Error)
            {
                //logText += "ERROR : failed starting a WebAPI session:" + Environment.NewLine;
                //logText += "Error code: " + resp.Error.ErrorCode + Environment.NewLine;
                //logText += "Error message: " + resp.Error.ErrorDescription + Environment.NewLine;

                Console.WriteLine("ERROR : failed starting a WebAPI session:");
                Console.WriteLine("Error code: " + resp.Error.ErrorCode);
                Console.WriteLine("Error message: " + resp.Error.ErrorDescription);
                return false;
            }
            else if (resp.ResponseCode == ResponseCodes.OK)
            {
                Console.WriteLine("OK : WebAPI session started successfully!");
                //logText += "OK : WebAPI session started successfully!" + Environment.NewLine;
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

            // TODO: implement labels data transfer to server-side
            // category=Client-server-interface issue=none estimate=6h


            string url = new Uri(base_webAPI_uri).Append("labels?command=post_current_example_labels&webapi_client_id=" + guid).AbsoluteUri;
            Console.WriteLine("URL = " + url);
            //logText += "URL = " + url + Environment.NewLine;
            //logText += url + Environment.NewLine;
            HttpResponseMessage response = await http.PostAsync(url, new StringContent(""), CancellationToken.None);
            HttpStatusCode respCode = response.StatusCode;
            Console.WriteLine("response status code: " + respCode);
            //logText += "response status code: " + respCode + Environment.NewLine;
            if (respCode != HttpStatusCode.OK)
            {
                
            }

            StateHasChanged();
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

        protected async void RequestNextExample()
        {
            #region request for a next image

            #region the image 
            string url = new Uri(base_webAPI_uri).Append("images?command=get_next_image&webapi_client_id=" + guid).AbsoluteUri;
            Console.WriteLine("URL = " + url);
            //logText += "URL = " + url + Environment.NewLine;
            string strRepl = await http.GetStringAsync(url);

            WebAPI_response resp = null;
            try
            {
                resp = JsonConvert.DeserializeObject<WebAPI_response>(strRepl);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //logText += e.ToString() + Environment.NewLine;
                throw;
            }

            if (resp == null)
            {
                Console.WriteLine("ERROR: failed deserializing WebAPI response");
                //logText += "ERROR: failed deserializing WebAPI response" + Environment.NewLine;
                return;
            }

            if (resp.ResponseCode == ResponseCodes.Error)
            {
                WebAPI_error currError = resp.Error;
                Console.WriteLine("WebAPI ERROR code : " + currError.ErrorCode);
                Console.WriteLine("WebAPI ERROR description : " + currError.ErrorDescription);
                Console.WriteLine("Response JSON: ");
                Console.Write(resp.ToJSON());
            }
            else
            {
                try
                {
                    CurrentBackgroundImageURI = resp.StringAttributes["imageURL"];
                    Console.WriteLine("got CurrentBackgroundImageURI value: " + CurrentBackgroundImageURI);
                    CurrentBackgroundImageURI = new Uri(base_webAPI_uri).Append(CurrentBackgroundImageURI).AbsoluteUri;
                    Console.WriteLine("now CurrentBackgroundImageURI: " + CurrentBackgroundImageURI);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR : failed extracting CurrentBackgroundImageURI from JSON response.");
                    Console.WriteLine("got JSON response:");
                    Console.Write(strRepl);
                    Console.WriteLine("converted it to the WebAPI_response instance:");
                    Console.Write(resp.ToJSON());
                    Console.WriteLine(e);
                    return;
                }
            }

            #endregion the image

            #region request existing labels
            resp = null;
            url = new Uri(base_webAPI_uri).Append("labels?command=get_current_example_labels&webapi_client_id=" + guid).AbsoluteUri;
            Console.WriteLine("URL = " + url);
            strRepl = await http.GetStringAsync(url);
            try
            {
                resp = JsonConvert.DeserializeObject<WebAPI_response>(strRepl);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("ERROR: failed deserializing WebAPI response");
            }

            
            if (resp != null)
            {
                if (resp.ResponseCode == ResponseCodes.Error)
                {
                    WebAPI_error currError = resp.Error;
                    Console.WriteLine("WebAPI ERROR code : " + currError.ErrorCode);
                    Console.WriteLine("WebAPI ERROR description : " + currError.ErrorDescription);
                    Console.WriteLine("Response JSON: ");
                }
                else
                {
                    try
                    {
                        CurrentBackgroundImageURI = resp.StringAttributes["imageURL"];
                        Console.WriteLine("got CurrentBackgroundImageURI value: " + CurrentBackgroundImageURI);
                        CurrentBackgroundImageURI =
                            new Uri(base_webAPI_uri).Append(CurrentBackgroundImageURI).AbsoluteUri;
                        Console.WriteLine("now CurrentBackgroundImageURI: " + CurrentBackgroundImageURI);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR : failed extracting CurrentBackgroundImageURI from JSON response.");
                        Console.WriteLine("got JSON response:");
                        Console.Write(strRepl);
                        Console.WriteLine("converted it to the WebAPI_response instance:");
                        Console.Write(resp.ToJSON());
                        Console.WriteLine(e);
                        return;
                    }
                }
            }

            #endregion request existing labels

            StateHasChanged();

            #endregion
        }




        protected async void RequestPreviousExample()
        {
            #region request for a next image

            string url = new Uri(base_webAPI_uri).Append("images?command=get_previous_image&webapi_client_id=" + guid).AbsoluteUri;
            Console.WriteLine("URL = " + url);
            string strRepl = await http.GetStringAsync(url);

            WebAPI_response resp = null;
            try
            {
                resp = JsonConvert.DeserializeObject<WebAPI_response>(strRepl);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //logText += e.ToString() + Environment.NewLine;
                throw;
            }

            if (resp == null)
            {
                Console.WriteLine("ERROR: failed deserializing WebAPI response");
                //logText += "ERROR: failed deserializing WebAPI response" + Environment.NewLine;
                return;
            }

            try
            {
                CurrentBackgroundImageURI = resp.StringAttributes["imageURL"];
                Console.WriteLine("got CurrentBackgroundImageURI value: " + CurrentBackgroundImageURI);
                //logText += "got CurrentBackgroundImageURI value: " + CurrentBackgroundImageURI + Environment.NewLine;
                CurrentBackgroundImageURI = new Uri(base_webAPI_uri).Append(CurrentBackgroundImageURI).AbsoluteUri;
                Console.WriteLine("now CurrentBackgroundImageURI: " + CurrentBackgroundImageURI);
                //logText += "now CurrentBackgroundImageURI: " + CurrentBackgroundImageURI + Environment.NewLine;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : failed extracting CurrentBackgroundImageURI from JSON response.");
                Console.WriteLine("got JSON response:");
                Console.Write(strRepl);
                Console.WriteLine("converted it to the WebAPI_response instance:");
                Console.Write(resp.ToJSON());
                Console.WriteLine(e);
                return;
            }

            StateHasChanged();

            #endregion
        }




        #endregion


        private void ColorSelected(string a)
        {
            Console.WriteLine("CompBlazorPaint.ColorSelected() hit; string a = " + a);
            Color1 = a;
            
            if (ObjectsList.Any())
            {
                foreach (var item in ObjectsList.Where(x => x.Selected))
                {
                    item.Color = Color1;
                    foreach(BPaintVertex v in item.VerticesList)
                    {
                        v.Color = Color1;
                    }
                }
            }
            
            StateHasChanged();
        }


        #region // cmd_Size_Changed
        //protected void cmd_Size_Changed(UIChangeEventArgs e)
        //{
        //    if (e?.Value != null)
        //    {
        //        LineWidth1 = int.Parse(e.Value as string);

        //        //if (Curr_Mode == BPaintMode.editing)
        //        //{
        //        if (ObjectsList.Any())
        //        {
        //            foreach (var item in ObjectsList.Where(x => x.Selected))
        //            {
        //                item.LineWidth = LineWidth1;
        //            }
        //        }
        //        //}
        //        cmd_RefreshSVG();
        //    }
        //}
        #endregion


        #region // clear & undo
        //public void cmd_clear()
        //{
        //    if (ObjectsList.Any())
        //    {
        //        ObjectsList = new List<BPaintObject>();
        //    }

        //    if (VerticesList.Any())
        //    {
        //        VerticesList = new List<BPaintVertex>();
        //    }

        //    cmd_RefreshSVG();
        //}


        //public void cmd_undo()
        //{
        //    if (ObjectsList.Any())
        //    {
        //        ObjectsList.Remove(ObjectsList.Last());
        //        cmd_RefreshSVG();
        //    }
        //}
        #endregion


        #region // scaling
        //protected void cmd_scale_x(UIChangeEventArgs e)
        //{
        //    if (e?.Value != null)
        //    {
        //        Curr_Scale_X = double.Parse(e.Value as string);




        //        if (ObjectsList.Any())
        //        {

        //            foreach (var item in ObjectsList.Where(x => x.Selected))
        //            {
        //                item.Scale = new PointD(Curr_Scale_X, Curr_Scale_Y);
        //            }



        //            cmd_RefreshSVG();


        //        }
        //    }
        //}


        //protected void cmd_scale_y(UIChangeEventArgs e)
        //{
        //    if (e?.Value != null)
        //    {
        //        Curr_Scale_Y = double.Parse(e.Value as string);

        //        if (ObjectsList.Any())
        //        {
        //            foreach (var item in ObjectsList.Where(x => x.Selected))
        //            {
        //                item.Scale = new PointD(Curr_Scale_X, Curr_Scale_Y);
        //            }
        //            cmd_RefreshSVG();
        //        }
        //    }
        //}
        #endregion // scaling


        //protected void cmd_rotate(UIChangeEventArgs e)
        //{
        //    bool anythingChanged = false;
        //    if (e?.Value != null)
        //    {
        //        Curr_Rotate_rad = double.Parse(e.Value as string);
        //
        //        if (ObjectsList.Any())
        //        {
        //            foreach (BPaintObject paintObject in ObjectsList.Where(x => x.ObjectType == BPaintOpbjectType.Line))
        //            {
        //                BPaintLine currLine = paintObject as BPaintLine;
        //                SVGtransform currLineTransform = new SVGtransform(
        //                    rotateCx: currLine.VerticesList.Average(vertex => vertex.Position.X),
        //                    rotateCy: currLine.VerticesList.Average(vertex => vertex.Position.Y),
        //                    rotateAngleDegrees: Curr_Rotate_deg);
        //                currLine.transform = currLineTransform;
        //                foreach (BPaintVertex vertex in currLine.VerticesList)
        //                {
        //                    vertex.transform = currLineTransform;
        //                }
        //            }
        //            anythingChanged = anythingChanged | true;
        //        }

        //        //if (VerticesList.Any())
        //        //{
        //        //    foreach (BPaintVertex vertex in VerticesList)
        //        //    {
        //        //        vertex.transform = new SVGtransform(rotateAngleDegrees: Curr_Rotate_deg, rotateCx: vertex.Position.X, rotateCy:vertex.Position.Y);
        //        //    }
        //        //    anythingChanged = anythingChanged | true;
        //        //}
        //    }
        //
        //    if (anythingChanged)
        //    {
        //        cmd_RefreshSVG();
        //    }
        //}




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
            strCurrMouseLocation = "x: " + currMouseLocation.X.ToString("F2") + "; y: " +
                                   currMouseLocation.Y.ToString("F2");

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
            if (ObjectsList.Any())
            {
                List<BPaintObject> objectsToDelete = ObjectsList;
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






        public void cmd_RefreshSVG()
        {
            Curr_CompMySVG.Refresh();
            StateHasChanged();
        }



        protected void cmd_ColorChange(ChangeEventArgs e)
        {
            Console.WriteLine("Hit cmd_ColorChange(), e = " + e.ToString());
            if (e?.Value != null)
            {
                Color1 = e.Value as string;

                if (Curr_CompUsedColors.UsedColors_List.Any(x => x == Color1))
                {
                    Curr_CompUsedColors.UsedColors_List.Remove(Curr_CompUsedColors.UsedColors_List.Single(x => x == Color1));
                }

                if (Curr_CompUsedColors.UsedColors_List.Count > 9)
                {
                    Curr_CompUsedColors.UsedColors_List.RemoveAt(0);
                }

                Curr_CompUsedColors.UsedColors_List.Add(Color1);

                Cmd_RefreshUsedColorsSVG();
            }
        }



        public void Cmd_RefreshUsedColorsSVG()
        {
            Curr_CompUsedColors.Refresh();
            StateHasChanged();
        }



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
            //Console.WriteLine("hit GetBoundingClientRect()");
            BPaintJsInterop.GetElementBoundingClientRect(ElementID, DotNetObjectReference.Create(this));
        }


        // TODO: implement labels data transfer to server-side
        // color=Red
        // category=GUI issue=none estimate=12h

        [JSInvokable]
        public void invokeFromjs(string id, double rect_left, double rect_top, double rect_width, double rect_height, double window_scrollX, double window_scrollY)
        {
            //Console.WriteLine("hit invokeFromjs()");
            //Console.WriteLine("id = " + id + "; left = " + rect_left + "; top = " + rect_top + "; width = " + rect_width + "; height = " + rect_height + "; w_scrollX = " + window_scrollX + "; w_scrollY = " + window_scrollY);

            //LocalData.SVGPosition = new PointD(rect_left + window_scrollX, rect_top + window_scrollY);
            LocalData.SVGPosition = new PointD(rect_left, rect_top);
        }
    }
}
