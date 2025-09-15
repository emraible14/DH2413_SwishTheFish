import { CompleteFish } from "@/components/complete-fish";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import type { FishConfig } from "@/utils/types";
import { OrbitControls } from "@react-three/drei";
import { Canvas } from "@react-three/fiber";
import { useState } from "react";

interface FinalViewProps {
  submitFish: (config: FishConfig) => void;
  config: FishConfig;
  connected: boolean;
}

function FinalView(props: FinalViewProps) {
  const [device, selectDevice] = useState<string | null>(null);

  return (
    <>
      <Card>
        <CardHeader>
          <CardTitle>
          </CardTitle>
        </CardHeader>
        <CardContent className='flex flex-col items-center'>
          <div style={{height: '50vh', width: '50vw'}}>
            <Canvas  camera={{ fov: 10, position: [0, 50, 40] }}>
              <directionalLight intensity={2} />
              <OrbitControls enableZoom={true} />
              <CompleteFish material_color={props.config.color}/>
            </Canvas>
          </div>
          <Select onValueChange={(value) => selectDevice(value)}>
            <SelectTrigger className="w-[180px]">
                <SelectValue placeholder="Select a device" />
            </SelectTrigger>
            <SelectContent>
                <SelectGroup>
                <SelectLabel>Select Device ID</SelectLabel>
                <SelectItem value="1">1</SelectItem>
                <SelectItem value="2">2</SelectItem>
                <SelectItem value="3">3</SelectItem>
                <SelectItem value="4">4</SelectItem>
                </SelectGroup>
            </SelectContent>
          </Select>
        </CardContent>
        <CardFooter className="flex-col gap-2">
          <Button id="sendBtn" onClick={() => {
            const newConfig = {...props.config}
            newConfig.deviceId = device;
            props.submitFish(newConfig);
          }} className="w-50" disabled={device === null || !props.connected}>Swish Your Fish!</Button>
        </CardFooter>
      </Card>
    </>
  )
}

export default FinalView;
