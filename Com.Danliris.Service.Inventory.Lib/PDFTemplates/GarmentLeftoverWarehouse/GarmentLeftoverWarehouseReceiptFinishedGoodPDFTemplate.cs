using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.PDFTemplates.GarmentLeftoverWarehouse
{
    public class GarmentLeftoverWarehouseReceiptFinishedGoodPDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(GarmentLeftoverWarehouseReceiptFinishedGoodViewModel viewModel)
        {
            const int MARGIN = 20;
            var timeoffset = 7;

            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 14);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font body_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font normal_font_underlined = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8, Font.UNDERLINE);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);

            Document document = new Document(PageSize.A5.Rotate(), MARGIN, MARGIN, MARGIN, 70);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            writer.PageEvent = new GarmentLeftoverWarehouseReceiptSignPDFTemplatePageEvent();

            document.Open();
            #region TITLE
            Paragraph title = new Paragraph("BON TERIMA BARANG JADI GUDANG SISA", header_font);
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);

            Paragraph danliris = new Paragraph("PT. DANLIRIS", bold_font);
            document.Add(danliris);
            Paragraph address = new Paragraph("BANARAN, GROGOL, SUKOHARJO", normal_font);
            document.Add(address);

            Paragraph p = new Paragraph(new Chunk(new LineSeparator(0.5F, 100.0F, BaseColor.Black, Element.ALIGN_LEFT, 1)));
            p.SpacingBefore = -10f;
            document.Add(p);
            #endregion

            #region HEADER
            PdfPTable tableHeader = new PdfPTable(6);
            tableHeader.WidthPercentage = 100;
            tableHeader.HorizontalAlignment = Element.ALIGN_LEFT;
            tableHeader.SetWidths(new float[] { 2f, 0.1f, 5f, 2f, 0.1f, 4f });

            PdfPCell cellHeaderContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellHeaderContentLeft.Phrase = new Phrase("NO", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(":", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(viewModel.FinishedGoodReceiptNo, bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);

            cellHeaderContentLeft.Phrase = new Phrase("Terima dari", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(":", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(viewModel.UnitFrom.Name, normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);

            cellHeaderContentLeft.Phrase = new Phrase("Tanggal", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(":", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(viewModel.ReceiptDate.GetValueOrDefault().ToOffset(new TimeSpan(timeoffset, 0, 0)).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("en-EN")), normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);

            cellHeaderContentLeft.Phrase = new Phrase("", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase("", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase("", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);



            tableHeader.SpacingAfter = 15f;
            document.Add(tableHeader);

            #endregion

            #region BODY
            PdfPTable bodyTable = new PdfPTable(6);
            bodyTable.WidthPercentage = 100;
            bodyTable.SetWidths(new float[] { 1f, 3f, 3f, 6f, 3f, 3f });

            PdfPCell bodyTableCellRightBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
            PdfPCell bodyTableCellLeftBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell bodyTableCellCenterBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

            bodyTableCellCenterBorder.Phrase = new Phrase("NO", bold_font);
            bodyTable.AddCell(bodyTableCellCenterBorder);
            bodyTableCellCenterBorder.Phrase = new Phrase("DASAR TERIMA", bold_font);
            bodyTable.AddCell(bodyTableCellCenterBorder);
            bodyTableCellCenterBorder.Phrase = new Phrase("NO RO", bold_font);
            bodyTable.AddCell(bodyTableCellCenterBorder);
            bodyTableCellCenterBorder.Phrase = new Phrase("KOMODITI", bold_font);
            bodyTable.AddCell(bodyTableCellCenterBorder);
            bodyTableCellCenterBorder.Phrase = new Phrase("QTY", bold_font);
            bodyTable.AddCell(bodyTableCellCenterBorder);
            bodyTableCellCenterBorder.Phrase = new Phrase("SAT", bold_font);
            bodyTable.AddCell(bodyTableCellCenterBorder);

            //Dictionary<string, ro> sameRO = new Dictionary<string, ro>();
            //int count = 1;
            //foreach(var item in viewModel.Items)
            //{
            //    if (sameRO.Count == 0)
            //    {
            //        sameRO.Add(item.ExpenditureGoodNo,new ro
            //        {
            //            ExpenditureGoodNo= item.ExpenditureGoodNo,
            //            RONo=item.RONo,
            //            count=count
            //        });
            //    }
            //    else
            //    {
            //        if (sameRO.ContainsKey(item.ExpenditureGoodNo))
            //        {
            //            sameRO[item.ExpenditureGoodNo] = new ro
            //            {
            //                ExpenditureGoodNo = item.ExpenditureGoodNo,
            //                RONo = item.RONo,
            //                count =count++
            //            };
            //        }
            //        else
            //        {
            //            count = 1;
            //            sameRO.Add(item.ExpenditureGoodNo, new ro
            //            {
            //                ExpenditureGoodNo = item.ExpenditureGoodNo,
            //                RONo = item.RONo,
            //                count = count
            //            });
            //        }
            //    }
            //}

            int index = 0;
            double totalQuantity = 0;
            foreach(var detail in viewModel.Items)
            {
                index++;
                bodyTableCellLeftBorder.Phrase = new Phrase(index.ToString(), normal_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);
                bodyTableCellLeftBorder.Phrase = new Phrase(detail.ExpenditureGoodNo, normal_font);
                //bodyTableCellLeftBorder.Rowspan = item.count;
                bodyTable.AddCell(bodyTableCellLeftBorder);
                bodyTableCellLeftBorder.Phrase = new Phrase(detail.RONo, normal_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);
                bodyTableCellLeftBorder.Phrase = new Phrase(detail.LeftoverComodity.Name, normal_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);
                bodyTableCellRightBorder.Phrase = new Phrase(detail.Quantity.ToString(), normal_font);
                bodyTable.AddCell(bodyTableCellRightBorder);
                bodyTableCellLeftBorder.Phrase = new Phrase("PCS", normal_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);

                totalQuantity += detail.Quantity;
                   
            }
                
            bodyTableCellRightBorder.Phrase = new Phrase("TOTAL", bold_font);
            bodyTableCellRightBorder.Colspan = 4;
            bodyTable.AddCell(bodyTableCellRightBorder);
            bodyTableCellRightBorder.Phrase = new Phrase(totalQuantity.ToString(), bold_font);
            bodyTableCellRightBorder.Colspan = 1;
            bodyTable.AddCell(bodyTableCellRightBorder);
            bodyTableCellLeftBorder.Phrase = new Phrase("PCS", bold_font);
            bodyTable.AddCell(bodyTableCellLeftBorder);

            bodyTable.SpacingAfter = 25f;
            document.Add(bodyTable);
            #endregion

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        //private class ro
        //{
        //    public string RONo { get; set; }
        //    public string ExpenditureGoodNo { get; set; }
        //    public int count { get; set; }
        //}
    }
}
