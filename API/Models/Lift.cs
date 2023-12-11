using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace API.Models
{
        public class Lift
        {
                public int Cap { get; set; }
                public int NumFloors { get; set; }
                public int StartFloor { get; set; }
                public int TimeToTravelOneFloor { get; set; }
                public List<int> IdleFloors { get; set; }
                public List<LiftUser> Users { get; set; }
                public int CurrFloor { get; set; }
                public int Direction { get; set; }
                public int NumUsers { get; set; }
                public bool Idling { get; set; }
                public List<string> Route { get; set; }
                public int SecondCount { get; set; }
                public LiftAlgoOutput LiftAlgoOutput { get; set; }

                public Lift(int cap, int numFloors, int timeToTravelOneFloor, int startFloor, List<int> idleFloors, List<LiftUser> users, LiftAlgoOutput liftAlgoOutput)
                {
                        Cap = cap;
                        NumFloors = numFloors;
                        TimeToTravelOneFloor = timeToTravelOneFloor;
                        IdleFloors = idleFloors;
                        Users = users;
                        StartFloor = startFloor;
                        CurrFloor = startFloor;
                        Direction = 1;
                        NumUsers = 0;
                        Idling = true;
                        Route = new List<string> { $"Start at floor {startFloor}" };
                        SecondCount = 0; //users[0].TimeCalled;
                        LiftAlgoOutput = liftAlgoOutput;
                }

                public void AlgoOutput()
                {
                        List<LiftUser> callQueue = CallsAhead().Concat(CallsBehind()).ToList();
                        List<LiftUser> usersInLift = Users.Where(user => user.PickUpTime < SecondCount && SecondCount < user.CompleteTime).ToList();
                        LiftState liftState = new LiftState(usersInLift, callQueue) { TimeElapsed = SecondCount, CurrentFloor = CurrFloor };
                        LiftAlgoOutput.LiftStates.Add(liftState);
                }

                public void Log(string msg)
                {
                        LiftAlgoOutput.LiftLog.Add(msg);
                }

                public void PickUpCallers()
                {
                        while (NumUsers < Cap)
                        {
                                List<LiftUser> callers = Users.Where(user => user.AtFloor == CurrFloor && user.TimeCalled <= SecondCount && user.PickUpTime == null).OrderBy(user => user.TimeCalled).Select(user => user).ToList();

                                if (callers.Any())
                                {
                                        AttendCall(callers[0]);
                                }
                                else
                                {
                                        break;
                                }
                        }

                }

                public void DropOffPassengers(int currentFloor)
                {
                        List<LiftUser> orders = Users.Where(user => user.GoingToFloor == currentFloor && user.TimeCalled <= SecondCount && user.PickUpTime != null && user.CompleteTime == null).Select(user => user).ToList();

                        if (orders.Any())
                        {
                                foreach (var order in orders)
                                {
                                        CompleteOrder(order);
                                }
                        }
                }

                public void ChangeDirection()
                {
                        Direction = -Direction;
                }

                public void MoveOne()
                {
                        Idling = false;
                        AlgoOutput();
                        SecondCount += TimeToTravelOneFloor;
                        CurrFloor += Direction;
                        Log($"At floor {CurrFloor}");
                }

                public List<LiftUser> NearestOrders()
                {
                        List<LiftUser> orders = GetOrders();
                        List<int> orderDistances = orders.Select(order => Math.Abs(CurrFloor - order.GoingToFloor)).ToList();
                        int minDist = orderDistances.Min();
                        IEnumerable<Tuple<int, int>> enumerateOrderDistances = orderDistances.Select((x, i) => new Tuple<int, int>(i, x));
                        List<int> indices = enumerateOrderDistances.Where(tuple => tuple.Item2 == minDist).Select(tuple => tuple.Item1).ToList();
                        List<LiftUser> nearestOrders = indices.Select(index => orders[index]).ToList();

                        return nearestOrders;
                }

                public List<LiftUser> NearestCalls()
                {
                        List<LiftUser> calls = GetCalls();
                        List<int> callDistances = calls.Select(call => Math.Abs(CurrFloor - call.AtFloor)).ToList();
                        int minDist = callDistances.Min();
                        IEnumerable<Tuple<int, int>> enumerateOrderDistances = callDistances.Select((x, i) => new Tuple<int, int>(i, x));
                        List<int> indices = enumerateOrderDistances.Where(tuple => tuple.Item2 == minDist).Select(tuple => tuple.Item1).ToList();
                        List<LiftUser> nearestCalls = indices.Select(index => calls[index]).ToList();

                        return nearestCalls;
                }

                public List<LiftUser> FurthestOrders()
                {
                        List<LiftUser> orders = GetOrders();
                        List<int> orderDistances = orders.Select(order => Math.Abs(CurrFloor - order.GoingToFloor)).ToList();
                        int maxDist = orderDistances.Max();
                        IEnumerable<Tuple<int, int>> enumerateOrderDistances = orderDistances.Select((x, i) => new Tuple<int, int>(i, x));
                        List<int> indices = enumerateOrderDistances.Where(tuple => tuple.Item2 == maxDist).Select(tuple => tuple.Item1).ToList();
                        List<LiftUser> furthestOrders = indices.Select(index => orders[index]).ToList();

                        return furthestOrders;
                }

                public void AttendCall(LiftUser userCalling)
                {
                        Users.Find(user => user.PersonID == userCalling.PersonID).PickUpTime = SecondCount;
                        Log($"Pick up Person {userCalling.PersonID} on floor {userCalling.AtFloor}. They have been waiting for {SecondCount - userCalling.TimeCalled} seconds");
                        NumUsers += 1;
                        Log($"The lift has {NumUsers} users inside");
                        Route.Add($"C_{userCalling.AtFloor}");
                }

                public void CompleteOrder(LiftUser userExiting)
                {
                        Users.Find(person => person.PersonID == userExiting.PersonID).CompleteTime = SecondCount;
                        Log($"Drop off Person {userExiting.PersonID} on floor {userExiting.GoingToFloor}. Their wait time plus journey time was {SecondCount - userExiting.TimeCalled} seconds");
                        NumUsers -= 1;
                        Log($"The lift has {NumUsers} users inside");
                        Route.Add($"O_{userExiting.GoingToFloor}");
                }

                public List<LiftUser> GetCalls()
                {
                        return Users.Where(user => user.TimeCalled <= SecondCount && user.PickUpTime == null).Select(user => user).ToList();
                }

                public List<LiftUser> GetOrders()
                {
                        return Users.Where(user => user.TimeCalled <= SecondCount && user.PickUpTime != null && user.CompleteTime == null).Select(user => user).ToList();
                }

                public List<LiftUser> CallsAhead()
                {
                        List<LiftUser> callsAhead = new List<LiftUser>();
                        if (Direction == -1)
                        {
                                callsAhead = Users.Where(user => CurrFloor - user.AtFloor >= 0 && user.TimeCalled <= SecondCount && user.PickUpTime == null).Select(user => user).ToList().OrderByDescending(user => user.AtFloor).ToList();
                        }
                        else
                        {
                                callsAhead = Users.Where(user => CurrFloor - user.AtFloor <= 0 && user.TimeCalled <= SecondCount && user.PickUpTime == null).Select(user => user).ToList().OrderBy(user => user.AtFloor).ToList();
                        }
                        return callsAhead;
                }

                public List<LiftUser> CallsBehind()
                {
                        List<LiftUser> callsBehind = new List<LiftUser>();

                        if (Direction == -1)
                        {
                                callsBehind = Users.Where(user => CurrFloor - user.AtFloor < 0 && user.TimeCalled <= SecondCount && user.PickUpTime == null).Select(user => user).ToList().OrderBy(user => user.AtFloor).ToList();
                        }
                        else
                        {
                                callsBehind = Users.Where(user => CurrFloor - user.AtFloor > 0 && user.TimeCalled <= SecondCount && user.PickUpTime == null).Select(user => user).ToList().OrderByDescending(user => user.AtFloor).ToList();
                        }
                        return callsBehind;
                }

                public List<LiftUser> OrdersAhead()
                {
                        List<LiftUser> ordersAhead = new List<LiftUser>();

                        if (Direction == -1)
                        {
                                ordersAhead = Users.Where(user => CurrFloor - user.GoingToFloor >= 0 && user.TimeCalled <= SecondCount && user.PickUpTime != null && user.CompleteTime == null).Select(user => user).ToList().OrderByDescending(user => user.GoingToFloor).ToList();
                        }
                        else
                        {
                                ordersAhead = Users.Where(user => CurrFloor - user.GoingToFloor <= 0 && user.TimeCalled <= SecondCount && user.PickUpTime != null && user.CompleteTime == null).Select(user => user).ToList().OrderBy(user => user.GoingToFloor).ToList();
                        }
                        return ordersAhead;
                }

                public void NearestCallSetDirection()
                {
                        LiftUser nearestCall = NearestCalls()[0];

                        if (nearestCall.AtFloor != CurrFloor)
                        {
                                if (Direction == 1 && nearestCall.AtFloor < CurrFloor || Direction == -1 && nearestCall.AtFloor > CurrFloor)
                                {
                                        ChangeDirection();
                                }
                        }

                }

                public void JustOrdersSetDirection()
                {
                        LiftUser furthestOrder = FurthestOrders()[0];
                        LiftUser nearestOrder = NearestOrders()[0];
                        List<LiftUser> orders = GetOrders();
                        if (orders.All(x => x.GoingToFloor <= CurrFloor) || orders.All(x => x.GoingToFloor >= CurrFloor))
                        {
                                if (Direction == 1 && nearestOrder.GoingToFloor < CurrFloor || Direction == -1 && nearestOrder.GoingToFloor > CurrFloor)
                                {
                                        ChangeDirection();
                                }
                        }
                        else if (Direction == 1 && furthestOrder.GoingToFloor > CurrFloor || Direction == -1 && furthestOrder.GoingToFloor < CurrFloor)
                        {
                                ChangeDirection();
                        }
                }

                public void ManageCallsAndOrdersAhead()
                {
                        while (OrdersAhead().Any() || (CallsAhead().Any() && NumUsers < Cap))
                        {
                                MoveOne();
                                DropOffPassengers(CurrFloor);
                                PickUpCallers();
                        }
                }

                public void GoTowardsIdleFloor()
                {
                        List<int> idleFloorDistances = IdleFloors.Select(floor => Math.Abs(CurrFloor - floor)).ToList();
                        int minDist = idleFloorDistances.Min();
                        IEnumerable<Tuple<int, int>> enumerateIdleFloorDistances = idleFloorDistances.Select((x, i) => new Tuple<int, int>(i, x));
                        List<int> indices = enumerateIdleFloorDistances.Where(tuple => tuple.Item2 == minDist).Select(tuple => tuple.Item1).ToList();
                        int nearestIdleFloor = IdleFloors[indices[0]];
                        if (nearestIdleFloor != CurrFloor)
                        {
                                if (nearestIdleFloor < CurrFloor && Direction == 1 || nearestIdleFloor > CurrFloor && Direction == -1)
                                {
                                        ChangeDirection();
                                }
                        }

                        List<LiftUser> calls = GetCalls();
                        while (CurrFloor != nearestIdleFloor)
                        {
                                MoveOne();
                                calls = GetCalls();
                                if (calls.Count != 0)
                                {
                                        ManageRoute();
                                }
                        }
                        AlgoOutput();
                }

                public void ManageRoute()
                {
                        while (GetCalls().Concat(GetOrders()).Any())
                        {
                                ManageCallsAndOrdersAhead();
                                ChangeDirection();
                        }
                        GoTowardsIdleFloor();
                }

                public void RunFromIdle()
                {
                        while (Users.Where(user => user.PickUpTime == null).ToList().Any())
                        {
                                // SecondCount = Users.Where(user => user.PickUpTime == null).ToList()[0].TimeCalled;
                                Log($"Start from floor {CurrFloor}");
                                List<LiftUser> calls = GetCalls();
                                if (calls.Count >= 1)
                                {
                                        NearestCallSetDirection();
                                        if (calls.Count == 1)
                                        {
                                                var firstCall = calls[0];
                                                bool moreCalls = false;
                                                while (CurrFloor != firstCall.AtFloor)
                                                {
                                                        MoveOne();
                                                        calls = GetCalls();
                                                        if (calls.Count > 1)
                                                        {
                                                                moreCalls = true;
                                                                break;
                                                        }
                                                }
                                                if (CurrFloor == firstCall.AtFloor && !moreCalls)
                                                {
                                                        AttendCall(firstCall);
                                                        JustOrdersSetDirection();
                                                }
                                                else
                                                {
                                                        PickUpCallers();
                                                }
                                        }
                                        else
                                        {
                                                PickUpCallers();
                                        }
                                        ManageRoute();
                                        string RouteString = string.Join(", ", Route);
                                        // Log($"Arrived back to idle floor after {SecondCount} seconds. Route taken since last idle: {RouteString}");
                                        Log($"Arrived back to idle floor after {SecondCount} seconds.");
                                }
                                else
                                {
                                        AlgoOutput();
                                        SecondCount += 1;
                                        // Log($"At floor {CurrFloor}");
                                }
                        }

                }
        }
}

