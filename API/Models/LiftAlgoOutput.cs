using System;
using Microsoft.Net.Http.Headers;

namespace API.Models
{
        public class LiftAlgoOutput
        {
                public List<LiftState> LiftStates { get; set; }
                public List<string> LiftLog { get; set; }
                public LiftAlgoOutput(List<LiftState> liftStates, List<string> liftLog)
                {
                        LiftStates = liftStates;
                        LiftLog = liftLog;
                }
        }
}
