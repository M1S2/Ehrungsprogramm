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

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Print details of a single person.
        /// </summary>
        /// <param name="person"><see cref="Person"/> that should be printed</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        public async Task<bool> PrintPerson(Person person, string pdfFilePath)
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
                        document.Add(new Paragraph("Person Details").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));

                        Dictionary<string, string> personBasicDetailValues = new Dictionary<string, string>();
                        personBasicDetailValues.Add("Name: ", person.FirstName + " " + person.Name);
                        personBasicDetailValues.Add("Geburtsdatum: ", person.BirthDate.ToShortDateString());
                        personBasicDetailValues.Add("Eintrittsdatum: ", person.EntryDate.ToShortDateString());
                        personBasicDetailValues.Add("Mitgliedsjahre: ", person.MembershipYears.ToString());
                        personBasicDetailValues.Add("BLSV Punkte: ", person.ScoreBLSV.ToString());
                        personBasicDetailValues.Add("TSV Punkte: ", person.ScoreTSV.ToString());
                        personBasicDetailValues.Add("Effektive Jahre Vorstand: ", person.EffectiveBoardMemberYears.ToString());
                        personBasicDetailValues.Add("Effektive Jahre Abteilungsleitung: ", person.EffectiveHeadOfDepartementYears.ToString());
                        personBasicDetailValues.Add("Effektive Jahre andere Funktion: ", person.EffectiveOtherFunctionsYears.ToString());

                        Table tableBasicDetails = new Table(2, false);      // 2 columns for: Parameter, Value
                        foreach (KeyValuePair<string, string> personBasicDetailValue in personBasicDetailValues)
                        {
                            tableBasicDetails.AddCell(new Cell(1, 1).Add(new Paragraph(personBasicDetailValue.Key)));
                            tableBasicDetails.AddCell(new Cell(1, 1).Add(new Paragraph(personBasicDetailValue.Value)));
                        }
                        document.Add(tableBasicDetails);

                        // ------------------------------------------------------------------------------------------------------------------

                        document.Add(new Paragraph("Funktionen").SetFontSize(20));
                        Table tableFunctions = new Table(3, false);      // 3 columns for: Description, Time Period, Years in function
                        tableFunctions.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Funktion")));
                        tableFunctions.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Zeitraum")));
                        tableFunctions.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Jahre")));
                        foreach (Function function in person.Functions)
                        {
                            tableFunctions.AddCell(new Cell(1, 1).Add(new Paragraph(function.Description)));
                            tableFunctions.AddCell(new Cell(1, 1).Add(new Paragraph(function.TimePeriod.Start.ToShortDateString() + " - " + function.TimePeriod.End.ToShortDateString())));
                            tableFunctions.AddCell(new Cell(1, 1).Add(new Paragraph(function.FunctionYears.ToString())));
                        }
                        document.Add(tableFunctions);

                        // ------------------------------------------------------------------------------------------------------------------

                        document.Add(new Paragraph("Ehrungen").SetFontSize(20));
                        Table tableRewards = new Table(2, false);      // 2 columns for: Description, Status
                        tableRewards.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Beschreibung")));
                        tableRewards.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Status")));
                        foreach (Reward reward in person.Rewards.Rewards)
                        {
                            if (reward.Available || reward.Obtained)
                            {
                                tableRewards.AddCell(new Cell(1, 1).Add(new Paragraph(reward.Description ?? rewardTypeToString(reward.Type))));
                                tableRewards.AddCell(new Cell(1, 1).Add(new Paragraph(reward.Obtained ? ("Erhalten am: " + reward.ObtainedDate.ToShortDateString()) : "Verfügbar")));
                            }
                        }
                        document.Add(tableRewards);

                        writeDocumentExportDatePageNumbers(document);
                        document.Close();
                    }
                    printingResult = true;
                }
                catch (Exception ex)
                {
                    printingResult = false;
                    throw ex;
                }
            });
            return printingResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

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

                        writeDocumentExportDatePageNumbers(document);
                        document.Close();
                    }
                    printingResult = true;
                }
                catch (Exception ex)
                {
                    printingResult = false;
                    throw ex;
                }
            });
            return printingResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Print an overview of all rewards.
        /// </summary>
        /// <param name="people">List with all available <see cref="Person"/> objects used to generate the rewards overview</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        public async Task<bool> PrintRewards(List<Person> people, string pdfFilePath)
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
                        document.Add(new Paragraph("TSV Ehrungen Übersicht").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));
                        document.Add(new Paragraph("Nur die neuesten, noch nicht erhaltenen Ehrungen sind aufgeführt." + Environment.NewLine));

                        List<Person> peopleTsvRewardAvailable = people.Where(p => p.Rewards.HighestAvailableTSVReward != null).ToList();
                        List <IGrouping<RewardTypes, Person>> tsvRewardGroups = peopleTsvRewardAvailable.OrderBy(p => p.Name).
                                                                                        OrderBy(p => p.Rewards.HighestAvailableTSVReward.Type).
                                                                                        GroupBy(p => p.Rewards.HighestAvailableTSVReward.Type).ToList();

                        document.Add(new Paragraph("Anzahl TSV Ehrungen: " + peopleTsvRewardAvailable.Count.ToString()));

                        Table tableTsv = new Table(4, false);      // 4 columns for: ID, Name, First Name, Score
                        tableTsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("ID")));
                        tableTsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Name")));
                        tableTsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Vorname")));
                        tableTsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Punkte")));

                        int counter = 0;
                        foreach (IGrouping<RewardTypes, Person> tsvRewardGroup in tsvRewardGroups)
                        {
                            string tsvRewardName = rewardTypeToString(tsvRewardGroup.Key);
                            tableTsv.AddCell(new Cell(1, 4).SetBackgroundColor(ColorConstants.GRAY).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph(tsvRewardName)));
                            foreach (Person person in tsvRewardGroup)
                            {
                                tableTsv.AddCell(new Cell(1, 1).Add(new Paragraph((++counter).ToString())));
                                tableTsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.Name)));
                                tableTsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.FirstName)));
                                tableTsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.ScoreTSV.ToString())));
                            }
                        }
                        document.Add(tableTsv);
                        document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                        // ------------------------------------------------------------------------------------------------------------------

                        document.Add(new Paragraph("BLSV Ehrungen Übersicht").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));
                        document.Add(new Paragraph("Nur die neuesten, noch nicht erhaltenen Ehrungen sind aufgeführt." + Environment.NewLine));

                        List<Person> peopleBlsvRewardAvailable = people.Where(p => p.Rewards.HighestAvailableBLSVReward != null).ToList();
                        List<IGrouping<RewardTypes, Person>> blsvRewardGroups = peopleBlsvRewardAvailable.OrderBy(p => p.Name).
                                                                                        OrderBy(p => p.Rewards.HighestAvailableBLSVReward.Type).
                                                                                        GroupBy(p => p.Rewards.HighestAvailableBLSVReward.Type).ToList();

                        document.Add(new Paragraph("Anzahl BLSV Ehrungen: " + peopleBlsvRewardAvailable.Count.ToString()));

                        Table tableBlsv = new Table(4, false);      // 4 columns for: ID, Name, First Name, Score
                        tableBlsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("ID")));
                        tableBlsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Name")));
                        tableBlsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Vorname")));
                        tableBlsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Punkte")));

                        counter = 0;
                        foreach (IGrouping<RewardTypes, Person> blsvRewardGroup in blsvRewardGroups)
                        {
                            tableBlsv.AddCell(new Cell(1, 4).SetBackgroundColor(ColorConstants.GRAY).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph(blsvRewardGroup.Key.ToString())));
                            foreach (Person person in blsvRewardGroup)
                            {
                                tableBlsv.AddCell(new Cell(1, 1).Add(new Paragraph((++counter).ToString())));
                                tableBlsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.Name)));
                                tableBlsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.FirstName)));
                                tableBlsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.ScoreBLSV.ToString())));
                            }
                        }
                        document.Add(tableBlsv);

                        writeDocumentExportDatePageNumbers(document);
                        document.Close();
                    }
                    printingResult = true;
                }
                catch (Exception ex)
                {
                    printingResult = false;
                    throw ex;
                }
            });
            return printingResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Write the export date (DateTime.Now) and the page numbers to the document
        /// </summary>
        /// <param name="document">Document to which the export date and page numbers are added</param>
        private void writeDocumentExportDatePageNumbers(Document document)
        {
            DateTime exportDate = DateTime.Now;
            // Page numbers and export date
            int numPages = document.GetPdfDocument().GetNumberOfPages();
            for (int i = 1; i <= numPages; i++)
            {
                document.ShowTextAligned(new Paragraph("Export Datum: " + exportDate.ToString()), 20, 20, i, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);
                document.ShowTextAligned(new Paragraph(string.Format("Seite {0} von {1}", i, numPages)), 559, 20, i, TextAlignment.RIGHT, VerticalAlignment.BOTTOM, 0);
            }
        }

        /// <summary>
        /// Convert the given <see cref="RewardTypes"/> to a string
        /// </summary>
        /// <param name="rewardType">Reward type to convert to a string</param>
        private string rewardTypeToString(RewardTypes rewardType)
        {
            string rewardName = rewardType.ToString();
            switch (rewardType)
            {
                case RewardTypes.TSVSILVER: rewardName = "TSV Silber"; break;
                case RewardTypes.TSVGOLD: rewardName = "TSV Gold"; break;
                case RewardTypes.TSVHONORARY: rewardName = "TSV Ehrenmitglied"; break;
                default: break;
            }
            return rewardName;
        }
    }
}
