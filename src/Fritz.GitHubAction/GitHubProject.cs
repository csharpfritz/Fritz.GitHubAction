using Octokit;
using StreamDeckLib;
using StreamDeckLib.Messages;
using System.Dynamic;
using System.Threading.Tasks;

namespace Fritz.GitHubAction
{
	internal class GitHubProject : BaseStreamDeckPlugin
	{
		private static string _Project = "csharpfritz/Fritz.StreamTools";
		private static bool _IsPropertyInspectorConnected = false;
		private GitHubClient _Client;

		/*
		Cheer 300 cpayette February 10, 2019
		Cheer 500 animatedslinky February 10, 2019
		Cheer 200 auth0bobby February 10, 2019
		Cheer 100 roberttables February 10, 2019
		Cheer 500 Lannonbr February 10, 2019
		Cheer 500 theMichaelJolley February 10, 2019
		Cheer 200 JamesMontemagno February 10, 2019
		*/



		public override async Task OnKeyUp(StreamDeckEventPayload args)
		{

			// TODO: Add the project to this URL
			await Manager.OpenUrlAsync(args.context, $"https://www.github.com/{_Project}");

		}

		public override async Task OnWillAppear(StreamDeckEventPayload args)
		{
			await SetGitHubTitles(args.context);
		}

		private async Task SetGitHubTitles(string streamDeckContext)
		{
			_Client = new GitHubClient(new ProductHeaderValue("StreamDeck-GitHub-Action"));
			var issueTask = _Client.Issue.GetAllForRepository(_Project.Split('/')[0], _Project.Split('/')[1], new RepositoryIssueRequest
			{
				State = ItemStateFilter.Open
			});
			var prTask = _Client.PullRequest.GetAllForRepository(_Project.Split('/')[0], _Project.Split('/')[1], new PullRequestRequest
			{
				State = ItemStateFilter.Open
			});

			await Task.WhenAll(issueTask, prTask);

			var outTitle = $"Issues: {issueTask.Result.Count}\nPR: {prTask.Result.Count}";

			await Manager.SetTitleAsync(streamDeckContext, outTitle);
		}

		//public override async Task OnWillDisappear(StreamDeckEventPayload args)
		//{

		//	var settings = new { counter = _Counter };

		//	await Manager.SetSettingsAsync(args.context, settings);
		//}

		//public override Task OnPropertyInspectorConnected(PropertyInspectorEventPayload args)
		//{
		// 		_IsPropertyInspectorConnected = true;
		// 		return Task.CompletedTask;
		//}

		//public override Task OnPropertyInspectorDisconnected(PropertyInspectorEventPayload args)
		//{
		//  _IsPropertyInspectorConnected = false;
		//  return Task.CompletedTask;
		//}

		public async override Task OnPropertyInspectorMessageReceived(PropertyInspectorEventPayload args)
		{
			if (args.PayloadHasProperty("github_repository"))
			{
				_Project = args.GetPayloadValue<string>("github_repository");
				await SetGitHubTitles(args.context);
			}

		}

	}
}
