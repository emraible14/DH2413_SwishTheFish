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
  const [tailColor, setTailColor] = useState(colorOptions[0]);
  const [bodyColor, setBodyColor] = useState(colorOptions[0]);
  const [headColor, setHeadColor] = useState(colorOptions[0]);

  const [tailIndex, setTailIndex] = useState(0);
  const [bodyIndex, setBodyIndex] = useState(0);
  const [headIndex, setHeadIndex] = useState(0);


  useEffect(() => {
    // Only run once all APIs are ready
    if (!headCarouselApi || !bodyCarouselApi || !tailCarouselApi) return;

    // Scroll to the indices (adjusting for 1-based IDs)
    if (props.config.headId > 0) headCarouselApi.scrollTo(props.config.headId - 1);
    if (props.config.bodyId > 0) bodyCarouselApi.scrollTo(props.config.bodyId - 1);
    if (props.config.tailId > 0) tailCarouselApi.scrollTo(props.config.tailId - 1);

    // Update color if needed
    if (props.config.tailColor && props.config.tailColor !== "#000000") {
      setTailColor(props.config.tailColor);
    }
    if (props.config.bodyColor && props.config.bodyColor !== "#000000") {
      setBodyColor(props.config.tailColor);
    }
    if (props.config.headColor && props.config.headColor !== "#000000") {
      setHeadColor(props.config.tailColor);
    }

  }, [props.config, headCarouselApi, bodyCarouselApi, tailCarouselApi]);

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
      tailId: tailCarouselApi?.selectedScrollSnap() ? tailCarouselApi?.selectedScrollSnap() + 1 : 0 + 1,
      bodyId: bodyCarouselApi?.selectedScrollSnap() ? bodyCarouselApi?.selectedScrollSnap() + 1 : 0 + 1,
      headId: headCarouselApi?.selectedScrollSnap() ? headCarouselApi?.selectedScrollSnap() + 1 : 0 + 1,
      tailColor: tailColor,
      bodyColor: bodyColor,
      headColor: headColor,
      deviceId: null,
    }
    props.addFish(config);
  }

  return (
    <>
      <div className='inset-0 flex flex-col items-center justify-center'>
        <h1 className='p-4'><b>Design your Fish:</b></h1>
        <div className='flex flex-col gap-1'>
          <div className='flex flex-row justify-center gap-3'>
            <div className='flex justify-center w-15'>Tail:</div>
            {colorOptions.map((buttonColor) => (
              <Button key={buttonColor} style={{backgroundColor: buttonColor}} onClick={() => setTailColor(buttonColor)} size="icon" className="size-8"/>
            ))}
          </div>
          <div className='flex flex-row justify-center gap-3'>
            <div className='flex justify-center w-15'>Body:</div>
            {colorOptions.map((buttonColor) => (
              <Button key={buttonColor} style={{backgroundColor: buttonColor}} onClick={() => setBodyColor(buttonColor)} size="icon" className="size-8"/>
            ))}
          </div>
          <div className='flex flex-row justify-center gap-3'>
          <div className='flex justify-center w-15'>Head:</div>
            {colorOptions.map((buttonColor) => (
              <Button key={buttonColor} style={{backgroundColor: buttonColor}} onClick={() => setHeadColor(buttonColor)} size="icon" className="size-8"/>
            ))}
          </div>
        </div>
        <div className='flex flex-col justify-center items-center'>
          <Carousel className="w-60" setApi={setTailCarouselApi}>
            <CarouselContent>
              {Array.from({ length: 5 }).map((_, index) => (
                <CarouselItem key={index}>
                  {index === tailIndex && <Canvas className="w-full h-full" camera={{ fov: 7.5, position: [0, 50, 0] }}>
                    <ambientLight intensity={0.5} />
                    <directionalLight position={[5, 5, 5]} />
                    <OverheadCamera />
                    <SubFishModel color={tailColor} componentPart='tail' componentId={index+1}/>
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
              {Array.from({ length: 7 }).map((_, index) => (
                <CarouselItem key={index}>
                  {index === bodyIndex && <Canvas className="w-full h-full" camera={{ fov: 7.5, position: [0, 50, 0] }}>
                    <ambientLight intensity={0.5} />
                    <directionalLight position={[5, 5, 5]} />
                    <OverheadCamera />
                    <SubFishModel color={bodyColor} componentPart='body' componentId={index+1}/>
                  </Canvas>}
                </CarouselItem>
              ))}
            </CarouselContent>
            <CarouselPrevious />
            <CarouselNext />
          </Carousel>
          <Carousel className="w-60" setApi={setHeadCarouselApi}>
            <CarouselContent>
              {Array.from({ length: 5 }).map((_, index) => (
                <CarouselItem key={index}>
                    {index === headIndex && <Canvas className="w-full h-full" camera={{ fov: 7.5, position: [0, 50, 0] }}>
                      <ambientLight intensity={0.5} />
                      <directionalLight position={[5, 5, 5]} />
                      <SubFishModel color={headColor} componentPart='head' componentId={index+1}/>
                    </Canvas>}
                </CarouselItem>
              ))}
            </CarouselContent>
            <CarouselPrevious />
            <CarouselNext />
          </Carousel>
        </div>
        <div className='flex flex-row justify-between w-100 pr-4 pl-4'>
          <Button id="sendBtn" onClick={props.goBack} >Back</Button>
          <Button id="sendBtn" onClick={sendConfig} >Next</Button>
        </div>
      </div>
    </>
  )
}

export default FishDesignerView;
