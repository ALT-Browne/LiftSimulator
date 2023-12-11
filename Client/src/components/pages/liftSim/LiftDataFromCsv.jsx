import { useState, useEffect } from 'react';
import css from "./styles.module.css";

const LiftDataFromCsv = ({ updateLiftData }) => {
        // const url = document.getElementsByTagName('base')[0].getAttribute('href') + "liftsimulator";
        const [file, setFile] = useState(null);
        const [error, setError] = useState(null);

        const handleOnSubmit = (e) => {
                e.preventDefault();
                const formData = new FormData();
                formData.append('name', "csvFileInput");
                formData.append('file', file);
                sendLiftData(formData);
        }

        const handleOnChange = (e) => {
                setFile(e.target.files[0]);
        };

        const updateError = (error) => {
                setError(error);
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

        const sendLiftData = (formData) => {
                const requestOptions = {
                        method: 'POST',
                        // headers: {
                        //         'Content-Type': 'multipart/form-data'
                        // },
                        body: formData
                }

                return fetch(`https://localhost:7109/LiftSim/sendLiftDataFromCsv`, requestOptions)
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

        return (
                <div>
                        <div className={css["card"]}>
                                <div>Option 2: Enter lift users by uploading a csv file. Please use the format: AtFloor,GoingToFloor,TimeCalledInSeconds. This method will use 10 floors, 10s between each floor and a lift capacity of 8</div>
                                <form>
                                        <input
                                                type={"file"}
                                                id={"csvFileInput"}
                                                accept={".csv"}
                                                onChange={handleOnChange}
                                        />
                                        <button className={css["import-button"]} onClick={(e) => handleOnSubmit(e)}>
                                                Upload
                                        </button>
                                </form>
                                {error && <div className={css["db-error"]}>{error}</div>}
                        </div>
                </div>
        )
}

export default LiftDataFromCsv;
