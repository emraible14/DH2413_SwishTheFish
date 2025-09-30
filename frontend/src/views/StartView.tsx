import { Button } from "@/components/ui/button";

interface StartViewProps {
  getStarted: () => void;
}

function StartView(props: StartViewProps) {

  return (
    <>
      <div className='absolute inset-0 flex flex-col items-center justify-evenly'>
        <div className="flex flex-col items-center justify-center gap-5">
          <div>
            <img src="/logo.png" height={150} width={150}></img>
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
