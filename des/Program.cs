namespace des
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.CancelKeyPress += delegate
            {
                //System.Console.ReadKey();
            };
            //if (args.Length < 2) return;

            var simulation = new Simulation();
            var inputParser = new InputParser();

            var topologyPath = args[0];
            inputParser.ParseTopology(topologyPath);
            foreach (var member in inputParser.Members)
            {
                simulation.AddPerson(member.Item1, member.Item2);
            }

            var scenarioPath = args[1];
            inputParser.ParseScenario(scenarioPath);
            simulation.RunScenario(inputParser.FirstSenderId, inputParser.FirstReceiverId);
        }
    }
}
