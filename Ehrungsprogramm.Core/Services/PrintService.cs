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
using iText.Kernel.Font;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Events;
using System.Reflection;
using Ehrungsprogramm.Core.Extensions;

namespace Ehrungsprogramm.Core.Services
{
    /// <summary>
    /// Service used to print various objects
    /// </summary>
    /// <see https://www.codeguru.com/dotnet/generating-a-pdf-document-using-c-net-and-itext-7/ for examples of iText7 usage/>
    public class PrintService : IPrintService
    {
        public static readonly Color TSVLIGHTBLUE = new DeviceRgb(33, 129, 255);

        private IPersonService _personService;

        /// <summary>
        /// Constructor of the <see cref="PrintService"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object used to get some parameters used to print the reports</param>
        public PrintService(IPersonService personService)
        {
            _personService = personService;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Print details of a single person.
        /// </summary>
        /// <param name="person"><see cref="Person"/> that should be printed</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        /// <returns>true if printing succeeded; false if printing failed</returns>
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
                        document.SetBottomMargin(65);
                        // Add events to handle the generation of headers and footers
                        pdf.AddEventHandler(PdfDocumentEvent.START_PAGE, new PageHeaderEventHandler());
                        pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new PageFooterEventHandler(_personService));

                        document.Add(new Paragraph(Properties.Resources.PrintPersonDetailsString).SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));
                        if (!String.IsNullOrEmpty(person.ParsingFailureMessage))
                        { 
                            document.Add(new Paragraph(Properties.Resources.PrintParsingErrorString + ": " + Environment.NewLine + person.ParsingFailureMessage));
                        }

                        Dictionary<string, string> personBasicDetailValues = new Dictionary<string, string>();
                        personBasicDetailValues.Add(Properties.Resources.PrintNameString + ": ", person.FirstName + " " + person.Name);
                        personBasicDetailValues.Add(Properties.Resources.PrintPersonIdString + ": ", person.PersonID.ToString());
                        personBasicDetailValues.Add(Properties.Resources.PrintBirthdateString + ": ", person.BirthDate.ToShortDateString());
                        personBasicDetailValues.Add(Properties.Resources.PrintEntrydateString + ": ", person.EntryDate.ToShortDateString());
                        personBasicDetailValues.Add(Properties.Resources.PrintMembershipYearsString + ": ", person.MembershipYears.ToString());
                        personBasicDetailValues.Add(Properties.Resources.PrintDepartementsString + ": ", person.Departements);
                        personBasicDetailValues.Add(Properties.Resources.PrintBLSVScoreString + ": ", person.ScoreBLSV.ToString());
                        personBasicDetailValues.Add(Properties.Resources.PrintTSVScoreString + ": ", person.ScoreTSV.ToString());
                        personBasicDetailValues.Add(Properties.Resources.PrintEffectiveYearsBoardMemberString + ": ", person.EffectiveBoardMemberYears.ToString());
                        personBasicDetailValues.Add(Properties.Resources.PrintEffectiveYearsHeadOfDepartementString + ": ", person.EffectiveHeadOfDepartementYears.ToString());
                        personBasicDetailValues.Add(Properties.Resources.PrintEffectiveYearsOtherFunctionString + ": ", person.EffectiveOtherFunctionsYears.ToString());
                        
                        Table tableBasicDetails = new Table(2, false);      // 2 columns for: Parameter, Value
                        foreach (KeyValuePair<string, string> personBasicDetailValue in personBasicDetailValues)
                        {
                            tableBasicDetails.AddCell(new Cell(1, 1).Add(new Paragraph(personBasicDetailValue.Key)));
                            tableBasicDetails.AddCell(new Cell(1, 1).Add(new Paragraph(personBasicDetailValue.Value)));
                        }
                        document.Add(tableBasicDetails);

                        // ------------------------------------------------------------------------------------------------------------------

