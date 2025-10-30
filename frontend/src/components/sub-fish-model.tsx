import { Center, useGLTF } from '@react-three/drei'
import { useEffect, useRef, useState } from 'react';
import { Vector3, type Group } from 'three';

interface SubFishModelProps {
  componentPart: string,
  componentId: number,
  color: string,
}

export function SubFishModel(props: SubFishModelProps) {
  const { scene, materials } = useGLTF(`/${props.componentPart}${props.componentId}.glb`) // or .glb
  const [position, setPosition] = useState<Vector3>(new Vector3(0, 0, 0));

  useEffect(() => {
    Object.keys(materials).forEach((matName : string) => {
      if (matName.slice(0, 3) !== "Mat") { // hacky way to avoid coloring eyes
        (materials[matName] as any).color.set(props.color);
      }
    });
  }, [materials, props])

  useEffect(() => {
    if (props.componentPart === 'head') {
      setPosition(new Vector3(0, 0, -2.1));
    } else if (props.componentPart === 'tail') {
      setPosition(new Vector3(0, 0, 1.9));
    }
  }, [props]);

  const fishRef = useRef<Group>(null!)

  return (
    <group ref={fishRef} position={position}>
      <Center>
        <primitive object={scene} />
      </Center>
    </group>
  )
}

useGLTF.preload('/head1.glb')
useGLTF.preload('/head2.glb')
useGLTF.preload('/head3.glb')
useGLTF.preload('/head4.glb')
useGLTF.preload('/head5.glb')
useGLTF.preload('/tail1.glb')
useGLTF.preload('/tail2.glb')
useGLTF.preload('/tail3.glb')
useGLTF.preload('/tail4.glb')
useGLTF.preload('/tail5.glb')
useGLTF.preload('/body1.glb')
useGLTF.preload('/body2.glb')
useGLTF.preload('/body3.glb')
useGLTF.preload('/body4.glb')
useGLTF.preload('/body5.glb')
useGLTF.preload('/body6.glb')
useGLTF.preload('/body7.glb')

