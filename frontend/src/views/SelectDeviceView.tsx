import { CompleteFishModel } from "@/components/complete-fish-model";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
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

interface SelectDeviceViewProps {
  config: FishConfig;
  connected: boolean;
  goBack: () => void;
  updateDevice: (config: FishConfig) => void;
}

function SelectDeviceView(props: SelectDeviceViewProps) {
  const [device, selectDevice] = useState<string | null>(null);

  return (
    <>
      <div className="flex flex-col inset-0 justify-center items-center h-full">
        <div className="flex flex-col justify-center items-center">
          <h1><b>Looking Good!</b></h1>
          <Label className="m-6">Feel free to inspect your wonderful creation until a connection device is available</Label>
          <div className="shrink-0">

          <Select onValueChange={(value) => selectDevice(value)}>
            <SelectTrigger className="w-[180px]">
                <SelectValue placeholder="Enter Device ID" />
            </SelectTrigger>
            <SelectContent>
                <SelectGroup>
                <SelectLabel>Select Device ID</SelectLabel>
                {/* <SelectItem value="1">1</SelectItem>
                <SelectItem value="2">2</SelectItem>
                <SelectItem value="3">3</SelectItem> */}
                <SelectItem value="4">4</SelectItem>
                </SelectGroup>
            </SelectContent>
          </Select>
          </div>
          <div style={{height: '60vh', width: '100%'}}>
            <Canvas  camera={{ fov: 8, position: [0, 60, 40] }}>
              <ambientLight intensity={0.5} />
              <directionalLight position={[5, 5, 5]} />
              <OrbitControls enableZoom={true} />
              <CompleteFishModel config={props.config}/>
            </Canvas>
          </div>
          <div className="flex flex-row w-100 justify-between p-4">
            <Button id="sendBtn" onClick={props.goBack} >Back</Button>
            <Button id="sendBtn" onClick={() => {
              const newConfig = {...props.config}
              newConfig.deviceId = device;
              props.updateDevice(newConfig);
            }} disabled={device === null || !props.connected}>Next</Button>
          </div>
        </div>
      </div>
    </>
  )
}

export default SelectDeviceView;
