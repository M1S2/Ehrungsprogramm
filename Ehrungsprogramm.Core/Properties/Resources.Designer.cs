﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ehrungsprogramm.Core.Properties {
    using System;
    
    
    /// <summary>
    ///   Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    /// </summary>
    // Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    // -Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    // Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    // mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ehrungsprogramm.Core.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        ///   Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Reward Program ähnelt.
        /// </summary>
        public static string AppDisplayName {
            get {
                return ResourceManager.GetString("AppDisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Birth date couldn&apos;t be parsed correctly ({0})! ähnelt.
        /// </summary>
        public static string ErrorCsvFileParserBirthDate {
            get {
                return ResourceManager.GetString("ErrorCsvFileParserBirthDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die The following column errors occured in the CSV file:
        ///{0}Parsing canceled. ähnelt.
        /// </summary>
        public static string ErrorCsvFileParserColumnErrors {
            get {
                return ResourceManager.GetString("ErrorCsvFileParserColumnErrors", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die - Mandatory &quot;{0}&quot; column is missing ähnelt.
        /// </summary>
        public static string ErrorCsvFileParserColumnMissing {
            get {
                return ResourceManager.GetString("ErrorCsvFileParserColumnMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die - Mandatory function columns (&quot;{0}&quot;, &quot;{1}&quot;, &quot;{2}&quot;) are missing. At least one group is necessary. ähnelt.
        /// </summary>
        public static string ErrorCsvFileParserColumnMissingFunctions {
            get {
                return ResourceManager.GetString("ErrorCsvFileParserColumnMissingFunctions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die - Mandatory reward columns (&quot;{0}&quot;, &quot;{1}&quot;, &quot;{2}&quot;) are missing. At least one group is necessary. ähnelt.
        /// </summary>
        public static string ErrorCsvFileParserColumnMissingRewards {
            get {
                return ResourceManager.GetString("ErrorCsvFileParserColumnMissingRewards", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Entry date couldn&apos;t be parsed correctly ({0})! ähnelt.
        /// </summary>
        public static string ErrorCsvFileParserEntryDate {
            get {
                return ResourceManager.GetString("ErrorCsvFileParserEntryDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die File doesn&apos;t exist (&quot;{0}&quot;). ähnelt.
        /// </summary>
        public static string ErrorCsvFileParserFileDoesntExist {
            get {
                return ResourceManager.GetString("ErrorCsvFileParserFileDoesntExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Opening CSV file failed (&quot;{0}&quot;).
        ///This could be caused by having the file opened in another program (e.g. Excel). ähnelt.
        /// </summary>
        public static string ErrorCsvFileParserFileOpenFailed {
            get {
                return ResourceManager.GetString("ErrorCsvFileParserFileOpenFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Function End Date couldn&apos;t be parsed correctly ({0}, Function: {1})! ähnelt.
        /// </summary>
        public static string ErrorCsvFileParserFunctionEndDate {
            get {
                return ResourceManager.GetString("ErrorCsvFileParserFunctionEndDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Function Start Date couldn&apos;t be parsed correctly ({0}, Function: {1})! ähnelt.
        /// </summary>
        public static string ErrorCsvFileParserFunctionStartDate {
            get {
                return ResourceManager.GetString("ErrorCsvFileParserFunctionStartDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Function End Date is earlier than Start Date (Start: {0}, End: {1}, Function: {2})! ähnelt.
        /// </summary>
        public static string ErrorCsvFileParserFunctionStartEndDateSwapped {
            get {
                return ResourceManager.GetString("ErrorCsvFileParserFunctionStartEndDateSwapped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Reward Number couldn&apos;t be parsed correctly ({0}, Reward: {1})! ähnelt.
        /// </summary>
        public static string ErrorCsvFileParserRewardNumber {
            get {
                return ResourceManager.GetString("ErrorCsvFileParserRewardNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Reward Obtained Date couldn&apos;t be parsed correctly ({0}, Reward: {1})! ähnelt.
        /// </summary>
        public static string ErrorCsvFileParserRewardObtainedDate {
            get {
                return ResourceManager.GetString("ErrorCsvFileParserRewardObtainedDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Available ähnelt.
        /// </summary>
        public static string PrintAvailableString {
            get {
                return ResourceManager.GetString("PrintAvailableString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Birthdate ähnelt.
        /// </summary>
        public static string PrintBirthdateString {
            get {
                return ResourceManager.GetString("PrintBirthdateString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die BLSV Rewards Overview ähnelt.
        /// </summary>
        public static string PrintBLSVRewardOverviewString {
            get {
                return ResourceManager.GetString("PrintBLSVRewardOverviewString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die BLSV Score ähnelt.
        /// </summary>
        public static string PrintBLSVScoreString {
            get {
                return ResourceManager.GetString("PrintBLSVScoreString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Calculation Deadline ähnelt.
        /// </summary>
        public static string PrintCalculationDeadlineString {
            get {
                return ResourceManager.GetString("PrintCalculationDeadlineString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Count BLSV Rewards ähnelt.
        /// </summary>
        public static string PrintCountBLSVRewardsString {
            get {
                return ResourceManager.GetString("PrintCountBLSVRewardsString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Count ähnelt.
        /// </summary>
        public static string PrintCountString {
            get {
                return ResourceManager.GetString("PrintCountString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Count TSV Rewards ähnelt.
        /// </summary>
        public static string PrintCountTSVRewardsString {
            get {
                return ResourceManager.GetString("PrintCountTSVRewardsString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die CSV File ähnelt.
        /// </summary>
        public static string PrintCSVFileString {
            get {
                return ResourceManager.GetString("PrintCSVFileString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Departements ähnelt.
        /// </summary>
        public static string PrintDepartementsString {
            get {
                return ResourceManager.GetString("PrintDepartementsString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Description ähnelt.
        /// </summary>
        public static string PrintDescriptionString {
            get {
                return ResourceManager.GetString("PrintDescriptionString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Effective Years Board Member ähnelt.
        /// </summary>
        public static string PrintEffectiveYearsBoardMemberString {
            get {
                return ResourceManager.GetString("PrintEffectiveYearsBoardMemberString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Effective Years Head Of Departement ähnelt.
        /// </summary>
        public static string PrintEffectiveYearsHeadOfDepartementString {
            get {
                return ResourceManager.GetString("PrintEffectiveYearsHeadOfDepartementString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Effective Years Other Function ähnelt.
        /// </summary>
        public static string PrintEffectiveYearsOtherFunctionString {
            get {
                return ResourceManager.GetString("PrintEffectiveYearsOtherFunctionString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Entrydate ähnelt.
        /// </summary>
        public static string PrintEntrydateString {
            get {
                return ResourceManager.GetString("PrintEntrydateString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Export Date ähnelt.
        /// </summary>
        public static string PrintExportDateString {
            get {
                return ResourceManager.GetString("PrintExportDateString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die First Name ähnelt.
        /// </summary>
        public static string PrintFirstNameString {
            get {
                return ResourceManager.GetString("PrintFirstNameString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Functions ähnelt.
        /// </summary>
        public static string PrintFunctionsString {
            get {
                return ResourceManager.GetString("PrintFunctionsString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Function ähnelt.
        /// </summary>
        public static string PrintFunctionString {
            get {
                return ResourceManager.GetString("PrintFunctionString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die # ähnelt.
        /// </summary>
        public static string PrintIDString {
            get {
                return ResourceManager.GetString("PrintIDString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Membership Years ähnelt.
        /// </summary>
        public static string PrintMembershipYearsString {
            get {
                return ResourceManager.GetString("PrintMembershipYearsString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Name ähnelt.
        /// </summary>
        public static string PrintNameString {
            get {
                return ResourceManager.GetString("PrintNameString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Obtained Date ähnelt.
        /// </summary>
        public static string PrintObtainedDateString {
            get {
                return ResourceManager.GetString("PrintObtainedDateString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Only the most recent rewards not yet received are listed. ähnelt.
        /// </summary>
        public static string PrintOnlyNewestRewardsAreShownString {
            get {
                return ResourceManager.GetString("PrintOnlyNewestRewardsAreShownString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Page {0} of {1} ähnelt.
        /// </summary>
        public static string PrintPageString {
            get {
                return ResourceManager.GetString("PrintPageString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Parsing Error ähnelt.
        /// </summary>
        public static string PrintParsingErrorString {
            get {
                return ResourceManager.GetString("PrintParsingErrorString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die People Overview ähnelt.
        /// </summary>
        public static string PrintPeopleOverviewString {
            get {
                return ResourceManager.GetString("PrintPeopleOverviewString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die People ähnelt.
        /// </summary>
        public static string PrintPeopleString {
            get {
                return ResourceManager.GetString("PrintPeopleString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Person Details ähnelt.
        /// </summary>
        public static string PrintPersonDetailsString {
            get {
                return ResourceManager.GetString("PrintPersonDetailsString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Person ID ähnelt.
        /// </summary>
        public static string PrintPersonIdString {
            get {
                return ResourceManager.GetString("PrintPersonIdString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Caution: The list is filtered (filter: &quot;{0}&quot;)! In total {1} people are available! ähnelt.
        /// </summary>
        public static string PrintPersonListFilteredWarningString {
            get {
                return ResourceManager.GetString("PrintPersonListFilteredWarningString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Caution: The list is filtered (hidden are: {0})! In total {1} rewards are available! ähnelt.
        /// </summary>
        public static string PrintRewardListFilteredWarningString {
            get {
                return ResourceManager.GetString("PrintRewardListFilteredWarningString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Rewards ähnelt.
        /// </summary>
        public static string PrintRewardsString {
            get {
                return ResourceManager.GetString("PrintRewardsString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die TSV Gold ähnelt.
        /// </summary>
        public static string PrintRewardTypeTSVGold {
            get {
                return ResourceManager.GetString("PrintRewardTypeTSVGold", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die TSV Honorary ähnelt.
        /// </summary>
        public static string PrintRewardTypeTSVHonorary {
            get {
                return ResourceManager.GetString("PrintRewardTypeTSVHonorary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die TSV Silver ähnelt.
        /// </summary>
        public static string PrintRewardTypeTSVSilver {
            get {
                return ResourceManager.GetString("PrintRewardTypeTSVSilver", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Score ähnelt.
        /// </summary>
        public static string PrintScoreString {
            get {
                return ResourceManager.GetString("PrintScoreString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Status ähnelt.
        /// </summary>
        public static string PrintStatusString {
            get {
                return ResourceManager.GetString("PrintStatusString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Time Period ähnelt.
        /// </summary>
        public static string PrintTimePeriodString {
            get {
                return ResourceManager.GetString("PrintTimePeriodString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die TSV Rewards Overview ähnelt.
        /// </summary>
        public static string PrintTSVRewardOverviewString {
            get {
                return ResourceManager.GetString("PrintTSVRewardOverviewString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die TSV Score ähnelt.
        /// </summary>
        public static string PrintTSVScoreString {
            get {
                return ResourceManager.GetString("PrintTSVScoreString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Version ähnelt.
        /// </summary>
        public static string PrintVersionString {
            get {
                return ResourceManager.GetString("PrintVersionString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Years ähnelt.
        /// </summary>
        public static string PrintYearsString {
            get {
                return ResourceManager.GetString("PrintYearsString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Ressource vom Typ System.Byte[].
        /// </summary>
        public static byte[] TSVLogo {
            get {
                object obj = ResourceManager.GetObject("TSVLogo", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Ressource vom Typ System.Byte[].
        /// </summary>
        public static byte[] WarningIcon {
            get {
                object obj = ResourceManager.GetObject("WarningIcon", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}
