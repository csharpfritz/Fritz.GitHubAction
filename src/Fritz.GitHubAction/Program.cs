using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Settings.Configuration;
using StreamDeckLib;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Fritz.GitHubAction
{
	class Program
	{
		static ILoggerFactory GetLoggerFactory()
		{
			var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Directory.SetCurrentDirectory(dir);

			var configuration = new ConfigurationBuilder()
							.AddJsonFile("appsettings.json")
							.Build();

			Log.Logger = new LoggerConfiguration()
							.ReadFrom.Configuration(configuration)
							.CreateLogger();

			var loggerFactory = new LoggerFactory()
					.AddSerilog(Log.Logger);

			TopLogger = loggerFactory.CreateLogger("top");

			TopLogger.LogInformation("Plugin started");

			return loggerFactory;
		}

		private static Microsoft.Extensions.Logging.ILogger TopLogger;

		static async Task Main(string[] args)
		{

			Debugger.Launch();

			while (!Debugger.IsAttached)
			{
				await Task.Delay(100);
			}


			using (var source = new CancellationTokenSource())
			{
				using (var loggerFactory = GetLoggerFactory())
				{
					try
					{

						TopLogger.LogDebug($"Arguments: {string.Join(',', args)}");

						await ConnectionManager.Initialize(args, loggerFactory)
								.SetPlugin(new GitHubProject())
								.StartAsync(source.Token);

						Console.ReadLine();

					}
					catch (Exception ex)
					{
						TopLogger.LogError(ex, "Error while running the plugin");
					}
					source.Cancel();
				}

			}
		}
	}
}
