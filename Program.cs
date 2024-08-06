// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

string transactionsStr;
string filePath = @"Transactions2014.csv";

using (StreamReader reader = new StreamReader(filePath)) 
{
    transactionsStr = reader.ReadToEnd();
}

string[] transactionsArray = transactionsStr.Split("\n");
//Console.WriteLine(transactionsArray);

string[] headers = transactionsArray[0].Split(",");
Console.WriteLine(headers[0]);

class Transactions {
    public string date {get;}
    public string from {get;}
    public string to {get;}
    public string narative {get;}
    public string amount {get;}
} 

string[] transactionsClass =[];

for (int i = 1; i < transactionsArray.Length; i++)
{
    var transaction = new Transactions() {transactionsArray[i][0], transactionsArray[i][1], transactionsArray[i][2], transactionsArray[i][3],transactionsArray[i][4]}
    transactionsClass = [.. transactionsClass, transaction];
}

Console.WriteLine(transactionsClass[0].date);
// foreach (string element in transactionsArray){
//     Console.Write(element);
// }
