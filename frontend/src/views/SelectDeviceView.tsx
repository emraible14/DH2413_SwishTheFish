import { CompleteFishModel } from "@/components/complete-fish-model";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Spinner } from "@/components/ui/spinner";
import type { FishConfig } from "@/utils/types";
import { Html, OrbitControls, useProgress } from "@react-three/drei";
import { Canvas } from "@react-three/fiber";
import { Suspense, useState } from "react";

interface SelectDeviceViewProps {
  config: FishConfig;
  connected: boolean;
  goBack: () => void;
  updateDevice: (config: FishConfig) => void;
}

function SelectDeviceView(props: SelectDeviceViewProps) {
  const [device, selectDevice] = useState<string | null>(null);

  function Loader() {
    const { progress } = useProgress()
    return (
      <Html center>
        <div className="flex flex-col items-center justify-center">
          <Spinner/>
          <div style={{ color: 'black', fontSize: '1.5em' }} >
            Loading... {progress.toFixed(0)}%
          </div>
        </div>
      </Html>
    )
  }

  return (
    <>
      <div className="flex flex-col inset-0 justify-center items-center h-full">
        <div className="flex flex-col justify-center items-center">
          <h1 className="p-4"><b>Looks Great!</b></h1>
          <Label className="mx-6">Feel free to inspect your wonderful creation until a phone holder is available</Label>
          
          <div style={{height: '60vh', width: '100%'}}>
            <Canvas  camera={{ fov: 15, position: [0, 60, 40] }}>
              <ambientLight intensity={0.5} />
              <directionalLight position={[5, 5, 5]} />
              <OrbitControls enableZoom={true} />
              <Suspense fallback={<Loader/>}>
                <CompleteFishModel config={props.config} swimming={false}/>
              </Suspense>
            </Canvas>
          </div>
          <div className="shrink-0">
            <Select onValueChange={(value) => selectDevice(value)}>
              <SelectTrigger className="w-[180px]">
                  <SelectValue placeholder="Phone Holder Color" />
              </SelectTrigger>
              <SelectContent>
                  <SelectGroup>
                  <SelectItem value="6">Blue</SelectItem>
                  <SelectItem value="5">Red</SelectItem>
                  </SelectGroup>
              </SelectContent>
            </Select>
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
