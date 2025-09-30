import { Center, useGLTF } from '@react-three/drei'
import { useEffect, useRef, useState } from 'react';
import type { Group } from 'three';

interface SubFishModelProps {
  componentPart: string,
  componentId: number,
  color: string,
}

export function SubFishModel(props: SubFishModelProps) {
  const { scene, materials } = useGLTF(`/${props.componentPart}${props.componentId}.glb`) // or .glb
  const [position, setPosition] = useState([0,0,0]);

  useEffect(() => {
    Object.keys(materials).forEach((matName : string) => {
      if (matName != "Material.003" && matName != "Material.004") {
        (materials[matName] as any).color.set(props.color);
      }
    });
  }, [materials, props])

  useEffect(() => {
    if (props.componentPart === 'head') {
      setPosition([0, 0, -2.1]);
    } else if (props.componentPart === 'tail') {
      setPosition([0, 0, 1.9]);
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
