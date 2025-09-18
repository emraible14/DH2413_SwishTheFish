import { useState } from 'react';
import './App.css'
import type { FishConfig } from './utils/types';
import FishDesignerView from './views/FishDesignerView';
import StartView from './views/StartView';
import FinalView from './views/FinalView';

function App() {

  const [viewState, setViewState] = useState(0);
  const [config, setConfig] = useState<FishConfig>({
    tailId: 0,
    bodyId: 0,
    headId: 0,
    color: '#000000',
    deviceId: null,
  });

  let ws: WebSocket | null = null;

  function connect() {
    try {
      ws = new WebSocket("ws://localhost:3001");

      ws.onopen = function () {
        console.log("Connected to WebSocket server", "received");
      };

      ws.onmessage = function (event) {
        if (typeof event.data === "string") {
          console.log(`Received: ${event.data}`, "received");
        } else {
          // Handle binary data
          const bytes = Array.from(new Uint8Array(event.data));
          console.log(`Received binary: [${bytes.join(", ")}]`, "received");
        }
      };

      ws.onclose = function () {
        console.log("Disconnected from WebSocket server", "info");
      };

      ws.onerror = function (error) {
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
    } catch (error) {
      const errorDetails = {
        type: "Connection Error",
        timeStamp: new Date().toISOString(),
        userAgent: navigator.userAgent,
      };
      console.log(error)

      console.log(`Connection error: Unknown Error`, "info");
      console.log(
        `Error Details: ${JSON.stringify(errorDetails, null, 2)}`,
        "info"
      );
    }
  }

  function addFish(config: FishConfig) {
    setConfig(config);
    console.log(config);
    setViewState(2);
  }

  function submitFish(config: FishConfig) {
    setConfig(config);
    if (config) {
      console.log(config);
      if (ws && ws.readyState === WebSocket.OPEN) {
        // ws.send("addFish");
        ws.send(JSON.stringify({
            type: "fishAdded",
            data: config,
        }));
        console.log(`Sent: addFish`, "sent");
        setViewState(0);
      }
    }
  }

  // Initialize
  connect();

  function getStarted() {
    setViewState(1);
  }

  return (
    <>
      {(viewState == 0) && <StartView getStarted={getStarted}/>}
      {(viewState == 1) && <FishDesignerView addFish={addFish}></FishDesignerView>}
      {(viewState == 2) && <FinalView submitFish={submitFish} config={config} connected={true}></FinalView>}
    </>
  )
}

export default App
