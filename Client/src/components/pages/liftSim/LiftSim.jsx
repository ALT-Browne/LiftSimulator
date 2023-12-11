import React, { useState, useEffect } from 'react';
import Floors from "./Floors.jsx";
import LiftDataFromUser from "./LiftDataFromUser.jsx";
import LiftDataFromCsv from "./LiftDataFromCsv.jsx";
import css from "./styles.module.css";

const LiftSim = () => {
        const [num, setNum] = useState(0);
        const [startAnimation, setStartAnimation] = useState(false);
        const [liftStates, setLiftStates] = useState([{ callQueue: [], usersInLift: [], timeElapsed: 0 }]);
        const [nextFloor, setNextFloor] = useState(null);
        const [users, setUsers] = useState(null);
        const [timeToTravelOneFloor, setTimeToTravelOneFloor] = useState(10);
        const [numFloors, setNumFloors] = useState(10);
        const [startFloor, setStartFloor] = useState(null);
        const [liftLog, setLiftLog] = useState(null);
        const [showLog, setShowLog] = useState(false);

        let usersWaiting = [...Array(numFloors)].map(e => []);
        let usersInLift = [];
        let firstUserTimeCalled = 0;
        if (users) {
                users.filter(user => user.timeCalled <= num && num < user.pickUpTime).forEach(user => usersWaiting[user.atFloor].push(user));
                users.filter(user => user.pickUpTime < num && num < user.completeTime).forEach(user => usersInLift.push(user));
                firstUserTimeCalled = users[0].timeCalled;
        }

        const updateLiftData = (data) => {
                setLiftStates(data.liftAlgoOutput.liftStates);
                setUsers(data.users);
                setLiftLog(data.liftAlgoOutput.liftLog);
                setTimeToTravelOneFloor(data.timeToTravelOneFloor);
                setNumFloors(data.numFloors);
                setStartFloor(data.startFloor);
        }

        const resetAnimation = () => {
                setStartAnimation(false);
                setNum(0);
                setNextFloor(startFloor);
        }

        useEffect(() => {
                const interval = setInterval(() => {
                        if (startAnimation && num < liftStates.slice(-1)[0].timeElapsed) {
                                setNum(num + 1);
                                const nextState = liftStates.find(state => state.timeElapsed === num + timeToTravelOneFloor);
                                if (nextState)
                                        setNextFloor(nextState.currentFloor);
                        }
                }, 1000 / timeToTravelOneFloor);
                return () => clearInterval(interval);
        });

        return (
                <div>
                        {
                                liftStates === null || users === null
                                        ?
                                        <div className={css["lift-data"]}>
                                                <div className={css["header-card"]}>
                                                        <h1>LIFT DATA</h1>
                                                </div>
                                                <LiftDataFromUser updateLiftData={updateLiftData} />
                                                <LiftDataFromCsv updateLiftData={updateLiftData} />
                                        </div>
                                        :
                                        <div className={css["lift-animation-wrapper"]}>
                                                <div className={css["buttons-num-wrapper"]}>
                                                        <div className={css["num"]}>{num}</div>
                                                        <button className={css["start-animation-button"]} onClick={() => setStartAnimation(prev => !prev)}>{startAnimation ? "Stop" : "Start"}</button>
                                                        <button className={css["reset-animation-button"]} onClick={resetAnimation}>Reset</button>
                                                        {
                                                                showLog &&
                                                                <div className={css["lift-log-card"]}>
                                                                        {liftLog.map((line, index) => {
                                                                                return <div key={index}>{line}</div>;
                                                                        })}
                                                                </div>
                                                        }
                                                        <button className={css["lift-log-button"]} onClick={() => setShowLog(prev => !prev)}>Lift log</button>
                                                </div>
                                                <div className={css["floors-lift-info-container"]}>
                                                        <Floors userCalls={usersWaiting} />
                                                        <div className={`${css["lift"]} ${css["floor-" + `${numFloors}` + "-" + `${nextFloor ?? startFloor}`]}`}>
                                                                {liftStates ? usersInLift.length : 0}
                                                        </div>
                                                        <div className={css["users-in-lift-container"]}>
                                                                <div>
                                                                        People in lift:
                                                                </div>
                                                                {usersInLift.map((user) => {
                                                                        return <div key={`Person${user.personID}InLift`}>{`Person ${user.personID}`}</div>;
                                                                })}
                                                        </div>
                                                </div>
                                        </div>
                        }
                </div>
        );
}

export default LiftSim;