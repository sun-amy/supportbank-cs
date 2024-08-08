using System.Formats.Asn1;
using System.Globalization;
using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SupportBank
{
    class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        static IList<Transactions> transactionsClassArray = new List<Transactions>{};

        static IList<Person> personClassArray = new List<Person>{};
        static void Main(string[] args)
        {
        
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Users\AmySun\OneDrive - Softwire Technology Limited\Documents\TechSwitch\SupportBank\supportbank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
            // Logger.Debug("This is an error message");
            
            string transactionsStr;
            string filePath = @"DodgyTransactions2015.csv";

            using (StreamReader reader = new StreamReader(filePath)) 
            {
                transactionsStr = reader.ReadToEnd();
            }


            string[] transactionsArray = transactionsStr.Split("\n");
            List<string> transactionsList = new List<string>(transactionsArray);
            transactionsList.RemoveAt(0);

            string[][] payments = [];

            foreach(string payment in transactionsList)
            {
                payments = [.. payments, payment.Split(",")];
            }

            for (var i = 0; i < payments.Length; i++)
            {
                var payment = new Transactions {Date = payments[i][0], From = payments[i][1], To = payments[i][2], Narrative = payments[i][3], Amount = payments[i][4]};
                transactionsClassArray = [.. transactionsClassArray, payment];
            }

            string[] peopleName = [];

            foreach(string[] payment in payments) {
                if (!peopleName.Contains(payment[1])) {
                    peopleName = [.. peopleName, payment[1]];
                }
                if (!peopleName.Contains(payment[2])) {
                    peopleName = [.. peopleName, payment[2]];
                }
            }
            
            for (var i = 0; i < peopleName.Length; i++)
            {
                var person = new Person {Name = peopleName[i]};
                personClassArray = [.. personClassArray, person];
            }

            static string ToTitleCase(string input)
            {
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                return textInfo.ToTitleCase(input);
            }

            Console.WriteLine("Welcome to the Support Bank program!");
            Console.Write("Please enter 'all' to see all accounts, or enter a name to see all relevent transactions: ");
            string? userInput = Console.ReadLine();

            if (userInput != null) {
                if (userInput.ToLower() == "all") {
                    foreach (Person person in personClassArray) {
                    Console.WriteLine(person.Name + "\nOwes: £" + person.Owes() + "\nIs owed: £" + person.Owed() + "\n");
                    }  

                } else if (peopleName.Contains(ToTitleCase(userInput.ToLower()))) {
                    Console.WriteLine("************************* Money lent *************************");
                    foreach (Transactions transaction in transactionsClassArray) {
                        if (transaction.From == ToTitleCase(userInput.ToLower())) {
                            if(DateTime.TryParse(transaction.Date, out DateTime resultDate) && float.TryParse(transaction.Amount, out float resultAmount))  {
                                Console.WriteLine(resultDate.ToShortDateString() + "\tfrom: " + transaction.From + "\tto: " + transaction.To + "\tnarrative: " + transaction.Narrative+ "\tamount: £" + resultAmount);
                            } else if (DateTime.TryParse(transaction.Date, out DateTime result)) {
                                Console.WriteLine(resultDate.ToShortDateString() + "\tfrom: " + transaction.From + "\tto: " + transaction.To + "\tnarrative: " + transaction.Narrative+ "\tamount is invalid");
                                Logger.Error("The amount is not a number.");
                            } else 
                            {
                                Console.WriteLine("Date is invalid" + "\tfrom: " + transaction.From + "\tto: " + transaction.To + "\tnarrative: " + transaction.Narrative+ "\tamount: £" + transaction.Amount);
                                Logger.Error("Date is not in a valid format.");
                            }
                        }
                    }
                    Console.WriteLine("\n************************* Money borrowed *************************");
                    foreach (Transactions transaction in transactionsClassArray) {
                        if (transaction.To == ToTitleCase(userInput.ToLower())) {
                            if(DateTime.TryParse(transaction.Date, out DateTime resultDate) && float.TryParse(transaction.Amount, out float resultAmount))  {
                                Console.WriteLine(resultDate.ToShortDateString() + "\tfrom: " + transaction.From + "\tto: " + transaction.To + "\tnarrative: " + transaction.Narrative+ "\tamount: £" + resultAmount);
                            } else if (DateTime.TryParse(transaction.Date, out DateTime result)) {
                                Console.WriteLine(resultDate.ToShortDateString() + "\tfrom: " + transaction.From + "\tto: " + transaction.To + "\tnarrative: " + transaction.Narrative+ "\tamount is invalid");
                                Logger.Error("The amount is not a number.");
                            } else 
                            {
                                Console.WriteLine("Date is invalid" + "\tfrom: " + transaction.From + "\tto: " + transaction.To + "\tnarrative: " + transaction.Narrative+ "\tamount: £" + transaction.Amount);
                                Logger.Error("Date is not in a valid format.");
                            }
                        }
                    }
                } else {
                    Console.Write("Invalid input.");
                    Logger.Info("Invalid input from the user.");
                }
            } 
        }                 
        public class Transactions {
            private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
            public string? Date { get; set; }
            public string? From { get; set; }
            public string? To { get; set; }
            public string? Narrative { get; set; }
            public string? Amount { get; set; }
            }

        public class Person {
            private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
            public string? Name { get; set; }

            public string Owes() {
                float owesTotal = 0;
                foreach (Transactions transaction in transactionsClassArray)
                {
                    if ( transaction.Amount == null ) {
                        continue;
                    }
                    else if (transaction.To == Name) {
                        if(float.TryParse(transaction.Amount, out float result)) 
                        {
                            owesTotal += result;
                        } 
                        else {
                            Console.WriteLine("Data Error");
                            Logger.Error("Transaction amount in data is not a float");
                        }
                    }
                }
                return Math.Round(owesTotal,2).ToString();
            }

            public string Owed() {
                float owedTotal = 0;
                foreach (Transactions transaction in transactionsClassArray)
                {
                    if (transaction.Amount == null) {
                        continue;
                    }
                    else if (transaction.From == Name) {
                        if(float.TryParse(transaction.Amount, out float result)) 
                        {
                            owedTotal += result;
                        } 
                        else {
                            Console.WriteLine("Data Error");
                            Logger.Error("Transaction amount in data is not a float");
                            break;
                        }
                    }
                }
                return Math.Round(owedTotal, 2).ToString();
            }
        }   
        
    }
}

// ($"{transaction.Date} from: {transaction.From} \t to: {transaction.To} \t amount: £{transaction.Amount} \t narrative: {transaction.Narrative}")