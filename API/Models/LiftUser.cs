using System;
using Microsoft.Net.Http.Headers;

namespace API.Models
{
        public class LiftUser
        {
                public int? PersonID { get; set; }
                public int AtFloor { get; set; }
                public int GoingToFloor { get; set; }
                public int TimeCalled { get; set; }
                public int? PickUpTime { get; set; }
                public int? CompleteTime { get; set; }
                public LiftUser()
                {

                }
        }
}


