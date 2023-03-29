let flag = "";
let oldBest = "";
let body_hidden = "initial";//"none"; // "initial"; //

elRoot = document.querySelector(":root");
body = document.querySelector("body");
elSpeed = document.getElementById("speed");
elGear = document.getElementById("gear");
elFlag = document.getElementById("flag");
elAirTemp = document.getElementById("airtemp");
elRoadTemp = document.getElementById("roadtemp");
elCurTime = document.getElementById("currenlaptime");
elLastLapTime = document.getElementById("lastlaptime");
elBestLapTime = document.getElementById("bestlaptime");
elSessionType = document.getElementById("sessionType");
elRemain = document.getElementById("remainingTime");
elDelta = document.getElementById("delta");
elSector1 = document.getElementById("sector1");
elSector2 = document.getElementById("sector2");
elSector3 = document.getElementById("sector3");
elSector1lbl = document.getElementById("lblSector1");
elSector2lbl = document.getElementById("lblSector2");
elSector3lbl = document.getElementById("lblSector3");
elDriverName = document.getElementById("driverName");
elCarName = document.getElementById("carName");
elCarNumber = document.getElementById("carNumber");
elFuelLevel = document.getElementById("fuelLevel");
elPosition = document.getElementById("position");
elLap = document.getElementById("lap");
body.style.setProperty("display", body_hidden);
elCup = document.getElementsByClassName("Winner");

elPedalLabels = document.getElementsByClassName("PedalLabel");

function connectServer() {
    wsUri = "ws://" + window.location.hostname + ":" + window.location.port + "/",
        websocket = new WebSocket(wsUri);

    websocket.onmessage = function (e) {
        let telemetry = JSON.parse(e.data);

        if (telemetry.Flag != flag) {
            flag = telemetry.Flag;
            if (flag == "")
                elFlag.classList.replace('FlagActive', 'Flag');
            else {
                url = window.getComputedStyle(elRoot).getPropertyValue("--flag-" + flag.toLowerCase() + "-img");
                elRoot.style.setProperty("--flag-img", url);
                elFlag.classList.replace('Flag', 'FlagActive');
            }
        }

        if (telemetry.BestLapTime != oldBest) {
            oldBest = telemetry.BestLapTime;
            setTimeout(() => {
                parent = elBestLapTime.parentElement;
                if (!parent.classList.contains("Active"))
                    parent.classList.add("Active");
                setTimeout(() => {
                    parent = elBestLapTime.parentElement;
                    if (parent.classList.contains("Active"))
                        parent.classList.remove("Active");
                }, 1000);
            }, 10);
        }

        if (telemetry.SessionType == "") {
            body.style.setProperty("display", body_hidden);
            oldBest = "";
            flag = "";
        }
        else {
            elSpeed.innerHTML = telemetry.Speed;
            elGear.innerHTML = telemetry.Gear;
            elAirTemp.innerHTML = telemetry.AirTemp;
            elRoadTemp.innerHTML = telemetry.RoadTemp;
            elSessionType.innerHTML = telemetry.SessionType;
            elRemain.innerHTML = telemetry.Remain;
            elFuelLevel.innerHTML = telemetry.FuelLevel;

            elSector1.innerHTML = telemetry.Sector1Time;
            elSector2.innerHTML = telemetry.Sector2Time;
            elSector3.innerHTML = telemetry.Sector3Time;
            elDriverName.innerHTML = telemetry.Delta;
            elCurTime.innerHTML = telemetry.LapTime;
            elBestLapTime.innerHTML = telemetry.BestLapTime;
            elLastLapTime.innerHTML = telemetry.LastLapTime;
            elDelta.innerHTML = telemetry.Delta;

            elDriverName.innerHTML = telemetry.DriverName;
            elCarName.innerHTML = telemetry.CarName;
            elCarNumber.innerHTML = telemetry.CarNumber;
            elPosition.innerHTML = telemetry.Position;
            elLap.innerHTML = telemetry.Lap;

            if (telemetry.Lap > 1 && telemetry.FinishStatus == "Finished" && telemetry.Position < 4) {
                if (!elCup[0].classList.contains("Active"))
                    elCup[0].classList.add("Active");
            }
            else {
                if (elCup[0].classList.contains("Active"))
                    elCup[0].classList.remove("Active");            
            }

            if (telemetry.Flag == "Chequered") {
                if (!body.classList.contains("Finished"))
                    body.classList.add("Finished");
            }
            else if (body.classList.contains("Finished"))
                body.classList.remove("Finished");

            elSector1lbl.style.borderBottom = telemetry.Sector == 1 ? "thin solid #e1e1e1" : "none";
            elSector2lbl.style.borderBottom = telemetry.Sector == 2 ? "thin solid #e1e1e1" : "none";
            elSector3lbl.style.borderBottom = telemetry.Sector == 3 ? "thin solid #e1e1e1" : "none";

            elPedalLabels[0].innerHTML = telemetry.Clutch;
            elPedalLabels[1].innerHTML = telemetry.Brake;
            elPedalLabels[2].innerHTML = telemetry.Throttle;

            elRoot.style.setProperty("--brake", telemetry.Brake);
            elRoot.style.setProperty("--throttle", telemetry.Throttle);
            elRoot.style.setProperty("--clutch", telemetry.Clutch);

            elRoot.style.setProperty("--sector1-color", telemetry.Sector1Color);
            elRoot.style.setProperty("--sector2-color", telemetry.Sector2Color);
            elRoot.style.setProperty("--sector3-color", telemetry.Sector3Color);
            elRoot.style.setProperty("--gear-color", telemetry.GearColor);
            elRoot.style.setProperty("--delta-color", telemetry.DeltaColor);
            body.style.setProperty("display", "initial");
        }
    };

    websocket.onopen = function (e) {
        clearTimeout(connectServer);
        body.style.setProperty("display", "initial");
    };

    websocket.onclose = function (e) {
        body.style.setProperty("display", body_hidden);
        setTimeout(function () {
            connectServer();
        }, 1000);
    };
}

setTimeout(function () {
    connectServer();
}, 1000);