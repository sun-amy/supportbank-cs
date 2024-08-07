namespace SupportBank
{
    class Program
    {
        static IList<Transactions> transactionsClassArray = new List<Transactions>{};

        static IList<Person> personClassArray = new List<Person>{};
        static void Main(string[] args)
        {
            string transactionsStr;
            string filePath = @"Transactions2014.csv";

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

            foreach (Person person in personClassArray) {
                Console.WriteLine(person.Name + " owes £" + person.Owes() + " and is owed £" + person.Owed() + ".");
            }
        
        }                 
        public class Transactions {
                public string? Date { get; set; }
                public string? From { get; set; }
                public string? To { get; set; }
                public string? Narrative { get; set; }
                public string? Amount { get; set; }
            }

        public class Person {
            public string? Name { get; set; }

            public string Owes() {
                float owesTotal = 0;
                foreach (Transactions transaction in transactionsClassArray)
                {
                    if (transaction.To == Name) {
                        owesTotal += float.Parse(transaction.Amount);
                    }
                }
                return Math.Round(owesTotal,2).ToString();
            }

            public string Owed() {
                float owedTotal = 0;
                foreach (Transactions transaction in transactionsClassArray)
                {
                    if (transaction.From == Name) {
                        owedTotal += float.Parse(transaction.Amount);
                    }
                }
                return Math.Round(owedTotal, 2).ToString();
            }
        }   
        
    }
}