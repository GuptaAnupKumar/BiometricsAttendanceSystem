﻿using DocumentFormat.OpenXml.Drawing.Charts;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Reports_DailyAttendanceBasicReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            BindDropDowns();
    }
    protected void Calendar1_SelectionChanged(object sender, EventArgs e)
    {
        txt_date.Text = Calendar1.SelectedDate.Date.ToString("d");
    }
    protected void btn_report_Click(object sender, EventArgs e)
    {
        
        ManageReports objManageReports = new ManageReports();
        TimeSpan relaxationTime = new TimeSpan();
        relaxationTime = TimeSpan.Parse(ddlRelaxation.SelectedValue.ToString());
        var data = objManageReports.GetDataForDailyAttendanceReport(Convert.ToInt32(ddlDepartments.SelectedValue.ToString()), Calendar1.SelectedDate.Date, relaxationTime);
        grid_dailyAttendance.DataSource = data;
        grid_dailyAttendance.DataBind();
        btnExport.Visible = true;
        btn_export_xl.Visible = true;


    }
    protected void BindDropDowns()
    {
        MasterEntries objMasterEntries = new MasterEntries();
        ddlDepartments.DataSource = objMasterEntries.GetAllDepartments();
        ddlDepartments.DataTextField = "Name";
        ddlDepartments.DataValueField = "Id";
        ddlDepartments.DataBind();


    }


    protected void btnExport_Click(object sender, EventArgs e)
    {
        using (StringWriter sw = new StringWriter())
        {
            using (HtmlTextWriter hw = new HtmlTextWriter(sw))
            {
                //To Export all pages
                grid_dailyAttendance.AllowPaging = false;
                // this.BindGrid();

                grid_dailyAttendance.RenderBeginTag(hw);
                grid_dailyAttendance.HeaderRow.RenderControl(hw);
                foreach (GridViewRow row in grid_dailyAttendance.Rows)
                {
                    row.RenderControl(hw);
                }
                grid_dailyAttendance.FooterRow.RenderControl(hw);
                grid_dailyAttendance.RenderEndTag(hw);
                StringReader sr = new StringReader(sw.ToString());
                Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();
                htmlparser.Parse(sr);
                pdfDoc.Close();

                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=Report.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();
            }
        }
    }

    protected void btn_export_xl_Click(object sender, EventArgs e)
    {
        Utility.GenerateExcel(grid_dailyAttendance,"AKGEC Daily Attendance Report", "Report", this);
    }
}