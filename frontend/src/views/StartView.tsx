import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

interface StartViewProps {
  getStarted: () => void;
}

function StartView(props: StartViewProps) {

  return (
    <>
      <Card>
        <CardHeader>
          <CardTitle>
          </CardTitle>
        </CardHeader>
        <CardContent className='flex flex-col items-center'>
          <div className='flex flex-col align-middle items-center gap-2'>
            <div>
            <img src="src/assets/logo.png" height={150} width={150}></img>
            </div>
            <div>
            <h1><b>Swish the Fish!</b></h1>
            </div>
          </div>
        </CardContent>
        <CardFooter className="flex-col gap-2">
          <Button id="sendBtn" onClick={props.getStarted} className="w-50"> Get Started!</Button>
        </CardFooter>
      </Card>
    </>
  )
}

export default StartView;
