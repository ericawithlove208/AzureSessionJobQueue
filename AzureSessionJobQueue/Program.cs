using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration; // Namespace for ConfigurationManager
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage
using System.IO;

namespace AzureSessionJobQueue
{
    class Program
    {
        static QueueClient CreateQueueClient()
        {
            // Get the connection string from app settings
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(connectionString, "sessionjob");

            // Create the queue
            queueClient.CreateIfNotExists();

            return queueClient;
        }

        static String GetNextMessage(QueueClient queueClient)
        {
            QueueMessage[] retrievedMessages = queueClient.ReceiveMessages();
            QueueMessage retrievedMessage = retrievedMessages[0];

            String message = retrievedMessage.MessageText;

            queueClient.DeleteMessage(retrievedMessage.MessageId, retrievedMessage.PopReceipt);

            return message;
        }

        static String GetMessageString()
        {
            String filename = Environment.CurrentDirectory + "\\SampleMessage.json";
            return File.ReadAllText(filename);
        }

        static void Main(string[] args)
        {
            System.Console.Out.WriteLine("Creating queue client...");
            QueueClient queueClient = CreateQueueClient();

            System.Console.Out.WriteLine("Getting message string...");

            String message = GetMessageString();

            System.Console.Out.WriteLine("Sending message string: " + message);

            queueClient.SendMessage(message);

            System.Console.Out.WriteLine("Getting the next message...");

            message = GetNextMessage(queueClient);

            System.Console.Out.WriteLine("Got message: " + message);

            System.Console.ReadLine();
        }
    }
}
