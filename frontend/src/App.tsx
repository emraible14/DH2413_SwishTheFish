import { useEffect, useState } from 'react';
import './App.css'
import type { FishConfig } from './utils/types';
import FishDesignerView from './views/FishDesignerView';
import StartView from './views/StartView';
import SelectDeviceView from './views/SelectDeviceView';
import SwipeView from './views/SwipeView';

function App() {

  const [viewState, setViewState] = useState(0);

  const defaultFish: FishConfig = {
    tailId: 1,
    bodyId: 1,
    headId: 1,
    color: '#000000',
    deviceId: null,
  };
  const [config, setConfig] = useState<FishConfig>(defaultFish);

  const [ws, setWs] = useState<WebSocket | null>(null);

  useEffect(() => {
    const websocket = new WebSocket("wss://dh2413-swishthefish.onrender.com");
    setWs(websocket)

    websocket.onopen = function () {
      console.log("Connected to WebSocket server", "received");
    };

    websocket.onmessage = function (event) {
      if (typeof event.data === "string") {
        console.log(`Received: ${event.data}`, "received");
      } else {
        // Handle binary data
        const bytes = Array.from(new Uint8Array(event.data));
        console.log(`Received binary: [${bytes.join(", ")}]`, "received");
      }
    };

    websocket.onclose = function () {
      console.log("Disconnected from WebSocket server", "info");
    };

    websocket.onerror = function (error) {
      const errorDetails = {
        type: error.type || "WebSocket Error",
        target: error.target ? "WebSocket" : "Unknown",
        timeStamp: new Date().toISOString(),
        userAgent: navigator.userAgent,
      };

      console.log(`Error: Connection failed`, "info");
      console.log(
        `Error Details: ${JSON.stringify(errorDetails, null, 2)}`,
        "info"
      );
    };

  }, []);

  function addFish(config: FishConfig) {
    setConfig(config);
    setViewState(2);
  }

  function updateDevice(config: FishConfig) {
    setConfig(config);
    setViewState(3);
  }

  function submitFish(config: FishConfig) {
    setConfig(config);
    if (config) {
      if (ws && ws.readyState === WebSocket.OPEN) {
        // ws.send("addFish");
        ws.send(JSON.stringify({
            type: "fishAdded",
            data: JSON.stringify(config),
        }));
        console.log(`Sent: addFish`, "sent");
      }
    }
  }

  function returnHome() {
    setViewState(0);
    setConfig(defaultFish);
  }

  return (
    <>
      <div className="p-1">
        {(viewState == 0) && <StartView getStarted={() => setViewState(1)}/>}
        {(viewState == 1) && <FishDesignerView config={config} addFish={addFish} goBack={() => setViewState(0)}></FishDesignerView>}
        {(viewState == 2) && <SelectDeviceView config={config} connected={true} goBack={() => setViewState(1)} updateDevice={updateDevice}></SelectDeviceView>}
        {(viewState == 3) && <SwipeView submitFish={submitFish} config={config} returnHome={returnHome}></SwipeView>}

      </div>
         </>
  )
}

export default App
