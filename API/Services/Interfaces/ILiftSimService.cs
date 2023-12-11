using System;
using API.Models;

namespace API.Services.Interfaces
{
        public interface ILiftSimService
        {
                Result<Lift> SendLiftDataFromCsv(IFormFile file);
                Result<Lift> SendLiftDataFromUser(LiftData liftDataRequest);
        }
}

