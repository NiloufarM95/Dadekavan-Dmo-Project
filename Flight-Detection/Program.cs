using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using CsvHelper;
using Flight_Detection.DataAccess.Models;
using Flight_Detection.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flight_Detection
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                var inputParameters = GetDetectionParams();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                Console.WriteLine("Please wait...");

                var lstFlightDetectionResults = GetData(inputParameters);

                if (lstFlightDetectionResults.Count == 0)
                {
                    Console.WriteLine("No flight is detected!");
                    return;
                }

                stopwatch.Stop();

                CreateResultFile(lstFlightDetectionResults, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static List<FlightDetectionResult> GetData(InputParameters inputParameters)
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<IFlightDetectionService, FlightDetectionService>()
                .BuildServiceProvider();

            var flightDetectionService = serviceProvider.GetRequiredService<IFlightDetectionService>();

            var lstFlightDetectionResults = flightDetectionService.GetRoutesByAgencyIdAndDuration(inputParameters);

            return lstFlightDetectionResults;
        }

        private static void CreateResultFile(List<FlightDetectionResult> lstFlightDetectionResults, long executionMetric)
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string resultFile = Path.Combine(directory, "results.csv");

            using (var writer = new StreamWriter(resultFile))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(lstFlightDetectionResults);
            }

            Console.WriteLine("The file results.csv with {0} records has been created on your desktop!",
                lstFlightDetectionResults.Count);
            Console.WriteLine("The execution metrics is {0} ms!", executionMetric);

            Console.WriteLine("Please press a key to continue ... !");
            Console.ReadKey();
        }

        private static InputParameters GetDetectionParams()
        {
            InputParameters input = new InputParameters();

            input.StartDate = GetStartDate();

            input.EndDate = GetEndDate();

            input.AgencyId = GetAgencyId();

            return input;

            #region Local Functions

            DateTime GetStartDate()
            {
                string startDateInput = "";
                while (string.IsNullOrEmpty(startDateInput))
                {
                    Console.WriteLine("Please Enter start date (in yyyy-mm-dd format):");
                    startDateInput = Console.ReadLine();
                }

                return Convert.ToDateTime(startDateInput);
            }

            DateTime GetEndDate()
            {
                string endDateInput = "";
                DateTime? endDate = null;
                while (string.IsNullOrEmpty(endDateInput))
                {
                    Console.WriteLine("Please Enter end date (in yyyy-mm-dd format):");
                    endDateInput = Console.ReadLine();

                    if (!string.IsNullOrEmpty(endDateInput))
                    {
                        endDate = Convert.ToDateTime(endDateInput);

                        if (endDate <= input.StartDate)
                        {
                            Console.WriteLine("End date should not be less than or equal to start date!");
                            endDateInput = "";
                        }
                    }
                }

                return Convert.ToDateTime(endDate);
            }

            int GetAgencyId()
            {
                string agencyIdInput = "";
                while (string.IsNullOrEmpty(agencyIdInput))
                {
                    Console.WriteLine("Please Enter agency id:");
                    agencyIdInput = Console.ReadLine();
                }

                return Convert.ToInt32(agencyIdInput);
            }

            #endregion
        }
    }
}

