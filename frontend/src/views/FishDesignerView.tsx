import { Canvas } from '@react-three/fiber'
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious, type CarouselApi } from '../components/ui/carousel';
import { Head } from '../components/head';
import { Body } from '../components/body';
import { Tail } from '../components/tail';
import { OverheadCamera } from '../components/overhead-camera';
import { useState } from 'react';
import type { FishConfig } from '@/utils/types';

interface FishDesignerProps {
  addFish: (config: FishConfig) => void;
}

function FishDesignerView(props: FishDesignerProps) {
  const [tailCarouselApi, setTailCarouselApi] = useState<CarouselApi>();
  const [bodyCarouselApi, setBodyCarouselApi] = useState<CarouselApi>();
  const [headCarouselApi, setHeadCarouselApi] = useState<CarouselApi>();
  const colorOptions = ['#ffb400', '#0ad86b', '#6f39f6', '#f83dea', '#00cad9'];
  const [color, setColor] = useState(colorOptions[0]);

  function sendConfig() {
    const config: FishConfig = {
      tailId: tailCarouselApi?.selectedScrollSnap() ?? 0,
      bodyId: bodyCarouselApi?.selectedScrollSnap() ?? 0,
      headId: headCarouselApi?.selectedScrollSnap() ?? 0,
      color: color,
      deviceId: null,
    }
    props.addFish(config);
  }

  return (
    <>
      <Card>
        <CardHeader>
          <CardTitle>
            <h1>Design your Fish:</h1>
          </CardTitle>
        </CardHeader>
        <CardContent className='flex flex-col items-center'>
          <div className='flex flex-row justify-between gap-3'>
            {colorOptions.map((buttonColor) => (
              <Button key={buttonColor} style={{backgroundColor: buttonColor}} onClick={() => setColor(buttonColor)} size="icon" className="size-8"/>
            ))}
          </div>
          <Carousel className="w-50" setApi={setTailCarouselApi}>
            <CarouselContent>
              {Array.from({ length: 3 }).map((_, index) => (
                <CarouselItem key={index}>
                  <Canvas  camera={{ fov: 7, position: [0, 50, 0] }}>
                    <directionalLight intensity={2} />
                    <OverheadCamera />
                    <Tail material_color={color} position={[0, 0, 2.2]}/>
                  </Canvas>
                </CarouselItem>
              ))}
            </CarouselContent>
            <CarouselPrevious />
            <CarouselNext />
          </Carousel>
          <Carousel className="w-50" setApi={setBodyCarouselApi}>
            <CarouselContent>
              {Array.from({ length: 3 }).map((_, index) => (
                <CarouselItem key={index}>
                  <Canvas  camera={{ fov: 4.5, position: [0, 50, 0] }}>
                    <directionalLight intensity={2} />
                    <OverheadCamera />
                    <Body material_color={color}/>
                  </Canvas>
                </CarouselItem>
              ))}
            </CarouselContent>
            <CarouselPrevious />
            <CarouselNext />
          </Carousel>
          <Carousel className="w-50" setApi={setHeadCarouselApi}>
            <CarouselContent>
              {Array.from({ length: 3 }).map((_, index) => (
                <CarouselItem key={index}>
                    <Canvas  camera={{ fov: 4.5, position: [0, 50, 0] }}>
                      <directionalLight intensity={2} />
                      <OverheadCamera />
                      <Head material_color={color} position={[0, 0, -1]}/>
                    </Canvas>
                </CarouselItem>
              ))}
            </CarouselContent>
            <CarouselPrevious />
            <CarouselNext />
          </Carousel>
        </CardContent>
        <CardFooter className="flex-col gap-2">
          <Button id="sendBtn" onClick={sendConfig} className="w-50">Next</Button>
        </CardFooter>
      </Card>
    </>
  )
}

export default FishDesignerView;
