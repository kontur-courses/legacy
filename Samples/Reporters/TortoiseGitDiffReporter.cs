using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;

namespace Samples.Reporters
{
/*
К сожалению в ApprovalTests нет готового репортера для TortoiseGitMerge.
Но зато есть для TortoiseMerge из SVN. Поэтому легко по образу и подобию сделать свой TortoiseGitDiffReporter.

TortoiseGitMerge без Ribbon работает быстрее! Это существенно, если надо заапрувить много тестов.
*/
	public class TortoiseGitDiffReporter : GenericDiffReporter
	{
		private static readonly string PATH =
			DotNet4Utilities.GetPathInProgramFilesX86(
				string.Format("TortoiseGit{0}bin{0}TortoiseGitMerge.exe",
					System.IO.Path.DirectorySeparatorChar));

		public static readonly TortoiseGitDiffReporter INSTANCE = new TortoiseGitDiffReporter();

		public TortoiseGitDiffReporter()
			: base(
				PATH,
				"Could not find TortoiseGitMerge at {0}, please install it (it's part of TortoiseGit) https://tortoisegit.org/ "
					.FormatWith(PATH))
		{
		}
	}
}