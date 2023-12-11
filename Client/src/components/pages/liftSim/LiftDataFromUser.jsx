import { useState, useEffect } from 'react';
import css from "./styles.module.css";

const LiftDataFromUser = ({ updateLiftData }) => {
        // const url = document.getElementsByTagName('base')[0].getAttribute('href') + "liftsimulator";
        const [formValues, setFormValues] = useState({
                floors: 10,
                time: 10,
                capacity: 8
        });
        const [formUserValues, setFormUserValues] = useState([{ atFloor: null, goingToFloor: null, timeCalled: null }])
        const [floorsFocused, setFloorsFocused] = useState(false);
        const [timeFocused, setTimeFocused] = useState(false);
        const [capacityFocused, setCapacityFocused] = useState(false);
        const [error, setError] = useState(null);

        const updateError = (error) => {
                setError(error);
        }

        const handleSubmit = (e) => {
                e.preventDefault();
                const formElement = e.target;
                const isValid = formElement.checkValidity();
                if (isValid && !error) {
                        sendLiftData();
                };
        }

        const handleChange = (e) => {
                setFormValues(prev => ({ ...prev, [e.target.name]: e.target.value }))
        }

        const handleErrors = (response) => {
                return response.json().then(body => {
                        if (!response.ok) {
                                // logout user if 401 Unauthorized or 403 Forbidden response returned from api
                                if ([401, 403].includes(response.status)) {

                                }

                                return Promise.reject(body.errorMessage);
                        }
                        return body;
                });
        }

        const sendLiftData = () => {
                const requestOptions = {
                        method: 'POST',
                        headers: {
                                'Content-Type': 'application/json;charset=utf-8'
                        },
                        body: JSON.stringify({ floors: formValues.floors, timeToTravelOneFloor: formValues.time, capacity: formValues.capacity, users: formUserValues })
                }

                return fetch(`https://localhost:7109/LiftSim/sendLiftDataFromUser`, requestOptions)
                        .then(handleErrors)
                        .then(body => {
                                updateLiftData(body.payload);
                                updateError(null);
                        })
                        .catch((error) => {
                                console.log('Request failed:', error);
                                updateError(error)
                        });
        }

        const handleUserChange = (i, e) => {
                if ((e.target.name === "atFloor" && e.target.value === formUserValues[i].goingToFloor) || (e.target.name === "goingToFloor" && e.target.value === formUserValues[i].atFloor)) {
                        setError("The values of 'At Floor' and 'Going to Floor' must be different");
                }
                else {
                        setError(null);
                }

                let newFormValues = [...formUserValues];
                newFormValues[i][e.target.name] = e.target.value;
                setFormUserValues(newFormValues);
        }

        const addFormFields = () => {
                setFormUserValues([...formUserValues, { atFloor: null, goingToFloor: null, timeCalled: null }])
        }

        const removeFormFields = (i) => {
                let newFormValues = [...formUserValues];
                newFormValues.splice(i, 1);
                setFormUserValues(newFormValues)
        }

        return (
                <div>
                        <div className={css["card"]}>
                                <div>Option 1: Enter lift data manually. Please enter users in ascending order of the time (in seconds) they call the lift.</div>
                                <form onSubmit={handleSubmit}>
                                        <div className={css["form-input"]}>
                                                <label>Floors</label>
                                                <input
                                                        id="floors"
                                                        name="floors"
                                                        type="number"
                                                        value={formValues.floors}
                                                        placeholder="10"
                                                        required={true}
                                                        min="2"
                                                        max="10"
                                                        onChange={handleChange}
                                                        onBlur={() => setFloorsFocused(true)}
                                                        focused={floorsFocused.toString()}
                                                />
                                                <span>Floors should be an integer from 2 - 10!</span>
                                        </div>
                                        <div className={css["form-input"]}>
                                                <label>Time (s) to travel one floor</label>
                                                <input
                                                        id="time"
                                                        name="time"
                                                        type="number"
                                                        value={formValues.time}
                                                        placeholder="10"
                                                        required={true}
                                                        min="1"
                                                        max="20"
                                                        onChange={handleChange}
                                                        onBlur={() => setTimeFocused(true)}
                                                        focused={timeFocused.toString()}
                                                />
                                                <span>Time should be an integer from 1 - 20!</span>
                                        </div>
                                        <div className={css["form-input"]}>
                                                <label>Capacity</label>
                                                <input
                                                        id="capacity"
                                                        name="capacity"
                                                        type="number"
                                                        value={formValues.capacity}
                                                        placeholder="8"
                                                        required={true}
                                                        min="1"
                                                        max="20"
                                                        onChange={handleChange}
                                                        onBlur={() => setCapacityFocused(true)}
                                                        focused={capacityFocused.toString()}
                                                />
                                                <span>Capacity should be an integer from 1 - 20!</span>
                                        </div>
                                        <label>Lift users:</label>
                                        {formUserValues.map((element, index) => (
                                                <div className={css["form-input"]} key={index}>
                                                        <label>At floor</label>
                                                        <input
                                                                id="atFloor"
                                                                type="number"
                                                                name="atFloor"
                                                                value={element.atFloor || ""}
                                                                required={true}
                                                                min="0"
                                                                max={formValues.floors - 1}
                                                                onChange={e => handleUserChange(index, e)}
                                                        />
                                                        <label>Going to floor</label>
                                                        <input
                                                                id="goingToFloor"
                                                                type="number"
                                                                name="goingToFloor"
                                                                value={element.goingToFloor || ""}
                                                                required={true}
                                                                min="0"
                                                                max={formValues.floors - 1}
                                                                onChange={e => handleUserChange(index, e)}
                                                        />
                                                        <label>Time called (s)</label>
                                                        <input
                                                                id="timeCalled"
                                                                type="number"
                                                                name="timeCalled"
                                                                value={element.timeCalled || ""}
                                                                required={true}
                                                                min="1"
                                                                onChange={e => handleUserChange(index, e)}
                                                        />
                                                        {
                                                                index ?
                                                                        <button type="button" className={css["remove-button"]} onClick={() => removeFormFields(index)}>Remove</button>
                                                                        : null
                                                        }
                                                </div>
                                        ))}
                                        <div className={css["button-section"]}>
                                                <button className={css["add-button"]} type="button" onClick={() => addFormFields()}>Add</button>
                                        </div>

                                        <button className={css["confirm-button"]}>Confirm</button>
                                        {error && <div className={css["db-error"]}>{error}</div>}
                                </form>
                        </div>
                </div>
        )
}

export default LiftDataFromUser;