                        document.Add(new Paragraph(Properties.Resources.PrintFunctionsString).SetFontSize(20));
                        Table tableFunctions = new Table(3, false);      // 3 columns for: Description, Time Period, Years in function
                        tableFunctions.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintFunctionString)));
                        tableFunctions.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintTimePeriodString)));
                        tableFunctions.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintYearsString)));
                        foreach (Function function in person.Functions)
                        {
                            tableFunctions.AddCell(new Cell(1, 1).Add(new Paragraph(function.Description)));
                            tableFunctions.AddCell(new Cell(1, 1).Add(new Paragraph(function.TimePeriod.Start.ToShortDateString() + " - " + function.TimePeriod.End.ToShortDateString())));
                            tableFunctions.AddCell(new Cell(1, 1).Add(new Paragraph(function.FunctionYears.ToString())));
                        }
                        document.Add(tableFunctions);

                        // ------------------------------------------------------------------------------------------------------------------

                        document.Add(new Paragraph(Properties.Resources.PrintRewardsString).SetFontSize(20));
                        Table tableRewards = new Table(2, false);      // 2 columns for: Description, Status
                        tableRewards.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintDescriptionString)));
                        tableRewards.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintStatusString)));
                        foreach (Reward reward in person.Rewards.Rewards)
                        {
                            if (reward.Available || reward.Obtained)
                            {
                                tableRewards.AddCell(new Cell(1, 1).Add(new Paragraph(reward.Description ?? rewardTypeToString(reward.Type))));
                                tableRewards.AddCell(new Cell(1, 1).Add(new Paragraph(reward.Obtained ? (Properties.Resources.PrintObtainedDateString + ": " + reward.ObtainedDate.ToShortDateString()) : Properties.Resources.PrintAvailableString)));
                            }
                        }
                        document.Add(tableRewards);
                        document.Close();
                    }
                    printingResult = true;
                }
                catch (Exception)
                {
                    printingResult = false;
                    throw;
                }
            });
            return printingResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Print an overview list of all people.
        /// </summary>
        /// <param name="people">List with all available <see cref="Person"/> objects. This list may be filtered.</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        /// <param name="fullPeopleCount">Number of all (unfilteted) people.</param>
        /// <param name="filterText">Text used to filter the people list (empty string if not filtered)</param>
        /// <returns>true if printing succeeded; false if printing failed</returns>
        public async Task<bool> PrintPersonList(List<Person> people, string pdfFilePath, int fullPeopleCount, string filterText = "")
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
                        document.SetTopMargin(60);
                        document.SetBottomMargin(70);
                        // Add events to handle the generation of headers and footers
                        pdf.AddEventHandler(PdfDocumentEvent.START_PAGE, new PageHeaderEventHandler());
                        pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new PageFooterEventHandler(_personService));

                        Image imageWarning = new Image(ImageDataFactory.CreatePng(Properties.Resources.WarningIcon)).Scale(0.25f, 0.25f);

                        document.Add(new Paragraph(Properties.Resources.PrintPeopleOverviewString).SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));
                        document.Add(new Paragraph(Properties.Resources.PrintCountString + ": " + people.Count.ToString() + " " + Properties.Resources.PrintPeopleString));

                        if (people.Count != fullPeopleCount) { document.Add(new Paragraph(string.Format(Properties.Resources.PrintPersonListFilteredWarningString, filterText, fullPeopleCount)).SetFontColor(ColorConstants.RED)); }
                        
                        Table table = new Table(7, false);      // 7 columns for: ID, Name, First Name, Entry Date, BLSV Score, TSV Score, ParsingErrors
                        table.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintIDString)));
                        table.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintNameString)));
                        table.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintFirstNameString)));
                        table.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintEntrydateString)));
                        table.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintBLSVScoreString)));
                        table.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintTSVScoreString)));
                        table.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintParsingErrorString)));

                        foreach (Person person in people)
                        {
                            table.AddCell(new Cell(1, 1).Add(new Paragraph(person.Id.ToString())));
                            table.AddCell(new Cell(1, 1).Add(new Paragraph(person.Name)));
                            table.AddCell(new Cell(1, 1).Add(new Paragraph(person.FirstName)));
                            table.AddCell(new Cell(1, 1).Add(new Paragraph(person.EntryDate.ToShortDateString())));
                            table.AddCell(new Cell(1, 1).Add(new Paragraph(person.ScoreBLSV.ToString())));
                            table.AddCell(new Cell(1, 1).Add(new Paragraph(person.ScoreTSV.ToString())));
                            if (String.IsNullOrEmpty(person.ParsingFailureMessage))
                            {
                                table.AddCell(new Cell(1, 1).Add(new Paragraph("")));
                            }
                            else
                            {
                                table.AddCell(new Cell(1, 1).Add(imageWarning));
                            }
                        }

                        document.Add(table);
                        document.Close();
                    }
                    printingResult = true;
                }
                catch (Exception)
                {
                    printingResult = false;
                    throw;
                }
            });
            return printingResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Print an overview of all TSV rewards.
        /// </summary>
        /// <param name="peopleTsvRewardAvailable">List with all available <see cref="Person"/> objects with available TSV rewards used to generate the rewards overview</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        /// <param name="fullTsvRewardsCount">Number of all (unfiltered) TSV rewards.</param>
        /// <param name="filterTextTsv">String indicating, which filters were applied to the TSV reward list</param>
        /// <returns>true if printing succeeded; false if printing failed</returns>
        public async Task<bool> PrintTsvRewards(List<Person> peopleTsvRewardAvailable, string pdfFilePath, int fullTsvRewardsCount, string filterTextTsv)
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
                        document.SetBottomMargin(70);
                        // Add events to handle the generation of headers and footers
                        pdf.AddEventHandler(PdfDocumentEvent.START_PAGE, new PageHeaderEventHandler());
                        pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new PageFooterEventHandler(_personService));

                        Image imageWarning = new Image(ImageDataFactory.CreatePng(Properties.Resources.WarningIcon)).Scale(0.25f, 0.25f);

                        document.Add(new Paragraph(Properties.Resources.PrintTSVRewardOverviewString).SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));
                        document.Add(new Paragraph(Properties.Resources.PrintOnlyNewestRewardsAreShownString + Environment.NewLine));

                        List<IGrouping<RewardTypes, Person>> tsvRewardGroups = peopleTsvRewardAvailable.GroupBy(p => p.Rewards.HighestAvailableTSVReward.Type).ToList();

                        document.Add(new Paragraph(Properties.Resources.PrintCountTSVRewardsString + ": " + peopleTsvRewardAvailable.Count.ToString()));

                        if (peopleTsvRewardAvailable.Count != fullTsvRewardsCount) { document.Add(new Paragraph(string.Format(Properties.Resources.PrintRewardListFilteredWarningString, filterTextTsv, fullTsvRewardsCount)).SetFontColor(ColorConstants.RED)); }

                        Table tableTsv = new Table(7, false);      // 7 columns for: ID, Name, First Name, Person ID, Score, Departements, ParsingErrors
                        tableTsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintIDString)));
                        tableTsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintNameString)));
                        tableTsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintFirstNameString)));
                        tableTsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintPersonIdString)));
                        tableTsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintScoreString)));
                        tableTsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintDepartementsString)));
                        tableTsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintParsingErrorString)));

                        int counter = 0;
                        foreach (IGrouping<RewardTypes, Person> tsvRewardGroup in tsvRewardGroups)
                        {
                            string tsvRewardName = rewardTypeToString(tsvRewardGroup.Key);
                            tableTsv.AddCell(new Cell(1, 7).SetBackgroundColor(TSVLIGHTBLUE).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph(tsvRewardName)));
                            foreach (Person person in tsvRewardGroup)
                            {
                                tableTsv.AddCell(new Cell(1, 1).Add(new Paragraph((++counter).ToString())));
                                tableTsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.Name)));
                                tableTsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.FirstName)));
                                tableTsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.PersonID.ToString())));
                                tableTsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.ScoreTSV.ToString())));
                                tableTsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.Departements)));
                                if (String.IsNullOrEmpty(person.ParsingFailureMessage))
                                {
                                    tableTsv.AddCell(new Cell(1, 1).Add(new Paragraph("")));
                                }
                                else
                                {
                                    tableTsv.AddCell(new Cell(1, 1).Add(imageWarning));
                                }
                            }
                        }
                        document.Add(tableTsv);
                        document.Close();
                    }
                    printingResult = true;
                }
                catch (Exception)
                {
                    printingResult = false;
                    throw;
                }
            });
            return printingResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Print an overview of all BLSV rewards.
        /// </summary>
        /// <param name="peopleBlsvRewardAvailable">List with all available <see cref="Person"/> objects with available BLSV rewards used to generate the rewards overview</param>
        /// <param name="pdfFilePath">Filepath of the output PDF file</param>
        /// <param name="fullBlsvRewardsCount">Number of all (unfiltered) BLSV rewards.</param>
        /// <param name="filterTextBlsv">String indicating, which filters were applied to the BLSV reward list</param>
        /// <returns>true if printing succeeded; false if printing failed</returns>
        public async Task<bool> PrintBlsvRewards(List<Person> peopleBlsvRewardAvailable, string pdfFilePath, int fullBlsvRewardsCount, string filterTextBlsv)
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
                        document.SetBottomMargin(70);
                        // Add events to handle the generation of headers and footers
                        pdf.AddEventHandler(PdfDocumentEvent.START_PAGE, new PageHeaderEventHandler());
                        pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new PageFooterEventHandler(_personService));

                        Image imageWarning = new Image(ImageDataFactory.CreatePng(Properties.Resources.WarningIcon)).Scale(0.25f, 0.25f);

                        document.Add(new Paragraph(Properties.Resources.PrintBLSVRewardOverviewString).SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));
                        document.Add(new Paragraph(Properties.Resources.PrintOnlyNewestRewardsAreShownString + Environment.NewLine));

                        List<IGrouping<RewardTypes, Person>> blsvRewardGroups = peopleBlsvRewardAvailable.GroupBy(p => p.Rewards.HighestAvailableBLSVReward.Type).ToList();

                        document.Add(new Paragraph(Properties.Resources.PrintCountBLSVRewardsString + ": " + peopleBlsvRewardAvailable.Count.ToString()));

                        if (peopleBlsvRewardAvailable.Count != fullBlsvRewardsCount) { document.Add(new Paragraph(string.Format(Properties.Resources.PrintRewardListFilteredWarningString, filterTextBlsv, fullBlsvRewardsCount)).SetFontColor(ColorConstants.RED)); }

                        Table tableBlsv = new Table(7, false);      // 7 columns for: ID, Name, First Name, Person ID, Score, Departements, ParsingErrors
                        tableBlsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintIDString)));
                        tableBlsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintNameString)));
                        tableBlsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintFirstNameString)));
                        tableBlsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintPersonIdString)));
                        tableBlsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintScoreString)));
                        tableBlsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintDepartementsString)));
                        tableBlsv.AddHeaderCell(new Cell(1, 1).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(Properties.Resources.PrintParsingErrorString)));
                        
                        int counter = 0;
                        foreach (IGrouping<RewardTypes, Person> blsvRewardGroup in blsvRewardGroups)
                        {
                            tableBlsv.AddCell(new Cell(1, 7).SetBackgroundColor(TSVLIGHTBLUE).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph(blsvRewardGroup.Key.ToString())));
                            foreach (Person person in blsvRewardGroup)
                            {
                                tableBlsv.AddCell(new Cell(1, 1).Add(new Paragraph((++counter).ToString())));
                                tableBlsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.Name)));
                                tableBlsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.FirstName)));
                                tableBlsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.PersonID.ToString())));
                                tableBlsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.ScoreBLSV.ToString())));
                                tableBlsv.AddCell(new Cell(1, 1).Add(new Paragraph(person.Departements)));
                                if (String.IsNullOrEmpty(person.ParsingFailureMessage))
                                {
                                    tableBlsv.AddCell(new Cell(1, 1).Add(new Paragraph("")));
                                }
                                else
                                {
                                    tableBlsv.AddCell(new Cell(1, 1).Add(imageWarning));
                                }
                            }
                        }
                        document.Add(tableBlsv);
                        document.Close();
                    }
                    printingResult = true;
                }
                catch (Exception)
                {
                    printingResult = false;
                    throw;
                }
            });
            return printingResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Convert the given <see cref="RewardTypes"/> to a string
        /// </summary>
        /// <param name="rewardType">Reward type to convert to a string</param>
        private string rewardTypeToString(RewardTypes rewardType)
        {
            string rewardName = rewardType.ToString();
            switch (rewardType)
            {
                case RewardTypes.TSVSILVER: rewardName = Properties.Resources.PrintRewardTypeTSVSilver; break;
                case RewardTypes.TSVGOLD: rewardName = Properties.Resources.PrintRewardTypeTSVGold; break;
                case RewardTypes.TSVHONORARY: rewardName = Properties.Resources.PrintRewardTypeTSVHonorary; break;
                default: break;
            }
            return rewardName;
        }
    }

    // ##############################################################################################################################################################

    /// <summary>
    /// Class used to generate the page headers
    /// </summary>
    public class PageHeaderEventHandler : IEventHandler
    {
        public void HandleEvent(Event currentEvent)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;
            PdfDocument pdfDoc = docEvent.GetDocument();
            PdfPage page = docEvent.GetPage();
            PdfCanvas canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);

            // Create an image object and add it to the upper right corner of the page
            ImageData imageDataTsvLogo = ImageDataFactory.CreatePng(Properties.Resources.TSVLogo);
            Image imageTsvLogo = new Image(imageDataTsvLogo);
            float imageScalingFactor = 0.25f;
            // Page origin is in the lower left edge
            Rectangle logoRect = new Rectangle(page.GetPageSize().GetWidth() - imageScalingFactor * imageTsvLogo.GetImageWidth() - 20, page.GetPageSize().GetHeight() - imageScalingFactor * imageTsvLogo.GetImageHeight() - 20, imageScalingFactor * imageTsvLogo.GetImageWidth(), imageScalingFactor * imageTsvLogo.GetImageHeight());
            new Canvas(canvas, logoRect).Add(imageTsvLogo).Close();
        }
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    /// <summary>
    /// Class used to generate the page footers
    /// </summary>
    public class PageFooterEventHandler : IEventHandler
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor of the <see cref="PageFooterEventHandler"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object used to get some parameters used to print the document footer</param>
        public PageFooterEventHandler(IPersonService personService)
        {
            _personService = personService;
        }

        public void HandleEvent(Event currentEvent)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;
            PdfDocument pdfDoc = docEvent.GetDocument();
            PdfPage page = docEvent.GetPage();
            PdfCanvas canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);

            // Page origin is in the lower left edge

            // Export date            
            Paragraph textExportDate = new Paragraph(Properties.Resources.PrintExportDateString + ": " + DateTime.Now.ToString());
            new Canvas(canvas, new Rectangle(20, 45, 200, 30)).Add(textExportDate).Close();
            // Calculation Deadline
            Paragraph textCalculationDeadline = new Paragraph(Properties.Resources.PrintCalculationDeadlineString + ": " + _personService.CalculationDeadline.ToShortDateString());
            new Canvas(canvas, new Rectangle(20, 30, 200, 30)).Add(textCalculationDeadline).Close();
            // CSV file path
            Paragraph textCsvFilePath = new Paragraph(Properties.Resources.PrintCSVFileString + ": " + _personService.LastImportFilePath.EllipsisPath(60));
            new Canvas(canvas, new Rectangle(20, 15, 600, 30)).Add(textCsvFilePath).Close();

            // Software Version
            string version = Assembly.GetEntryAssembly()!.GetName().Version!.ToString();
            version = version.Substring(0, version.LastIndexOf("."));
            Paragraph textSoftwareVersion = new Paragraph(Properties.Resources.AppDisplayName + " " + Properties.Resources.PrintVersionString + ": v" + version);
            new Canvas(canvas, new Rectangle(page.GetPageSize().GetWidth() - 220, 30, 200, 30)).Add(textSoftwareVersion).Close();
            // Page numbers
            Paragraph textPageNumbers = new Paragraph(string.Format(Properties.Resources.PrintPageString, pdfDoc.GetPageNumber(page), pdfDoc.GetNumberOfPages()));
            new Canvas(canvas, new Rectangle(page.GetPageSize().GetWidth() - 110, 15, 100, 30)).Add(textPageNumbers).Close();
        }
    }
}
