import { CompleteFishModel } from "@/components/complete-fish-model";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
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
      <div className="absolute inset-0 flex flex-col justify-center items-center h-full bg-cyan-100">
        <div className="flex flex-col justify-center items-center">
          <h1 className="p-4"><b>Looks Great!</b></h1>
          <Label className="mx-6">Inspect your fish until a phone holder is ready</Label>
          
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
          <div className="flex flex-col items-center gap-3">
            <Label>The phone holder given to you is...</Label>
            <RadioGroup defaultValue="option-one" onValueChange={(value) => selectDevice(value)}>
              <div className="flex items-center space-x-2">
                <RadioGroupItem value="5" id="5" className="text-red-600 border-red-600 [&_svg]:fill-red-600"/>
                <Label htmlFor="5" className="text-red-600">Red</Label>
              </div>
              <div className="flex items-center space-x-2">
                <RadioGroupItem value="6" id="6" className="text-green-600 border-green-600 [&_svg]:fill-green-600"/>
                <Label htmlFor="6" className="text-green-600">Green</Label>
              </div>
            </RadioGroup>
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
