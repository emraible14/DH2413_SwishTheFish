import { CompleteFishModel } from "@/components/complete-fish-model";
import { OverheadCamera } from "@/components/overhead-camera";
import { Button } from "@/components/ui/button";

import type { FishConfig } from "@/utils/types";
import { Canvas } from "@react-three/fiber";
import { useRef, useState } from "react";

interface SwipeViewProps {
  submitFish: (config: FishConfig) => void;
  config: FishConfig;
  returnHome: () => void;
}

function SwipeView(props: SwipeViewProps) {

  const [showReturnHome, setShowReturnHome] = useState(false);
  const [showInstructions, setShowInstructions] = useState(true);
  const [swimTriggered, setSwimTriggered] = useState(false);
  const touchStartY = useRef<number | null>(null);

  function handleTouchStart(e: React.TouchEvent) {
    touchStartY.current = e.touches[0].clientY;
  }

  function handleTouchEnd(e: React.TouchEvent) {
    if (touchStartY.current === null) return;
    const endY = e.changedTouches[0].clientY;
    const deltaY = endY - touchStartY.current;

    // If user swiped up by at least 30px
    if (deltaY < -30 && !swimTriggered) {
      setSwimTriggered(true);
      setShowInstructions(false);
      props.submitFish(props.config);
    }

    touchStartY.current = null;
  }

  return (
    <>
      <div style={{height: '100vh', width: '100%', touchAction: 'none'}} 
        onTouchStart={handleTouchStart}
        onTouchEnd={handleTouchEnd}>
        {showInstructions && (
          <div className="absolute inset-0 flex justify-center pt-10">
            <h1><b>Swipe to Swish your Fish!</b></h1>
          </div>
        )}
        <Canvas camera={{ position: [0, 20, 0], fov: 75 }}>
          <ambientLight intensity={0.5} />
          <directionalLight position={[5, 5, 5]} />
          <CompleteFishModel config={props.config} onSwimAway={() => {
            setShowReturnHome(true);
          }} swimming={swimTriggered}/>
          <OverheadCamera />
        </Canvas>
        {showReturnHome && (
          <div className="absolute inset-0 flex justify-center items-center">
            <Button onClick={() => {
              props.returnHome();
            }}>
              Finished
            </Button>
          </div>
        )}
      </div>
    </>
  )
}

export default SwipeView;
