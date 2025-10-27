import { Button } from "@/components/ui/button";
import { Spinner } from "@/components/ui/spinner";
import { Suspense } from "react";

interface StartViewProps {
  getStarted: () => void;
}

function StartView(props: StartViewProps) {

  return (
    <>
      <div className='absolute inset-0 flex flex-col items-center justify-evenly'>
        <div className="flex flex-col items-center justify-center gap-2">
          <div style={{height: '200px', width: '200px'}}>
            <Suspense fallback={<Spinner/>}>
              <img src="/logo3.png" height={200} width={200}></img>
            </Suspense>
          </div>
          <div>
            <h1><b>Swish the Fish!</b></h1>
          </div>
          <Button id="sendBtn" onClick={props.getStarted}> Get Started!</Button>
        </div>
      </div>
    </>
  )
}

export default StartView;
