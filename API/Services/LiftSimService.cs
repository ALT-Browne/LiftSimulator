using API.Models;
using API.Services.Interfaces;

namespace API.Services
{
        public class LiftSimService : ILiftSimService
        {
                public LiftSimService()
                {
                }

                public Result<Lift> SendLiftDataFromCsv(IFormFile file)
                {
                        using (var reader = new StreamReader(file.OpenReadStream()))
                        {
                                List<string> lines = new List<string>();
                                while (!reader.EndOfStream)
                                {
                                        lines.Add(reader.ReadLine());
                                }

                                List<LiftUser> users = new List<LiftUser>();
                                for (int i = 0; i < lines.Count; i++)
                                {
                                        List<int> userData = lines[i].Split(',').Select(s => int.Parse(s)).ToList();
                                        users.Add(new LiftUser() { AtFloor = userData[0], GoingToFloor = userData[1], TimeCalled = userData[2] });
                                }

                                // Order the users by TimeCalled so that their PersonIDs make sense
                                List<LiftUser> orderedUsers = users.OrderBy(user => user.TimeCalled).ToList();
                                for (int i = 0; i < orderedUsers.Count; i++)
                                {
                                        orderedUsers[i].PersonID = i + 1;
                                }

                                // check for errors in the csv data
                                string errorMessage = "";
                                foreach (LiftUser user in orderedUsers)
                                {
                                        if (user.AtFloor == user.GoingToFloor)
                                        {
                                                errorMessage = "The values of 'At floor' and 'Going to floor' must be different.";
                                                break;
                                        }
                                }
                                if (errorMessage != "")
                                        return new Result<Lift>(errorMessage);

                                // Create new lift with given orderedUsers list
                                Lift Lift = new Lift(8, 10, 10, 5, new List<int> { 4, 5 }, orderedUsers, new LiftAlgoOutput(new List<LiftState>(), new List<string>()));
                                // Run lift algo
                                Lift.RunFromIdle();
                                // Remove duplicate lines from lift log
                                List<string> liftLog = Lift.LiftAlgoOutput.LiftLog;
                                List<string> newLiftLog = liftLog.Where((line, i) => i == 0 || line != liftLog[i - 1]).ToList();
                                Lift.LiftAlgoOutput.LiftLog = newLiftLog;
                                // Return lift which contains the Algo output
                                return new Result<Lift>(Lift);
                        }
                }

                public Result<Lift> SendLiftDataFromUser(LiftData liftDataRequest)
                {
                        List<LiftUser> users = new List<LiftUser>();
                        for (int i = 0; i < liftDataRequest.Users.Count; i++)
                        {
                                users.Add(new LiftUser() { AtFloor = liftDataRequest.Users[i].AtFloor, GoingToFloor = liftDataRequest.Users[i].GoingToFloor, TimeCalled = liftDataRequest.Users[i].TimeCalled });
                        }

                        // Order the users by TimeCalled so that their PersonIDs make sense
                        List<LiftUser> orderedUsers = users.OrderBy(user => user.TimeCalled).ToList();
                        for (int i = 0; i < orderedUsers.Count; i++)
                        {
                                orderedUsers[i].PersonID = i + 1;
                        }

                        int startFloor = liftDataRequest.Floors % 2 == 0 ? liftDataRequest.Floors / 2 : (liftDataRequest.Floors - 1) / 2;
                        List<int> idleFloors = liftDataRequest.Floors % 2 == 0 ? new List<int> { (liftDataRequest.Floors - 2) / 2, liftDataRequest.Floors / 2 } : new List<int> { (liftDataRequest.Floors - 1) / 2 };

                        // Create new lift with given orderedUsers list
                        Lift Lift = new Lift(liftDataRequest.Capacity, liftDataRequest.Floors, liftDataRequest.TimeToTravelOneFloor, startFloor, idleFloors, orderedUsers, new LiftAlgoOutput(new List<LiftState>(), new List<string>()));
                        // Run lift algo
                        Lift.RunFromIdle();
                        // Remove duplicate lines from lift log
                        List<string> liftLog = Lift.LiftAlgoOutput.LiftLog;
                        List<string> newLiftLog = liftLog.Where((line, i) => i == 0 || line != liftLog[i - 1]).ToList();
                        Lift.LiftAlgoOutput.LiftLog = newLiftLog;
                        // Return lift which contains the Algo output
                        return new Result<Lift>(Lift);
                }
        }
}

