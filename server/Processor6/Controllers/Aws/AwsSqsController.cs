
using System;
using System.Collections.Generic;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Amazon;
using Contensive.Processor.Models.Domain;
using Amazon.SQS;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
//
namespace Contensive.Processor.Controllers {
    public class AwsSqsController {
        //
        //====================================================================================================
        /// <summary>
        /// Create an Sqs Client to be used as a parameter in methods
        /// </summary>
        /// <param name="core"></param>
        /// <returns></returns>
        public static AmazonSQSClient getSqsClient(CoreController core) {
            BasicAWSCredentials cred = new BasicAWSCredentials(core.awsCredentials.awsAccessKeyId, core.awsCredentials.awsSecretAccessKey);
            return new AmazonSQSClient(cred, core.awsCredentials.awsRegion);
        }
        //
        //====================================================================================================
        //
        public static string createQueue(CoreController core, AmazonSQSClient sqsClient, string queueName) {
            try {
                var queueRequest = new Amazon.SQS.Model.CreateQueueRequest(core.appConfig.name.ToLowerInvariant() + "_" + queueName);
                queueRequest.Attributes.Add("VisibilityTimeout", "600");
                var queueResponse =  sqsClient.CreateQueue(queueRequest);
                return queueResponse.QueueUrl;
            } catch (Exception ex) {
                LogController.logError(core, ex);
                return "";
            }
        }
        //
        //====================================================================================================
        //
        public static List<string> getQueueList(CoreController core, AmazonSQSClient sqsClient) {
            var result = new List<string>();
            var listQueuesResponse = sqsClient.ListQueues(core.appConfig.name.ToLowerInvariant() + "_");
            int nameStartPos = core.appConfig.name.Length;
            foreach (var queueUrl in listQueuesResponse.QueueUrls) {
                result.Add(queueUrl.Substring(nameStartPos));
            }
            return result;
        }
    }
}
