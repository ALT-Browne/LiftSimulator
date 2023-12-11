using System;
using Microsoft.Net.Http.Headers;

namespace API.Models
{
        public class LiftState
        {
                public int TimeElapsed { get; set; }
                public int CurrentFloor { get; set; }
                public List<LiftUser> UsersInLift { get; set; }
                public List<LiftUser> CallQueue { get; set; }
                public LiftState(List<LiftUser> usersInLift, List<LiftUser> callQueue)
                {
                        UsersInLift = usersInLift;
                        CallQueue = callQueue;
                }
        }
}


