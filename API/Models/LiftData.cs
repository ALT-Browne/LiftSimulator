using System;
using Microsoft.Net.Http.Headers;

namespace API.Models
{
        public class LiftData
        {
                public int Floors { get; set; }
                public int Capacity { get; set; }
                public int TimeToTravelOneFloor { get; set; }
                public List<LiftUser> Users { get; set; }
                public LiftData(List<LiftUser> users)
                {
                        Users = users;
                }
        }
}


