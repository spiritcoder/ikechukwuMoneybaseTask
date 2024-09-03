using System;
namespace Moneybase.core
{
	public class EnvironmentVariables
	{
		public static string KafkaURL { get; }  = Environment.GetEnvironmentVariable("KafkaURL");
		public static string KafkaTopic { get; }  = Environment.GetEnvironmentVariable("KafkaTopic");
		public static string KafkaGroupID { get; }  = Environment.GetEnvironmentVariable("KafkaGroupID");
    }
}

