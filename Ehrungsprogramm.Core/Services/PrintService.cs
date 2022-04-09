using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ehrungsprogramm.Core.Contracts.Services;
using Ehrungsprogramm.Core.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Win32;

namespace Ehrungsprogramm.Core.Services
{
    /// <summary>
    /// Service used to print various objects
    /// </summary>
    /// <see https://www.codeguru.com/dotnet/generating-a-pdf-document-using-c-net-and-itext-7/ for examples of iText7 usage/>
    public class PrintService : IPrintService
    {
        /// <summary>
        /// Constructor of the <see cref="PrintService"/>
        /// </summary>
        public PrintService()
        {
        }

        /// <summary>
        /// Print details of a single person.
        /// </summary>
        /// <param name="person"><see cref="Person"/> that should be printed</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        public Task<bool> PrintPerson(Person person, string pdfFilePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Print an overview list of all people.
        /// </summary>
        /// <param name="people">List with all available <see cref="Person"/> objects</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        /// <returns>true if printing succeeded; false if printing failed</returns>
        public async Task<bool> PrintPersonList(List<Person> people, string pdfFilePath)
        {
            bool printingResult = false;
            await Task.Run(() =>
            {
                try
                {
                    using (PdfWriter writer = new PdfWriter(pdfFilePath))
                    using (PdfDocument pdf = new PdfDocument(writer))
                    using (Document document = new Document(pdf, PageSize.A4, false))
                    {
                        document.Add(new Paragraph("Personen Übersicht").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));
                        document.Add(new Paragraph("Anzahl: " + people.Count.ToString() + " Personen"));

                        Table table = new Table(6, false);      // 6 columns for: ID, Name, First Name, Entry Date, BLSV Score, TSV Score
                        table.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("ID")));
                        table.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Name")));
                        table.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Vorname")));
                        table.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Eintrittsdatum")));
                        table.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("BLSV Punkte")));
                        table.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("TSV Punkte")));

                        foreach (Person person in people)
                        {
                            table.AddCell(new Cell(1, 1).Add(new Paragraph(person.Id.ToString())));
                            table.AddCell(new Cell(1, 1).Add(new Paragraph(person.Name)));
                            table.AddCell(new Cell(1, 1).Add(new Paragraph(person.FirstName)));
                            table.AddCell(new Cell(1, 1).Add(new Paragraph(person.EntryDate.ToShortDateString())));
                            table.AddCell(new Cell(1, 1).Add(new Paragraph(person.ScoreBLSV.ToString())));
                            table.AddCell(new Cell(1, 1).Add(new Paragraph(person.ScoreTSV.ToString())));
                        }

                        document.Add(table);

                        DateTime exportDate = DateTime.Now;
                        // Page numbers and export date
                        int numPages = pdf.GetNumberOfPages();
                        for (int i = 1; i <= numPages; i++)
                        {
                            document.ShowTextAligned(new Paragraph("Export Datum: " + exportDate.ToString()), 20, 20, i, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);
                            document.ShowTextAligned(new Paragraph(string.Format("Seite {0} von {1}", i, numPages)), 559, 20, i, TextAlignment.RIGHT, VerticalAlignment.BOTTOM, 0);
                        }

                        document.Close();
                    }
                    printingResult = true;
                }
                catch (Exception)
                {
                    printingResult = false;
                }
            });
            return printingResult;
        }

        /// <summary>
        /// Print an overview of all rewards.
        /// </summary>
        /// <param name="people">List with all available <see cref="Person"/> objects used to generate the rewards overview</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        public Task<bool> PrintRewards(List<Person> people, string pdfFilePath)
        {
            throw new NotImplementedException();
        }
    }
}
