import React from 'react';
import css from "./styles.module.css";
import { FaPerson } from "react-icons/fa6";

const Floors = ({ userCalls }) => {
        const numFloors = userCalls.length;
        return (
                <div className={css["people-floors-container"]}>
                        <div className={css["people-container"]}>
                                {[...Array(numFloors).keys()].reverse().map((int) => {
                                        return <div key={`StickPeopleAtFloor${int}`} className={css["people-floor-container"]}>
                                                {
                                                        [...Array(userCalls[int].length).keys()].map((num) => {
                                                                return <div key={`Person${num}AtFloor${int}`} className={css["person-icon-wrapper"]}>
                                                                        <FaPerson />
                                                                </div>
                                                        })}
                                        </div>
                                })}
                        </div>
                        <div className={css["floors-container"]}>
                                {[...Array(numFloors).keys()].map((int) => { return <div key={(numFloors - 1) - int} >{(numFloors - 1) - int}</div> })}
                        </div>
                </div>
        );
}

export default Floors;