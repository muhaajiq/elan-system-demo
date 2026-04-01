using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.JSONConstants;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using static MHA.ELAN.Framework.Constants.ConstantHelper.SPColumn;

namespace MHA.ELAN.Framework.Helpers
{
    public static class ReceiptPdfHelper
    {
        private static readonly JSONAppSettings appSettings;

        static ReceiptPdfHelper()
        {
            appSettings = ConfigurationManager.GetAppSetting();
        }

        public static byte[] GenerateITDepartReceiptPdf(ViewModelFinalEmployeeDetails employee, List<ViewModelFinalHardware> hardwares)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new PdfWriter(ms))
                using (var pdf = new PdfDocument(writer))
                using (var document = new Document(pdf, PageSize.A4))
                {
                    document.SetMargins(20, 20, 20, 20);
                    Color headerBg = new DeviceRgb(230, 230, 230);

                    // ===== Title =====
                    document.Add(new Paragraph("IT Equipment Acknowledgement")
                        .SimulateBold()
                        .SetFontSize(14)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(15));

                    // ===== Staff Information =====
                    document.Add(new Paragraph("Staff Information")
                        .SimulateBold()
                        .SetFontSize(12)
                        .SetMarginBottom(2));

                    document.Add(new LineSeparator(new SolidLine(1)).SetMarginBottom(5));

                    Table staffTable = new Table(UnitValue.CreatePercentArray(new float[] { 20, 30, 20, 30 }))
                        .UseAllAvailableWidth()
                        .SetFontSize(10);

                    staffTable.AddCell(CreateLabelCell("Employee Name"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.EmployeeName ?? ""));
                    staffTable.AddCell(CreateLabelCell("Employee ID"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.EmployeeID ?? ""));

                    staffTable.AddCell(CreateLabelCell("Department"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.DepartmentTitle ?? ""));
                    staffTable.AddCell(CreateLabelCell("Designation"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.DesignationTitle ?? ""));

                    staffTable.AddCell(CreateLabelCell("Email"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.EmployeeEmail ?? ""));
                    staffTable.AddCell(CreateLabelCell("Date Joined"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.JoinDate?.ToString("dd/MM/yyyy") ?? ""));

                    document.Add(staffTable);
                    document.Add(new Paragraph(" "));

                    // ===== Items Table =====
                    document.Add(new Paragraph("Assigned Equipment")
                        .SimulateBold()
                        .SetFontSize(12)
                        .SetMarginBottom(2));

                    document.Add(new LineSeparator(new SolidLine(1)).SetMarginBottom(5));

                    Table itemTable = new Table(UnitValue.CreatePercentArray(new float[] { 5, 25, 15, 20, 20 }))
                        .UseAllAvailableWidth()
                        .SetFontSize(9);

                    string[] headers = { "#", "Item", "Model", "Serial Number", "Date Assigned" };
                    foreach (var h in headers)
                    {
                        itemTable.AddHeaderCell(new Cell()
                            .Add(new Paragraph(h).SimulateBold())
                            .SetBackgroundColor(headerBg)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetPadding(4));
                    }

                    int counter = 1;
                    foreach (var req in hardwares)
                    {
                        itemTable.AddCell(CreateCenteredCell(counter.ToString()));
                        itemTable.AddCell(CreateCenteredCell(req.Final_Hardware.ItemTitle));
                        itemTable.AddCell(CreateCenteredCell(req.Final_Hardware.Model));
                        itemTable.AddCell(CreateCenteredCell(req.Final_Hardware.SerialNumber));
                        itemTable.AddCell(CreateCenteredCell(FormatDate(req.Final_Hardware.DateAssigned)));
                        counter++;
                    }

                    document.Add(itemTable);

                    // ===== Terms & Responsibilities =====
                    document.Add(new Paragraph("Acknowledgement")
                        .SimulateBold()
                        .SetFontSize(12)
                        .SetMarginTop(15)
                        .SetMarginBottom(5));

                    List bulletList = new List()
                        .SetSymbolIndent(12)
                        .SetListSymbol("\u2022")
                        .SetFontSize(10);

                    bulletList.Add(new ListItem("I understand and agree that it is my responsibility to take good care of the item(s) and to return them at the end of my service with the company."));
                    bulletList.Add(new ListItem("I shall compensate the Company the value of RM 1,500 per unit as replacement costs should any of the items be stolen or lost, accompanied by the original copy of the police report."));
                    bulletList.Add(new ListItem("I hereby authorize the Company to deduct the amount of RM 1,500 per unit from my current salary for the item(s) lost or stolen."));
                    bulletList.Add(new ListItem("I have read and understand the NOTEBOOK/SMART DEVICE USAGE GUIDELINES and I agree to abide by the terms and conditions upon receipt and usage of the item(s)."));

                    document.Add(bulletList);

                    document.Add(new Paragraph("Please be advised that you are not allowed to install any other additional unlicensed software.")
                        .SetFontSize(10)
                        .SetMarginTop(5));

                    // ===== Disclaimer (instead of signature) =====
                    document.Add(new Paragraph("No signature required. This is a computer-generated document.")
                        .SetFontSize(9)
                        .SimulateItalic()
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginTop(25));
                }
                return ms.ToArray();
            }
        }

        public static byte[] GenerateNormalReceiptPdf(ViewModelFinalEmployeeDetails employee, List<ViewModelFinalHardware> hardwares, string refNo)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new PdfWriter(ms))
                using (var pdf = new PdfDocument(writer))
                using (var document = new Document(pdf, PageSize.A4))
                {
                    document.SetMargins(20, 20, 20, 20);

                    Color headerBg = new DeviceRgb(230, 230, 230);

                    // ===== Header =====
                    document.Add(new Paragraph("Property Collection Acknowledgement")
                        .SimulateBold()
                        .SetFontSize(14)
                        .SetTextAlignment(TextAlignment.CENTER));

                    //TODO
                    document.Add(new Paragraph(" "));
                    string initiatorEmailRaw = employee.Final_EmployeeDetails.CreatedByLogin ?? "";
                    string initiatorEmail = initiatorEmailRaw.Contains("|")
                        ? initiatorEmailRaw.Split('|').Last()
                        : initiatorEmailRaw;

                    document.Add(new Paragraph($"Initiator: {employee.Final_EmployeeDetails.CreatedBy ?? ""}").SetFontSize(10));
                    document.Add(new Paragraph($"Initiator Email: {initiatorEmail}").SetFontSize(10));
                    document.Add(new Paragraph(" "));

                    // ===== Staff Information =====
                    document.Add(new Paragraph("Staff Information")
                        .SimulateBold()
                        .SetFontSize(12)
                        .SetMarginBottom(2));

                    document.Add(new LineSeparator(new SolidLine(1)).SetMarginBottom(5));

                    Table staffTable = new Table(UnitValue.CreatePercentArray(new float[] { 20, 30, 20, 30 }))
                        .UseAllAvailableWidth()
                        .SetFontSize(10);

                    staffTable.AddCell(CreateLabelCell("Reference No."));
                    staffTable.AddCell(CreateValueCell(refNo ?? ""));
                    staffTable.AddCell(CreateLabelCell("Employee Name"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.EmployeeName ?? ""));

                    staffTable.AddCell(CreateLabelCell("Employee ID"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.EmployeeID ?? ""));
                    staffTable.AddCell(CreateLabelCell("Date Joined"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.JoinDate?.ToString("dd/MM/yyyy") ?? ""));

                    staffTable.AddCell(CreateLabelCell("Department"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.DepartmentTitle ?? ""));
                    staffTable.AddCell(CreateLabelCell("Email"));
                    staffTable.AddCell(CreateValueCell((employee.Final_EmployeeDetails.EmployeeEmail ?? "").Trim().ToLower()));

                    staffTable.AddCell(CreateLabelCell("Designation"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.DesignationTitle ?? ""));
                    staffTable.AddCell(new Cell(1, 2).SetBorder(Border.NO_BORDER));

                    document.Add(staffTable);
                    document.Add(new Paragraph(" "));

                    // ===== Items Table =====
                    Table itemTable = new Table(UnitValue.CreatePercentArray(new float[] { 5, 25, 8, 15, 27, 20 }))
                        .UseAllAvailableWidth()
                        .SetFontSize(9);

                    // Header row
                    string[] headers = { "#", "Item", "Qty", "Date Assigned", "Remarks", "Date Received" };
                    foreach (var h in headers)
                    {
                        itemTable.AddHeaderCell(new Cell()
                            .Add(new Paragraph(h).SimulateBold())
                            .SetBackgroundColor(headerBg)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetPadding(4));
                    }

                    // Data rows
                    int counter = 1;
                    foreach (var req in hardwares)
                    {
                        itemTable.AddCell(CreateCenteredCell(counter.ToString()));
                        itemTable.AddCell(CreateCenteredCell(req.Final_Hardware.ItemTitle));
                        itemTable.AddCell(CreateCenteredCell(req.Final_Hardware.Quantity.ToString()));
                        itemTable.AddCell(CreateCenteredCell(FormatDate(req.Final_Hardware.DateAssigned)));
                        itemTable.AddCell(CreateCenteredCell(req.Final_Hardware.RemarkHistory));
                        itemTable.AddCell(CreateCenteredCell(DateTime.Now.ToString(appSettings.DefaultDateFormat)));
                        counter++;
                    }

                    document.Add(itemTable);

                    // ===== Terms and Conditions =====
                    document.Add(new Paragraph("Terms and Conditions")
                        .SimulateBold()
                        .SetFontSize(10)
                        .SetMarginTop(12)
                        .SetMarginBottom(2));
                    document.Add(new Paragraph("I have read and hereby agree with the Terms and Conditions.")
                        .SetFontSize(9));

                    // ===== Disclaimer (instead of signature) =====
                    document.Add(new Paragraph("No signature required. This is a computer-generated document.")
                        .SetFontSize(9)
                        .SimulateItalic()
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginTop(25));
                }
                return ms.ToArray();
            }
        }

        public static byte[] GenerateManagerReceiptPdf(ViewModelFinalEmployeeDetails employee, List<ViewModelFinalHardware> hardwares)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new PdfWriter(ms))
                using (var pdf = new PdfDocument(writer))
                using (var document = new Document(pdf, PageSize.A4))
                {
                    document.SetMargins(20, 20, 20, 20);
                    Color headerBg = new DeviceRgb(230, 230, 230);

                    // ===== Header =====
                    document.Add(new Paragraph("Property Return Acknowledgement (Reporting Manager Copy)")
                        .SimulateBold()
                        .SetFontSize(14)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(15));

                    // ===== Manager Information =====
                    document.Add(new Paragraph("Reporting Manager Information")
                        .SimulateBold()
                        .SetFontSize(12)
                        .SetMarginBottom(2));

                    document.Add(new LineSeparator(new SolidLine(1)).SetMarginBottom(5));

                    Table managerTable = new Table(UnitValue.CreatePercentArray(new float[] { 20, 30, 20, 30 }))
                        .UseAllAvailableWidth()
                        .SetFontSize(10);

                    managerTable.AddCell(CreateLabelCell("Manager Name"));
                    managerTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.ReportingManagerName ?? ""));
                    managerTable.AddCell(CreateLabelCell("Manager Email"));
                    managerTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.ReportingManagerEmail ?? ""));

                    document.Add(managerTable);
                    document.Add(new Paragraph(" "));

                    // ===== Resigned Employee Information =====
                    document.Add(new Paragraph("Resigned Employee Information")
                        .SimulateBold()
                        .SetFontSize(12)
                        .SetMarginBottom(2));

                    document.Add(new LineSeparator(new SolidLine(1)).SetMarginBottom(5));

                    Table staffTable = new Table(UnitValue.CreatePercentArray(new float[] { 20, 30, 20, 30 }))
                        .UseAllAvailableWidth()
                        .SetFontSize(10);

                    staffTable.AddCell(CreateLabelCell("Employee Name"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.EmployeeName ?? ""));
                    staffTable.AddCell(CreateLabelCell("Employee ID"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.EmployeeID ?? ""));

                    staffTable.AddCell(CreateLabelCell("Department"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.DepartmentTitle ?? ""));
                    staffTable.AddCell(CreateLabelCell("Designation"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.DesignationTitle ?? ""));

                    staffTable.AddCell(CreateLabelCell("Date Joined"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.JoinDate?.ToString(ConstantHelper.DateFormat.DefaultDateFormat) ?? ""));
                    staffTable.AddCell(CreateLabelCell("Last Working Day"));
                    staffTable.AddCell(CreateValueCell(employee.Final_EmployeeDetails.EndDate?.ToString(ConstantHelper.DateFormat.DefaultDateFormat) ?? ""));

                    document.Add(staffTable);
                    document.Add(new Paragraph(" "));

                    // ===== Returned Items =====
                    document.Add(new Paragraph("Returned Company Property")
                        .SimulateBold()
                        .SetFontSize(12)
                        .SetMarginBottom(2));

                    document.Add(new LineSeparator(new SolidLine(1)).SetMarginBottom(5));

                    Table itemTable = new Table(UnitValue.CreatePercentArray(new float[] { 5, 25, 15, 20, 20 }))
                        .UseAllAvailableWidth()
                        .SetFontSize(9);

                    string[] headers = { "#", "Item", "Model", "Serial Number", "Date Returned" };
                    foreach (var h in headers)
                    {
                        itemTable.AddHeaderCell(new Cell()
                            .Add(new Paragraph(h).SimulateBold())
                            .SetBackgroundColor(headerBg)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetPadding(4));
                    }

                    int counter = 1;
                    foreach (var req in hardwares)
                    {
                        itemTable.AddCell(CreateCenteredCell(counter.ToString()));
                        itemTable.AddCell(CreateCenteredCell(req.Final_Hardware.ItemTitle));
                        itemTable.AddCell(CreateCenteredCell(req.Final_Hardware.Model));
                        itemTable.AddCell(CreateCenteredCell(req.Final_Hardware.SerialNumber));
                        itemTable.AddCell(CreateCenteredCell(FormatDate(req.Final_Hardware.DateReturned)));
                        counter++;
                    }

                    document.Add(itemTable);

                    // ===== Confirmation Note =====
                    document.Add(new Paragraph("Confirmation")
                        .SimulateBold()
                        .SetFontSize(12)
                        .SetMarginTop(15)
                        .SetMarginBottom(5));

                    document.Add(new Paragraph(
                        "This document serves as confirmation that the above employee has returned all assigned company property. " +
                        "No further action is required from the reporting manager regarding these items.")
                        .SetFontSize(10));

                    // ===== Disclaimer (instead of signature) =====
                    document.Add(new Paragraph("No signature required. This is a computer-generated document.")
                        .SetFontSize(9)
                        .SimulateItalic()
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginTop(25));
                }
                return ms.ToArray();
            }
        }


        #region Helper methods
        private static string FormatDate(DateTime? date) =>
            date.HasValue && date.Value.Year > 1 ? date.Value.ToString(appSettings.DefaultDateFormat) : "";

        private static Cell CreateLabelCell(string text) =>
            new Cell().Add(new Paragraph(text ?? "").SimulateBold())
                      .SetPadding(3)
                      .SetBorder(Border.NO_BORDER);

        private static Cell CreateValueCell(string text) =>
            new Cell().Add(new Paragraph(text ?? "").SetTextAlignment(TextAlignment.LEFT).SetMultipliedLeading(1f))
                      .SetPadding(3)
                      .SetBorder(Border.NO_BORDER);

        private static Cell CreateCenteredCell(string text) =>
            new Cell().Add(new Paragraph(text ?? "").SetTextAlignment(TextAlignment.CENTER))
                      .SetPadding(3);
        #endregion

    }
}
