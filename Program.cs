using System.Globalization;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Text.Json;
using System.Transactions;

namespace SupportBank
{
    class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        static IList<Transactions> transactionsClassArray = new List<Transactions>{};

        static IList<TransactionsJson> transactionsClassArrayJson = new List<TransactionsJson>{};
        static IList<Person> personClassArray = new List<Person>{};
        static void Main(string[] args)
        {
        
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Users\AmySun\OneDrive - Softwire Technology Limited\Documents\TechSwitch\SupportBank\supportbank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
            
            string transactionsStr;
            string filePath = @"Transactions2013.json";

            using (StreamReader reader = new StreamReader(filePath)) 
            {
                transactionsStr = reader.ReadToEnd();
            }

            string[] peopleName = [];

            if (filePath.Substring(filePath.Length - 4) == ".csv")
            {
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
                    var payment = new Transactions {Date = payments[i][0], FromAccount = payments[i][1], ToAccount = payments[i][2], Narrative = payments[i][3], Amount = payments[i][4]};
                    transactionsClassArray = [.. transactionsClassArray, payment];
                }

                foreach(string[] payment in payments) {
                    if (!peopleName.Contains(payment[1])) {
                        peopleName = [.. peopleName, payment[1]];
                    }
                    if (!peopleName.Contains(payment[2])) {
                        peopleName = [.. peopleName, payment[2]];
                    }
                }
            }
            else if (filePath.Substring(filePath.Length - 5) == ".json")
            {
                List<TransactionsJson>? transactionsClassArrayJson = JsonSerializer.Deserialize<List<TransactionsJson>>(transactionsStr);
                if (transactionsClassArrayJson != null) {
                    foreach (TransactionsJson transaction in transactionsClassArrayJson) {
                        if (transaction.FromAccount != null && !peopleName.Contains(transaction.FromAccount)) {
                            peopleName = [.. peopleName, transaction.FromAccount];
                        }
                        if (transaction.ToAccount != null &&!peopleName.Contains(transaction.ToAccount)) {
                            peopleName = [.. peopleName, transaction.ToAccount];
                        }
                    }

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
                    if (filePath.Substring(filePath.Length - 4) == ".csv") {
                        foreach (Transactions transaction in transactionsClassArray) {
                            if (transaction.FromAccount == ToTitleCase(userInput.ToLower())) {
                                if(DateTime.TryParse(transaction.Date, out DateTime resultDate) && float.TryParse(transaction.Amount, out float resultAmount))  {
                                    Console.WriteLine(resultDate.ToShortDateString() + "\tfrom: " + transaction.FromAccount + "\tto: " + transaction.ToAccount + "\tnarrative: " + transaction.Narrative+ "\tamount: £" + resultAmount);
                                } else if (DateTime.TryParse(transaction.Date, out DateTime result)) {
                                    Console.WriteLine(resultDate.ToShortDateString() + "\tfrom: " + transaction.FromAccount + "\tto: " + transaction.ToAccount + "\tnarrative: " + transaction.Narrative+ "\tamount is invalid");
                                    Logger.Error("The amount is not a number.");
                                } else 
                                {
                                    Console.WriteLine("Date is invalid" + "\tfrom: " + transaction.FromAccount + "\tto: " + transaction.ToAccount + "\tnarrative: " + transaction.Narrative+ "\tamount: £" + transaction.Amount);
                                    Logger.Error("Date is not in a valid format.");
                                }
                            }
                        }
                        Console.WriteLine("\n************************* Money borrowed *************************");
                        foreach (Transactions transaction in transactionsClassArray) {
                            if (transaction.ToAccount == ToTitleCase(userInput.ToLower())) {
                                if(DateTime.TryParse(transaction.Date, out DateTime resultDate) && float.TryParse(transaction.Amount, out float resultAmount))  {
                                    Console.WriteLine(resultDate.ToShortDateString() + "\tfrom: " + transaction.FromAccount + "\tto: " + transaction.ToAccount + "\tnarrative: " + transaction.Narrative+ "\tamount: £" + resultAmount);
                                } else if (DateTime.TryParse(transaction.Date, out DateTime result)) {
                                    Console.WriteLine(resultDate.ToShortDateString() + "\tfrom: " + transaction.FromAccount + "\tto: " + transaction.ToAccount + "\tnarrative: " + transaction.Narrative+ "\tamount is invalid");
                                    Logger.Error("The amount is not a number.");
                                } else 
                                {
                                    Console.WriteLine("Date is invalid" + "\tfrom: " + transaction.FromAccount + "\tto: " + transaction.ToAccount + "\tnarrative: " + transaction.Narrative+ "\tamount: £" + transaction.Amount);
                                    Logger.Error("Date is not in a valid format.");
                                }
                            }
                        }
                    }
                    else if(filePath.Substring(filePath.Length - 5) == ".json") {
                        foreach (TransactionsJson transaction in transactionsClassArrayJson) {
                            if (transaction.FromAccount == ToTitleCase(userInput.ToLower())) {
                                // if(DateTime.TryParse(transaction.Date, out DateTime resultDate) && float.TryParse(transaction.Amount, out float resultAmount))  {
                                //     Console.WriteLine(resultDate.ToShortDateString() + "\tfrom: " + transaction.FromAccount + "\tto: " + transaction.ToAccount + "\tnarrative: " + transaction.Narrative+ "\tamount: £" + resultAmount);
                                // } else if (DateTime.TryParse(transaction.Date, out DateTime result)) {
                                //     Console.WriteLine(resultDate.ToShortDateString() + "\tfrom: " + transaction.FromAccount + "\tto: " + transaction.ToAccount + "\tnarrative: " + transaction.Narrative+ "\tamount is invalid");
                                //     Logger.Error("The amount is not a number.");
                                // } else 
                                // {
                                    Console.WriteLine("Date is invalid" + "\tfrom: " + transaction.FromAccount + "\tto: " + transaction.ToAccount + "\tnarrative: " + transaction.Narrative+ "\tamount: £" + transaction.Amount);
                                    Logger.Error("Date is not in a valid format.");
                                // }
                            }
                        }
                        Console.WriteLine("\n************************* Money borrowed *************************");
                        foreach (TransactionsJson transaction in transactionsClassArrayJson) {
                            if (transaction.ToAccount == ToTitleCase(userInput.ToLower())) {
                                // if(DateTime.TryParse(transaction.Date, out DateTime resultDate) && float.TryParse(transaction.Amount, out float resultAmount))  {
                                //     Console.WriteLine(resultDate.ToShortDateString() + "\tfrom: " + transaction.FromAccount + "\tto: " + transaction.ToAccount + "\tnarrative: " + transaction.Narrative+ "\tamount: £" + resultAmount);
                                // } else if (DateTime.TryParse(transaction.Date, out DateTime result)) {
                                //     Console.WriteLine(resultDate.ToShortDateString() + "\tfrom: " + transaction.FromAccount + "\tto: " + transaction.ToAccount + "\tnarrative: " + transaction.Narrative+ "\tamount is invalid");
                                //     Logger.Error("The amount is not a number.");
                                // } else 
                                // {
                                    Console.WriteLine("Date is invalid" + "\tfrom: " + transaction.FromAccount + "\tto: " + transaction.ToAccount + "\tnarrative: " + transaction.Narrative+ "\tamount: £" + transaction.Amount);
                                    Logger.Error("Date is not in a valid format.");
                                // }
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
            public string? FromAccount { get; set; }
            public string? ToAccount { get; set; }
            public string? Narrative { get; set; }
            public string? Amount { get; set; }
            }
            
            public class TransactionsJson {
            private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
            public string? Date { get; set; }
            public string? FromAccount { get; set; }
            public string? ToAccount { get; set; }
            public string? Narrative { get; set; }
            public float? Amount { get; set; }
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
                    else if (transaction.ToAccount == Name) {
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
                    else if (transaction.FromAccount == Name) {
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