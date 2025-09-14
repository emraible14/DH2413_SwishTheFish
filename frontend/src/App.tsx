import './App.css'
import { Canvas } from '@react-three/fiber'
import { OrbitControls } from "@react-three/drei";
import { Model } from "./components/clown-fish"

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious } from './components/ui/carousel';

function App() {

  let ws: WebSocket | null = null;
  // let intervalId: NodeJS.Timeout | null = null;

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
          // message: error.message || "Connection failed",
          // code: error.code || "Unknown",
          // reason: error.reason || "No reason provided",
          // wasClean: error.wasClean || false,
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
        // message: error.message || "Unknown error",
        // name: error.name || "Error",
        // stack: error.stack || "No stack trace",
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

  // function disconnect() {
  //   if (ws) {
  //     ws.close();
  //     ws = null;
  //   }
  // }

  function sendMessage() {
    if (ws && ws.readyState === WebSocket.OPEN) {
      ws.send("addFish");
      console.log(`Sent: addFish`, "sent");
    }
  }

  // function sendIntervalMessage() {
  //   intervalId = setInterval(() => {
  //     sendMessage();
  //   }, 1000);
  // }

  // function stopIntervalMessage() {
  //   if (intervalId) {
  //     clearInterval(intervalId);
  //     intervalId = null;
  //     const button = document.getElementById("sendIntervalBtn");
  //     button.textContent = "Add Fish Every Second";
  //     button.onclick = function () {
  //       sendIntervalMessage();
  //     };
  //   }
  // }

  // Initialize
  connect();

  return (
    <>
      
      <div id="canvas-container">
        <Canvas camera={{ fov: 64, position: [-2, 2, 0] }}>
          <ambientLight intensity={5} />
          <OrbitControls enableZoom={true} />
          <Model material_color={'green'}/>
        </Canvas>
      </div>
      <Card>
        <CardHeader>
          <CardTitle>
            <h1>Design your Fish:</h1>
          </CardTitle>
        </CardHeader>
        <CardContent className='flex flex-col items-center'>
          <Carousel className="w-full max-w-xs">
            <CarouselContent>
              {Array.from({ length: 5 }).map((_, index) => (
                <CarouselItem key={index}>
                  <div className="p-1">
                  <Canvas>
                  <ambientLight intensity={0.3} />
                  <directionalLight color="white" position={[-5, -5, -5]} />
                  <mesh position={[0, 0, 0]} scale={1}>
                    <sphereGeometry/>
                    <meshStandardMaterial />
                  </mesh>
                </Canvas>
                  </div>
                </CarouselItem>
              ))}
            </CarouselContent>
            <CarouselPrevious />
            <CarouselNext />
          </Carousel>
          <Carousel className="w-full max-w-xs">
            <CarouselContent>
              {Array.from({ length: 5 }).map((_, index) => (
                <CarouselItem key={index}>
                  <div className="p-1">
                  <Canvas>
                  <ambientLight intensity={0.3} />
                  <directionalLight color="white" position={[-5, -5, -5]} />
                  <mesh position={[0, 0, 0]} scale={1}>
                    <sphereGeometry/>
                    <meshStandardMaterial />
                  </mesh>
                </Canvas>
                  </div>
                </CarouselItem>
              ))}
            </CarouselContent>
            <CarouselPrevious />
            <CarouselNext />
          </Carousel>
          <Carousel className="w-full max-w-xs">
            <CarouselContent>
              {Array.from({ length: 5 }).map((_, index) => (
                <CarouselItem key={index}>
                  <div className="p-1">
                  <Canvas>
                  <ambientLight intensity={0.3} />
                  <directionalLight color="white" position={[-5, -5, -5]} />
                  <mesh position={[0, 0, 0]} scale={1}>
                    <sphereGeometry/>
                    <meshStandardMaterial />
                  </mesh>
                </Canvas>
                  </div>
                </CarouselItem>
              ))}
            </CarouselContent>
            <CarouselPrevious />
            <CarouselNext />
          </Carousel>
        </CardContent>
        <CardFooter className="flex-col gap-2">
          <Button id="sendBtn" onClick={sendMessage} className="w-full"> Add Fish</Button>
        </CardFooter>
      </Card>
      <div>
        </div>
    </>
  )
}

export default App
