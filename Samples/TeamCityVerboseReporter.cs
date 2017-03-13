using ApprovalTests.Reporters;
using Samples;

/*
У ApprovalTests есть такое понятие как FrontLoaderReporter — это репортер, который проверяет, 
может ли он работать в текущем окружении и если может, заменяет собой репортер из атрибута UseReporter.

Так вот есть DefaultFrontLoaderReporter, который работает по умолчанию и он на агенте Тимсити считает, 
что надо вмешаться и подменить все репортеры собой. 
Поэтому какие бы ни были репортеры, аутпут при падении теста будет дурацким и малоинформативным. 
В принципе, все равно, чтобы тест поднять, придется перезапустить его на локальной машине, поэтому это не очень страшно.

Но если все же хочется больше информации при падении, то можно подменить FrontLoaderReporter на свой.
Делается это атрибутом сборки:
*/

[assembly: FrontLoadedReporter(typeof(TeamCityVerboseReporter))]

namespace Samples
{
	public class TeamCityVerboseReporter : TeamCityReporter
	{
		private readonly FrameworkAssertReporter realReporter = new FrameworkAssertReporter();

		public override void Report(string approved, string received)
		{
			realReporter.Report(approved, received);
		}
	}
}