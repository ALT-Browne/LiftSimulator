import { BrowserRouter } from "react-router-dom";
import React from "react";
import ReactDOM from "react-dom/client";
import '@fontsource/roboto';
import LiftSim from './components/pages/liftSim/LiftSim';

ReactDOM.createRoot(document.getElementById("root")).render(
        <BrowserRouter>
                <LiftSim />
        </BrowserRouter>
);
