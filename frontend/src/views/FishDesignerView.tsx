import { Canvas } from '@react-three/fiber'
import { Button } from "@/components/ui/button";
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious, type CarouselApi } from '../components/ui/carousel';
import { OverheadCamera } from '../components/overhead-camera';
import { useEffect, useState } from 'react';
import type { FishConfig } from '@/utils/types';
import { SubFishModel } from '@/components/sub-fish-model';

interface FishDesignerProps {
  config: FishConfig;
  addFish: (config: FishConfig) => void;
  goBack: () => void;
}

function FishDesignerView(props: FishDesignerProps) {
  const [tailCarouselApi, setTailCarouselApi] = useState<CarouselApi>();
  const [bodyCarouselApi, setBodyCarouselApi] = useState<CarouselApi>();
  const [headCarouselApi, setHeadCarouselApi] = useState<CarouselApi>();
  const colorOptions = ['#ffb400', '#0ad86b', '#6f39f6', '#f83dea', '#00cad9'];
  const [color, setColor] = useState(colorOptions[0]);

  const [tailIndex, setTailIndex] = useState(0);
  const [bodyIndex, setBodyIndex] = useState(0);
  const [headIndex, setHeadIndex] = useState(0);


  useEffect(() => {
    if (props.config.headId != 0) {
      headCarouselApi?.scrollTo(props.config.headId);
    }
    if (props.config.bodyId != 0) {
      bodyCarouselApi?.scrollTo(props.config.bodyId);
    }
    if (props.config.tailId != 0) {
      tailCarouselApi?.scrollTo(props.config.tailId);
    }
    if (props.config.color != "#000000") {
      setColor(props.config.color);
    }
  }, []);

  useEffect(() => {
    if (!tailCarouselApi) return;
  
    // set initial index
    setTailIndex(tailCarouselApi.selectedScrollSnap());
  
    const onSelect = () => {
      setTailIndex(tailCarouselApi.selectedScrollSnap());
    };
  
    tailCarouselApi.on("select", onSelect);
    return () => {
      tailCarouselApi.off("select", onSelect);
    };
  }, [tailCarouselApi]);

  useEffect(() => {
    if (!headCarouselApi) return;
  
    // set initial index
    setHeadIndex(headCarouselApi.selectedScrollSnap());
  
    const onSelect = () => {
      setHeadIndex(headCarouselApi.selectedScrollSnap());
    };
  
    headCarouselApi.on("select", onSelect);
    return () => {
      headCarouselApi.off("select", onSelect);
    };
  }, [headCarouselApi]);

  useEffect(() => {
    if (!bodyCarouselApi) return;
  
    // set initial index
    setBodyIndex(bodyCarouselApi.selectedScrollSnap());
  
    const onSelect = () => {
      setBodyIndex(bodyCarouselApi.selectedScrollSnap());
    };
  
    bodyCarouselApi.on("select", onSelect);
    return () => {
      bodyCarouselApi.off("select", onSelect);
    };
  }, [bodyCarouselApi]);


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
      <div className='inset-0 flex flex-col items-center justify-center'>
        <h1 className='p-4'><b>Design your Fish:</b></h1>
        <div className='flex flex-row justify-center gap-3'>
          {colorOptions.map((buttonColor) => (
            <Button key={buttonColor} style={{backgroundColor: buttonColor}} onClick={() => setColor(buttonColor)} size="icon" className="size-8"/>
          ))}
        </div>
        <div className='flex flex-col justify-center items-center'>
          <Carousel className="w-60" setApi={setTailCarouselApi}>
            <CarouselContent>
              {Array.from({ length: 3 }).map((_, index) => (
                <CarouselItem key={index}>
                  {index === tailIndex && <Canvas className="w-full h-full" camera={{ fov: 7.5, position: [0, 50, 0] }}>
                    <ambientLight intensity={0.5} />
                    <directionalLight position={[5, 5, 5]} />
                    <OverheadCamera />
                    <SubFishModel color={color} componentPart='tail' componentId={index+3}/>
                    {/* <Tail material_color={color} position={[0, 0, 2.2]}/> */}
                  </Canvas>}
                </CarouselItem>
              ))}
            </CarouselContent>
            <CarouselPrevious />
            <CarouselNext />
          </Carousel>
          <Carousel className="w-60" setApi={setBodyCarouselApi}>
            <CarouselContent>
              {Array.from({ length: 4 }).map((_, index) => (
                <CarouselItem key={index}>
                  {index === bodyIndex && <Canvas className="w-full h-full" camera={{ fov: 7.5, position: [0, 50, 0] }}>
                    <ambientLight intensity={0.5} />
                    <directionalLight position={[5, 5, 5]} />
                    <OverheadCamera />
                    <SubFishModel color={color} componentPart='body' componentId={index+3}/>
                  </Canvas>}
                </CarouselItem>
              ))}
            </CarouselContent>
            <CarouselPrevious />
            <CarouselNext />
          </Carousel>
          <Carousel className="w-60" setApi={setHeadCarouselApi}>
            <CarouselContent>
              {Array.from({ length: 3 }).map((_, index) => (
                <CarouselItem key={index}>
                    {index === headIndex && <Canvas className="w-full h-full" camera={{ fov: 7.5, position: [0, 50, 0] }}>
                      <ambientLight intensity={0.5} />
                      <directionalLight position={[5, 5, 5]} />
                      <SubFishModel color={color} componentPart='head' componentId={index+3}/>
                      {/* <Head material_color={color} position={[0, 0, -1]}/> */}
                    </Canvas>}
                </CarouselItem>
              ))}
            </CarouselContent>
            <CarouselPrevious />
            <CarouselNext />
          </Carousel>
        </div>
        <div className='flex flex-row justify-between w-100 p-4'>
          <Button id="sendBtn" onClick={props.goBack} >Back</Button>
          <Button id="sendBtn" onClick={sendConfig} >Next</Button>
        </div>
      </div>
    </>
  )
}

export default FishDesignerView;
